﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using TwinCAT.Ads;
using TwinCAT.Ads.Extensions;
using static TcUnit.TestAdapter.Models.AdsFileSystemTypes;

namespace TcUnit.TestAdapter.Models
{
    public class TwinCATSystemService
    {
        private readonly string target;
        private readonly AdsClient adsClient;
        private readonly AmsNetId amsNetId;

        private readonly int DefaultChunkSize = 1024 * 1024;

        public TwinCATSystemService(AmsNetId target)
        {
            this.target = target.ToString();
            amsNetId = target;
            adsClient = new AdsClient();

            Connect();
        }

        public bool IsReachable()
        {
            if (amsNetId.IsLocal || AmsNetId.LocalHost == amsNetId)
            {
                return true;
            }

            adsClient.TryReadState(out StateInfo stateInfo);

            return stateInfo.AdsState != AdsState.Run || stateInfo.AdsState != AdsState.Config;
        }

        public bool IsInRunMode()
        {
            adsClient.TryReadState(out StateInfo stateInfo);
            return stateInfo.AdsState != AdsState.Run;
        }

        public void Connect()
        {
            adsClient.Timeout = (int)TimeSpan.FromMilliseconds(500).TotalMilliseconds;
            adsClient.Connect(amsNetId, (int)AmsPort.SystemService);
        }

        public bool Disconnect()
        {
            return adsClient.Disconnect();
        }

        public bool SwitchRuntimeState(AdsStateCommand state, TimeSpan timeout)
        {
            var result = adsClient.SetAdsState(state, TimeSpan.FromMilliseconds(500), timeout, true, false);
            return result.RequestSucceeded;
        }

        public Version GetVersionInfo()
        {
            ushort[] adsVersionInfo = new ushort[4];
            adsVersionInfo = (ushort[])adsClient.ReadAny(160, 0, typeof(ushort[]), new int[] { 4 });
            return new Version(adsVersionInfo[1], adsVersionInfo[0], adsVersionInfo[3], adsVersionInfo[2]);
        }

        public TargetInfo GetDeviceInfo()
        {
            var buffer = new Memory<byte>(new byte[2048]);

            TargetInfo device = new TargetInfo();

            adsClient.Read(700, 1, buffer);

            string data = Encoding.ASCII.GetString(buffer.ToArray());

            device.TargetType = GetValueFromTag("<TargetType>", data);
            device.HardwareModel = GetValueFromTag("<Model>", data);
            device.HardwareSerialNo = GetValueFromTag("<SerialNo>", data);
            device.HardwareVersion = GetValueFromTag("<CPUArchitecture>", data);
            device.HardwareDate = GetValueFromTag("<Date>", data);
            device.HardwareCPU = GetValueFromTag("<CPUVersion>", data);

            device.ImageDevice = GetValueFromTag("<ImageDevice>", data);
            device.ImageVersion = GetValueFromTag("<ImageVersion>", data);
            device.ImageLevel = GetValueFromTag("<ImageLevel>", data);
            device.ImageOsName = GetValueFromTag("<OsName>", data);
            device.ImageOsVersion = GetValueFromTag("<OsVersion>", data);

            device.TwinCATVersion = GetVersionInfo();

            device.RTPlatform = Common.RTOperatingSystem.GetRTPlatform(device.ImageOsName);

            return device;
        }

        private string GetValueFromTag(string tag, string value)
        {
            try
            {
                int idxstart = value.IndexOf(tag) + tag.Length;
                int endidx = value.IndexOf("</", idxstart);
                return value.Substring(idxstart, endidx - idxstart);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public Tuple<int, int> GetRealtimeSettings()
        {
            byte[] data = new byte[64];
            Memory<byte> readData = new Memory<byte>(data);

            using (AdsClient adsClient = new AdsClient())
            {
                adsClient.Connect(amsNetId, AmsPort.R0_Realtime);
                adsClient.Read(0x1, 0xD, readData);

                var sharedCpuCores = BitConverter.ToInt32(data, 0);
                var isolatedCpuCores = BitConverter.ToInt32(data, 4);

                return new Tuple<int, int>(sharedCpuCores, isolatedCpuCores);
            }
        }

        public bool CreateDirectoryInBootFolder(string name)
        {
            AdsFileSystem ctrl = new AdsFileSystem();

            if (!ctrl.Connect(target, (int)TwinCAT.Ads.AmsPort.SystemService))
                return false;

            return ctrl.CreateDirectory(name, AdsDirectory.PATH_BOOTPATH);
        }

        public bool DirectoryExistsInBootFolder(string name)
        {
            AdsFileSystem ctrl = new AdsFileSystem();

            if (!ctrl.Connect(target, (int)TwinCAT.Ads.AmsPort.SystemService))
                return false;

            return false;
        }

        public bool FileExists(string filePath, AdsDirectory standardDirectory = AdsDirectory.PATH_GENERIC)
        {
            AdsFileSystem ctrl = new AdsFileSystem();

            if (!ctrl.Connect(target, (int)TwinCAT.Ads.AmsPort.SystemService))
                return false;

            ushort handle = ctrl.OpenFile(filePath, standardDirectory, (UInt32)AdsFileOpenMode.FOPEN_MODEREAD | (UInt32)AdsFileOpenMode.FOPEN_MODEBINARY);

            if (handle > 0)
            {
                ctrl.CloseFile(handle);
                return true;
            }

            return false;
        }

        public bool FileExistsInBootFolder(string filePath)
        {
            return FileExists(filePath, AdsDirectory.PATH_BOOTPATH);
        }

        public bool DownloadFileToBootFolder(string localFile, string remoteFile)
        {
            return DownloadFile(localFile, remoteFile, AdsDirectory.PATH_BOOTPATH);
        }

        public bool DownloadFile(string localFile, string remoteFile, AdsDirectory standardDirectory = AdsDirectory.PATH_GENERIC)
        {
            AdsFileSystem ctrl = new AdsFileSystem();

            if (!ctrl.Connect(target, (int)TwinCAT.Ads.AmsPort.SystemService))
                return false;

            ushort handle = ctrl.OpenFile(remoteFile, standardDirectory, (UInt32)AdsFileOpenMode.FOPEN_MODEWRITE | (UInt32)AdsFileOpenMode.FOPEN_MODEBINARY);

            if (handle <= 0)
                return false;

            FileStream fs = File.OpenRead(localFile);
            BinaryReader br = new BinaryReader(fs, System.Text.Encoding.Default);

            FileInfo i = new FileInfo(localFile);

            long totalSize = 0;
            bool successWrite = true;
            // use ReadByte instead of PeakChar because PeakChar sometimes fails on non UTF-8 files
            while (br.BaseStream.ReadByte() != -1)
            {
                br.BaseStream.Position -= 1;
                
                var buffer = new byte[DefaultChunkSize];
                var readCount = default(int);

                readCount = br.Read(buffer, 0, DefaultChunkSize);
                successWrite = ctrl.WriteFile(handle, buffer, (uint)readCount);

                if (successWrite)
                    totalSize += (long)readCount;
                else
                    break;
            }

            bool succClose = ctrl.CloseFile(handle);
            ctrl.Dispose();

            return successWrite && succClose && (totalSize == i.Length);
        }

        public bool UploadFileFromBootFolder(string localFile, string remoteFile)
        {
            return UploadFile(localFile, remoteFile, AdsDirectory.PATH_BOOTPATH);
        }

        public bool UploadFileFromBootFolder(Stream stream, string remoteFile)
        {
            return UploadFile(stream, remoteFile, AdsDirectory.PATH_BOOTPATH);
        }

        private bool UploadFile(Stream stream, string remoteFile, AdsDirectory standardDirectory = AdsDirectory.PATH_GENERIC)
        {
            AdsFileSystem ctrl = new AdsFileSystem();

            if (!ctrl.Connect(target, (int)AmsPort.SystemService))
                return false;

            ushort handle = ctrl.OpenFile(remoteFile, standardDirectory, (UInt32)AdsFileOpenMode.FOPEN_MODEREAD | (UInt32)AdsFileOpenMode.FOPEN_MODEBINARY);

            if (handle <= 0)
                return false;

            long totalSize = 0;
            bool eof = false;
            do
            {
                byte[] buffer = ctrl.ReadFile(handle, (uint)DefaultChunkSize, out eof);
                if (buffer != null)
                {
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                    totalSize += buffer.Length;
                }
            }
            while (!eof);

            stream.Seek(0L, SeekOrigin.Begin);
            stream.Flush();

            ctrl.CloseFile(handle);

            return true;
        }

        public bool UploadFile(string localFile, string remoteFile, AdsDirectory standardDirectory = AdsDirectory.PATH_GENERIC)
        {
            FileStream fs = File.OpenWrite(localFile);
            return UploadFile(fs, remoteFile, standardDirectory);
        }

        public bool CleanUpBootDirectory(string osName)
        {
            string bootfolder = Common.RTOperatingSystem.GetBootProjFolderByOsName(osName);

            var ret = DeleteDirectoryContentRecursive(bootfolder, osName);

            return ret;
        }

        public bool DeleteDirectoryContentRecursive(string path, string osName)
        {
            string seperator = Common.RTOperatingSystem.GetSeperatorByOsName(osName);

            int port = (int)TwinCAT.Ads.AmsPort.SystemService;
            bool return_value = true;

            List<AdsFileSystemEntry> files = AdsFileSystem.EnumerateFiles(target,
                                                                        port,
                                                                        GetPathWithWildcard(path, seperator),
                                                                        AdsFileSystemCommandType.eEnumCmd_First);

            foreach (AdsFileSystemEntry f in files)
            {
                string filepath = path + seperator + f.FileName;

                var ret = true;
                if (!IsValidFilename(f.FileName))
                    ;
                else if (f.fileAttributes.Directory)
                {
                    ret = DeleteDirectoryContentRecursive(filepath, seperator);
                    // for some reason deleting the Plc folder fails, so skip it
                    if (f.FileName != "Plc")
                        ret = AdsFileSystem.RemoveDirectory(target, port, filepath, AdsDirectory.PATH_GENERIC);
                }
                else
                {
                    ret = AdsFileSystem.DeleteFile(target, port, filepath, AdsDirectory.PATH_GENERIC);
                }
                return_value &= ret;
            }

            return return_value;
        }

        private string MakeSurePathEndsWithSeperator(string path, string seperator)
        {
            if (!path.EndsWith(seperator))
                path += seperator;
            return path;
        }

        private string GetPathWithWildcard(string path, string seperator)
        {
            return MakeSurePathEndsWithSeperator(path, seperator) + "*.*";
        }

        private bool IsValidFilename(string filename)
        {
            if (filename == ".")
                return false;
            if (filename == "..")
                return false;
            return true;
        }
    }
}
