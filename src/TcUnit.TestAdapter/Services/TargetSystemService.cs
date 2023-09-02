using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TwinCAT;
using TwinCAT.Ads;

namespace TcUnit.Core
{
    public class TargetSystemService
    {
        private readonly AdsFileManager _adsFileManager;
        private readonly string _target;
        private readonly AmsNetId _amsAddress;
        private readonly TcAdsClient _adsConnection;
        private readonly RouteTarget _routeInfo;
        private readonly BehaviorSubject<ConnectionState> _connectionStateSubject = new BehaviorSubject<ConnectionState>(TwinCAT.ConnectionState.None);
        private readonly BehaviorSubject<AdsState> _adsStateSubject = new BehaviorSubject<AdsState>(TwinCAT.Ads.AdsState.Init);
        private static SystemServiceClass systemService = SingletonPlugIn<SystemServiceClass>.Instance;
        private System.Timers.Timer _adsStateUpdateTimer;
        private AdsState _currentAdsState;
        private bool _isDisposed;


        public event EventHandler<TargetStateChangedEventArgs> StateChanged;

        public bool IsConfigMode => _adsStateSubject.Value == TwinCAT.Ads.AdsState.Config;
        public bool IsRunMode => _adsStateSubject.Value == TwinCAT.Ads.AdsState.Run;
        public bool IsStopMode => _adsStateSubject.Value == TwinCAT.Ads.AdsState.Stop;

        public Version TcVersion { get; private set; }
        public DetailedTargetInfo TargetInfo { get; private set; }
        public IObservable<AdsState> AdsState => _adsStateSubject.AsObservable();
        public IObservable<ConnectionState> ConnectionState => _connectionStateSubject.AsObservable();

        public IAdsConnection AdsConnection => _adsConnection;

        public TargetSystemService(RouteTarget route)
        {
            _target = route.NetId.ToString();
            _routeInfo = route;
            _amsAddress = new AmsNetId(_target);
            _adsConnection = new TcAdsClient(new AdsClientSettings(80));
            _adsConnection.Connect(new AmsAddress(_target, AmsPort.SystemService));

            TargetInfo = new DetailedTargetInfo(route);
            _adsFileManager = new AdsFileManager(_adsConnection);

            _adsStateUpdateTimer = new System.Timers.Timer(1000);
            _adsStateUpdateTimer.Elapsed += this.OnAdsStateUpdate;
            _adsStateUpdateTimer.Enabled = true;

        }

        ~TargetSystemService()
        {
            Dispose(false);
        }

        public void Initialize()
        {
            ReadTwinCATVersion();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _adsStateUpdateTimer.Dispose();
                _adsConnection.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            _isDisposed = true;
        }

        public bool DownloadFileToTargetBootFolder(string localFile, string remoteFile, int chunksize = 1024 * 20)
        {
            ushort handle = _adsFileManager.OpenFile(remoteFile, AdsStandardDirectory.RelativeFromBootFolder, (UInt32)AdsFileMode.Write | (UInt32)AdsFileMode.Binary);

            if (handle <= 0)
                return false;

            FileStream fs = File.OpenRead(localFile);
            BinaryReader br = new BinaryReader(fs, System.Text.Encoding.Default);

            FileInfo i = new FileInfo(localFile);

            long totalSize = 0;
            bool successWrite = true;
            while (br.PeekChar() != -1)
            {
                var buffer = new byte[chunksize];
                var readCount = default(int);

                readCount = br.Read(buffer, 0, chunksize);
                successWrite = _adsFileManager.WriteToFile(handle, buffer, (int)readCount);

                if (successWrite)
                    totalSize += (long)readCount;
                else
                    break;
            }

            bool succClose = _adsFileManager.CloseFile(handle);

            return successWrite && succClose && (totalSize == i.Length);
        }

        public bool DownloadFileToTarget(string localFile, string remoteFile, int chunksize = 1024 * 20)
        {
            ushort handle = _adsFileManager.OpenFile(remoteFile, AdsStandardDirectory.Generic, (UInt32)AdsFileMode.Write | (UInt32)AdsFileMode.Binary);

            if (handle <= 0)
                return false;

            FileStream fs = File.OpenRead(localFile);
            BinaryReader br = new BinaryReader(fs, System.Text.Encoding.Default);

            FileInfo i = new FileInfo(localFile);

            long totalSize = 0;
            bool successWrite = true;
            while (br.PeekChar() != -1)
            {
                var buffer = new byte[chunksize];
                var readCount = default(int);

                readCount = br.Read(buffer, 0, chunksize);
                successWrite = _adsFileManager.WriteToFile(handle, buffer, (int)readCount);

                if (successWrite)
                    totalSize += (long)readCount;
                else
                    break;
            }

            bool succClose = _adsFileManager.CloseFile(handle);

            return successWrite && succClose && (totalSize == i.Length);
        }

        public bool UploadFileFromTargetBootFolder(string localFile, string remoteFile, int chunksize = 1024 * 20)
        {
            ushort handle = _adsFileManager.OpenFile(remoteFile, AdsStandardDirectory.RelativeFromBootFolder, (UInt32)AdsFileMode.Read | (UInt32)AdsFileMode.Binary);

            if (handle <= 0)
                return false;

            FileStream fs = File.OpenWrite(localFile);
            BinaryWriter bw = new BinaryWriter(fs, System.Text.Encoding.Default);
            long totalSize = 0;
            bool eof = false;
            do
            {
                byte[] buffer = _adsFileManager.ReadFromFile(handle, (uint)chunksize, out eof);
                if (buffer != null)
                {
                    bw.Write(buffer, 0, buffer.Length);
                    totalSize += buffer.Length;
                }
            }
            while (!eof);

            _adsFileManager.CloseFile(handle);
            fs.Flush(true);
            fs.Close();

            return true;
        }


        public bool DirectoryExists(StandardDirectory standardDirectory, string path)
        {
            var remoteIO = new RemoteIO(_adsConnection, "");
            return remoteIO.Exist(StandardDirectory.BootDir, path, 180);
        }

        public IList<AdsFileSystemInfo> FindFIles(StandardDirectory standardDirectory, string path)
        {
            var remoteIO = new RemoteIO(_adsConnection, "");
            IList<AdsFileSystemInfo> items;
            remoteIO.FindFiles(standardDirectory, path, "*.*", 180, out items);

            return items;
        }

        public void DeleteDirectoryRecursive(StandardDirectory standardDirectory, string path)
        {
            var remoteIO = new RemoteIO(_adsConnection, "");

            if (!remoteIO.Exist(standardDirectory, path, 180))
            {
                throw new DirectoryNotFoundException("Boot folder doas not exist on target");
            }

            IList<AdsFileSystemInfo> items;
            remoteIO.FindFiles(standardDirectory, path, "*.*", 180, out items);

            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                if (item.Name == "." || item.Name == "..")
                {
                    continue;
                }

                if (item.IsDirectory)
                {
                    var subFolder = path + GetDelimiterOfOS() + item.Name;
                    DeleteDirectoryRecursive(standardDirectory, subFolder);

                    var errorCode = remoteIO.DeleteDirectory(standardDirectory, subFolder, 180);

                    if(errorCode != AdsErrorCode.NoError)
                    {
                        throw new Exception("Could not delete directory (" + item.Name + ") during clean up of boot folder");
                    }
                }
                else
                {
                    var filePath = path + GetDelimiterOfOS() + item.Name;
                    //remoteIO.DeleteFile(standardDirectory, filePath, 30);
                    _adsFileManager.DeleteFile(filePath, AdsStandardDirectory.RelativeFromBootFolder);
                }
            }
        }

        public void CleanUpBootFolder()
        {
            DeleteDirectoryRecursive(StandardDirectory.BootDir, "");
        }

        public Task CleanUpBootFolderAsync()
        {
            try
            {
                return AsyncFactory.StartNew(() => CleanUpBootFolder());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool CreateDirectoryOnTarget(string path, AdsStandardDirectory pathType = AdsStandardDirectory.Generic)
            => _adsFileManager.CreateDirectory(path, pathType);

        public void Shutdown(TimeSpan delay)
            => _adsConnection.WriteControl(new StateInfo(TwinCAT.Ads.AdsState.Shutdown, 0));

        public void Reboot()
            => _adsConnection.WriteControl(new StateInfo(TwinCAT.Ads.AdsState.Shutdown, 1));

        public bool IsTcBSD => TargetInfo.OsImage.RTOperatingSystem.Platform == RTPlatform.TcBSD;
        public bool IsWindows10 => TargetInfo.OsImage.RTOperatingSystem.Platform == RTPlatform.Win32;
        public bool IsWindowsCE => TargetInfo.OsImage.RTOperatingSystem.Platform == RTPlatform.WinCE;
        public bool IsTcRTOS => TargetInfo.OsImage.RTOperatingSystem.Platform == RTPlatform.TcRTOS;

        public string GetDelimiterOfOS()
        {
            var seperator = "";

            if (IsTcBSD)
            {
                seperator = "/";
            }
            else if (IsWindowsCE)
            {
                seperator = "\\";
            }
            else if (IsTcRTOS)
            {
                seperator = "\\";
            }
            else // Win 7 - 10
            {
                seperator = "\\";
            }

            return seperator;
        }

        public IRoute AddRoute(Credentials credentials)
        {
            if (_routeInfo.NetId == AmsNetId.Local) return null;

            return systemService.AddRoute(_routeInfo, credentials, RouteAddressingType.IPAddress, null);
        }

        public void RemoveRoute()
        {
            systemService.RemoveRoute(_routeInfo, RouteTarget.Local, RouteChangeMode.Single, null);
            systemService.RefreshRoutes();
        }

        private void ReadTwinCATVersion()
        {

            if (!_adsConnection.IsConnected)
            {
                return;
            }

            var stream = new AdsStream(8);
            _adsConnection.Read((uint)160, 0, stream);

            var minor = BitConverter.ToInt16(stream.ToArray(), 0);
            var major = BitConverter.ToInt16(stream.ToArray(), 2);
            var rev = BitConverter.ToInt16(stream.ToArray(), 4);
            var build = BitConverter.ToInt16(stream.ToArray(), 6);

            TcVersion = new Version(major, minor, build, rev);

        }



        private void OnAdsStateUpdate(object sender, EventArgs args)
        {
            if (_isDisposed)
            {
                return;
            }
            _adsStateUpdateTimer.Enabled = false;
            getAdsState();
            _adsStateUpdateTimer.Enabled = true;
        }

        private void waitForAdsState(AdsState state)
        {
            bool isInRequestedState = false;
            while (!isInRequestedState)
            {
                Thread.Sleep(500);
                AdsState adsState = getAdsState();

                if (adsState == state)
                {
                    isInRequestedState = true;
                }
                else if (state != TwinCAT.Ads.AdsState.Reset)
                {
                    if (state == TwinCAT.Ads.AdsState.Reconfig && adsState == TwinCAT.Ads.AdsState.Config)
                    {
                        isInRequestedState = true;
                    }
                }
                else if (adsState == TwinCAT.Ads.AdsState.Run || adsState == TwinCAT.Ads.AdsState.Config)
                {
                    isInRequestedState = true;
                }
            }
        }

        private AdsState getAdsState()
        {
            try
            {
                DeviceInfo deviceInfo = _adsConnection.ReadDeviceInfo();
                onSetAdsState(_adsConnection.ReadState().AdsState);
            }
            catch (Exception)
            {
                onSetAdsState(TwinCAT.Ads.AdsState.Invalid);
            }
            return _currentAdsState;
        }

        public void SetConfigMode()
        {
            try
            {
                StateInfo stateInfo = new StateInfo(TwinCAT.Ads.AdsState.Reconfig, 0);
                _adsConnection.WriteControl(stateInfo);
                getAdsState();
            }
            catch (Exception)
            {
                onSetAdsState(TwinCAT.Ads.AdsState.Invalid);
            }
        }

        public void StartSystem()
        {
            try
            {
                StateInfo stateInfo = new StateInfo(TwinCAT.Ads.AdsState.Reset, 0);
                _adsConnection.WriteControl(stateInfo);
                getAdsState();
            }
            catch (Exception)
            {
                onSetAdsState(TwinCAT.Ads.AdsState.Invalid);
            }
        }

        private void onSetAdsState(AdsState newState)
        {
            if (_currentAdsState != newState)
            {
                _connectionStateSubject.OnNext(TwinCAT.ConnectionState.Connected);
                _adsStateSubject.OnNext(newState);

                TargetStateChangedEventArgs e = new TargetStateChangedEventArgs(newState, _currentAdsState);
                _currentAdsState = newState;
                if (StateChanged != null)
                {
                    StateChanged(this, e);
                }
            }
        }
    }
}
