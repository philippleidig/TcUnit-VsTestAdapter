using System;
using System.Collections.Generic;
using System.IO;
using static Beckhoff.App.TcHelper.AdsFileCtrl.Types;
using System.Threading;
using TwinCAT.Ads;

namespace Beckhoff.App.TcHelper.AdsFileCtrl
{
    public class AdsFileCtrlExt
    {
        public static readonly int DefaultChunksize = 10000;

        /// <summary>
        ///  Use a BinaryReader to open a local File bytewise, and make a FB_FileWrite to stream it to the remote system
        ///  Will overwrite an existing file with the same name
        /// </summary>
        /// <param name="amsNetID">NetID of the remote system</param>
        /// <param name="localFile">Local file to be opened and copied</param>
        /// <param name="remoteFile">Remote file to be created. Path needs to exist!</param>
        /// <param name="chunksize">How big do we want our chunks of data on the ads line</param>
        /// <returns></returns>
        public static bool DownloadFileToTarget(string amsNetID, string localFile, string remoteFile, int chunksize)
        {
            AdsFileCtrl ctrl = new AdsFileCtrl();

            if (!ctrl.Connect(amsNetID, (int)TwinCAT.Ads.AmsPort.SystemService))
                return false;

            ushort handle = ctrl.FB_FileOpen(remoteFile, E_OpenPath.PATH_GENERIC, (UInt32)E_FopenMode.FOPEN_MODEWRITE | (UInt32)E_FopenMode.FOPEN_MODEBINARY);

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
                successWrite = ctrl.FB_FileWrite(handle, buffer, (uint)readCount);

                if (successWrite)
                    totalSize += (long)readCount;
                else
                    break;
            }

            bool succClose = ctrl.FB_FileClose(handle);
            ctrl.Dispose();

            return successWrite && succClose && (totalSize == i.Length);
        }

        /// <summary>
        ///  Use a BinaryReader to open a local File bytewise, and make a FB_FileWrite to stream it to the remote system
        ///  Will overwrite an existing file with the same name
        ///  Uses the DefaultChunksize
        /// </summary>
        /// <param name="amsNetID">NetID of the remote system</param>
        /// <param name="localFile">Local file to be opened and copied</param>
        /// <param name="remoteFile">Remote file to be created. Path needs to exist!</param>
        /// <returns></returns>
        public static bool DownloadFileToTarget(string amsNetID, string localFile, string remoteFile)
        {
            return DownloadFileToTarget(amsNetID, localFile, remoteFile, DefaultChunksize);
        }

        /// <summary>
        ///  Use a BinaryWriter to create a local File, and make a FB_FileRead to stream it from the remote system
        ///  Will overwrite an existing file with the same name
        /// </summary>
        /// <param name="amsNetID">NetID of the remote system</param>
        /// <param name="localFile">Local file to be created. Path needs to exist!</param>
        /// <param name="remoteFile">Remote file to be opened and copied</param>
        /// <param name="chunksize">How big do we want our chunks of data on the ads line</param>
        /// <returns></returns>
        public static bool UploadFileFromTarget(string amsNetID, string localFile, string remoteFile, int chunksize)
        {
            AdsFileCtrl ctrl = new AdsFileCtrl();

            if (!ctrl.Connect(amsNetID, (int)TwinCAT.Ads.AmsPort.SystemService))
                return false;

            ushort handle = ctrl.FB_FileOpen(remoteFile, E_OpenPath.PATH_GENERIC, (UInt32)E_FopenMode.FOPEN_MODEREAD | (UInt32)E_FopenMode.FOPEN_MODEBINARY);

            if (handle <= 0)
                return false;

            FileStream fs = File.OpenWrite(localFile);
            BinaryWriter bw = new BinaryWriter(fs, System.Text.Encoding.Default);
            long totalSize = 0;
            bool eof = false;
            do
            {
                byte[] buffer = ctrl.FB_FileRead(handle, (uint)chunksize, out eof);
                if (buffer != null)
                {
                    bw.Write(buffer, 0, buffer.Length);
                    totalSize += buffer.Length;
                }
            }
            while (!eof);

            ctrl.FB_FileClose(handle);
            fs.Flush(true);
            fs.Close();

            return true;
        }

        /// <summary>
        ///  Use a BinaryWriter to create a local File, and make a FB_FileRead to stream it from the remote system
        ///  Will overwrite an existing file with the same name
        ///  Uses the DefaultChunksize
        /// </summary>
        /// <param name="amsNetID">NetID of the remote system</param>
        /// <param name="localFile">Local file to be created. Path needs to exist!</param>
        /// <param name="remoteFile">Remote file to be opened and copied</param>
        /// <returns></returns>
        public static bool UploadFileFromTarget(string amsNetID, string localFile, string remoteFile)
        {
            return UploadFileFromTarget(amsNetID, localFile, remoteFile, DefaultChunksize);
        }

        /// <summary>
        /// we connect to both the target systems, open 2 file handles, 1 reading, 1 writing
        /// and transfer a file from yource to sink
        /// </summary>
        /// <param name="amsNetIdSource">NetID of the source remote system</param>
        /// <param name="filePathSource">File to be opened and transferred</param>
        /// <param name="amsNetIdSink">NetID of the sink remote system</param>
        /// <param name="filePathSink">File to be created</param>
        /// <param name="chunksize">How big do we want our chunks of data on the ads line</param>
        /// <returns>success</returns>
        public static bool TransferFileFromSourceToSink(string amsNetIdSource, string filePathSource, string amsNetIdSink, string filePathSink, int chunksize)
        {
            AdsFileCtrl ctrlSource = new AdsFileCtrl();

            if (!ctrlSource.Connect(amsNetIdSource, (int)TwinCAT.Ads.AmsPort.SystemService))
                return false;

            ushort handleSource = ctrlSource.FB_FileOpen(filePathSource, E_OpenPath.PATH_GENERIC, (UInt32)E_FopenMode.FOPEN_MODEREAD | (UInt32)E_FopenMode.FOPEN_MODEBINARY);

            if (handleSource <= 0)
                return false;

            AdsFileCtrl ctrlSink = new AdsFileCtrl();

            if (!ctrlSink.Connect(amsNetIdSink, (int)TwinCAT.Ads.AmsPort.SystemService))
                return false;

            ushort handleSink = ctrlSink.FB_FileOpen(filePathSink, E_OpenPath.PATH_GENERIC, (UInt32)E_FopenMode.FOPEN_MODEWRITE | (UInt32)E_FopenMode.FOPEN_MODEBINARY);

            if (handleSink <= 0)
                return false;

            long totalSize = 0;
            bool eof = false;
            do
            {
                byte[] buffer = ctrlSource.FB_FileRead(handleSource, (uint)chunksize, out eof);
                if (buffer != null)
                {
                    var successWrite = ctrlSink.FB_FileWrite(handleSink, buffer, (uint)buffer.Length);
                    if (successWrite)
                        totalSize += buffer.Length;
                }
            }
            while (!eof);

            ctrlSource.FB_FileClose(handleSource);
            ctrlSink.FB_FileClose(handleSink);

            return true;
        }

        /// <summary>
        /// we connect to both the target systems, open 2 file handles, 1 reading, 1 writing
        /// and transfer a file from yource to sink
        /// </summary>
        /// <param name="amsNetIdSource">NetID of the source remote system</param>
        /// <param name="filePathSource">File to be opened and transferred</param>
        /// <param name="amsNetIdSink">NetID of the sink remote system</param>
        /// <param name="filePathSink">File to be created</param>
        /// <returns>success</returns>
        public static bool TransferFileFromSourceToSink(string amsNetIdSource, string filePathSource, string amsNetIdSink, string filePathSink)
        {
            return TransferFileFromSourceToSink(amsNetIdSource, filePathSource, amsNetIdSink, filePathSink, DefaultChunksize);
        }

        /// <summary>
        /// transfer an (executable) file to the remote system and run it
        /// a folder will be created with a random guid as name
        /// </summary>
        /// <param name="amsNetID">NetID of the remote system</param>
        /// <param name="localFile">File to be opened and transferred</param>
        /// <param name="osName">Name of the remote operating system</param>
        /// <param name="chunksize">How big do we want our chunks of data on the ads line</param>
        /// <returns>success</returns>
        public static bool UploadFileAndRunOnTarget(string amsNetID, string localFile, string osName, int chunksize)
        {
            string seperator = OsHelper.getSeperatorByOsName(osName);
            string entryPoint = OsHelper.getEntryPointByOsName(osName);

            var name = GetFileFolderName(localFile, OsHelper.getEngineeringSeperator());

            var path = entryPoint + seperator + Guid.NewGuid();
            var remoteFile = path + seperator + name;

            if (!AdsFileCtrl.FB_CreateDir(amsNetID, (int)TwinCAT.Ads.AmsPort.SystemService, path, E_OpenPath.PATH_GENERIC))
                return false;

            if (!DownloadFileToTarget(amsNetID, localFile, remoteFile, chunksize))
                return false;

            if (!AdsFileCtrl.NT_StartProcess(amsNetID, (int)TwinCAT.Ads.AmsPort.SystemService, remoteFile, path, ""))
                return false;

            return true;
        }

        /// <summary>
        /// transfer an (executable) file to the remote system and run it
        /// file and containing folder will be destroyed afterwards
        /// </summary>
        /// <param name="amsNetID">NetID of the remote system</param>
        /// <param name="localFile">File to be opened and transferred</param>
        /// <param name="osName">Name of the remote operating system</param>
        /// <param name="chunksize">How big do we want our chunks of data on the ads line</param>
        /// <param name="sleeptimeb4delete">How long do we wait between execution and deletion</param>
        /// <returns>success</returns>
        public static bool UploadFileRunOnTargetAndDelete(string amsNetID, string localFile, string osName, int chunksize, int sleeptimeb4delete)
        {
            string seperator = OsHelper.getSeperatorByOsName(osName);
            string entryPoint = OsHelper.getEntryPointByOsName(osName);

            var name = GetFileFolderName(localFile, OsHelper.getEngineeringSeperator());

            var path = entryPoint + seperator + Guid.NewGuid();
            var remoteFile = path + seperator + name;

            if (!AdsFileCtrl.FB_CreateDir(amsNetID, (int)TwinCAT.Ads.AmsPort.SystemService, path, E_OpenPath.PATH_GENERIC))
                return false;

            if (!DownloadFileToTarget(amsNetID, localFile, remoteFile, chunksize))
                return false;

            if (!AdsFileCtrl.NT_StartProcess(amsNetID, (int)TwinCAT.Ads.AmsPort.SystemService, remoteFile, path, ""))
                return false;

            Thread.Sleep(sleeptimeb4delete);

            if (!AdsFileCtrl.FB_FileDelete(amsNetID, (int)TwinCAT.Ads.AmsPort.SystemService, remoteFile, E_OpenPath.PATH_GENERIC))
                return false;

            if (!AdsFileCtrl.FB_RemoveDir(amsNetID, (int)TwinCAT.Ads.AmsPort.SystemService, path, E_OpenPath.PATH_GENERIC))
                return false;

            return true;
        }

        /// <summary>
        /// Copy all folder content from a local folder recusively to a remote folder
        /// </summary>
        /// <param name="amsNetID">NetID of the remote system</param>
        /// <param name="localPath">Path of the local folder to be copied</param>
        /// <param name="remotePath">Remote folder path, where data is copied to</param>
        /// <param name="osName">Name of the remote operating system</param>
        /// <param name="chunksize">How big do we want our chunks of data on the ads line</param>
        /// <returns></returns>
        public static bool DownloadFolderContentToTarget(string amsNetID, string localPath, string remotePath, string osName, int chunksize)
        {
            int port = (int)TwinCAT.Ads.AmsPort.SystemService;
            string seperator = OsHelper.getSeperatorByOsName(osName);

            DeleteRemoteDirectoryContent(amsNetID, remotePath, seperator);

            bool return_value = true;

            List<ST_FindFileEntry> files = AdsFileCtrl.FB_EnumFindFileList(AmsNetId.Local.ToString(),
                                                                        port,
                                                                        GetPathWithWildcard(localPath, seperator),
                                                                        E_EnumCmdType.eEnumCmd_First);

            foreach (ST_FindFileEntry f in files)
            {
                var ret = false;
                string localFile = localPath + OsHelper.getEngineeringSeperator() + f.sFileName;
                string remoteFile = remotePath + seperator + f.sFileName;
                if (!IsValidFilename(f.sFileName))
                    ;
                else if (f.fileAttributes.bDirectory)
                {
                    AdsFileCtrl.FB_CreateDir(amsNetID, port, remoteFile, E_OpenPath.PATH_GENERIC);
                    ret = DownloadFolderContentToTarget(amsNetID, localFile, remoteFile, osName, chunksize);
                }
                else
                {
                    ret = DownloadFileToTarget(amsNetID, localFile, remoteFile, chunksize);
                }
                return_value &= ret;
            }

            return return_value;
        }

        /// <summary>
        /// Copy all folder content from a local folder recusively to a remote folder
        /// </summary>
        /// <param name="amsNetID">NetID of the remote system</param>
        /// <param name="localPath">Path of the local folder to be copied</param>
        /// <param name="remotePath">Remote folder path, where data is copied to</param>
        /// <param name="osName">Name of the remote operating system</param>
        /// <returns></returns>
        public static bool DownloadFolderContentToTarget(string amsNetID, string localPath, string remotePath, string osName)
        {
            return DownloadFolderContentToTarget(amsNetID, localPath, remotePath, osName, DefaultChunksize);
        }

        /// <summary>
        /// Copy all folder content from a local folder recusively to a remote folder
        /// </summary>
        /// <param name="amsNetIDSource">NetID of the source remote system</param>
        /// <param name="pathSource">Path of the local folder to be copied</param>
        /// <param name="amsNetIDSink">NetID of the sink remote system</param>
        /// <param name="pathSink">Path of the remote folder where data is copied</param>
        /// <param name="osNameSource">Name of the source operating system</param>
        /// <param name="osNameSink">Name of the sink operating system</param>
        /// <param name="chunksize">How big do we want our chunks of data on the ads line</param>
        /// <returns>success</returns>
        public static bool DownloadFolderContentToTarget(string amsNetIDSource, string pathSource, string amsNetIDSink, string pathSink, string osNameSource, string osNameSink, int chunksize)
        {
            bool return_value = true;

            int port = (int)TwinCAT.Ads.AmsPort.SystemService;
            string seperatorSource = OsHelper.getSeperatorByOsName(osNameSource);
            string seperatorSink = OsHelper.getSeperatorByOsName(osNameSink);

            return_value &= DeleteRemoteDirectoryContent(amsNetIDSink, pathSink, seperatorSink);


            List<ST_FindFileEntry> files = AdsFileCtrl.FB_EnumFindFileList(AmsNetId.Local.ToString(),
                                                                        port,
                                                                        GetPathWithWildcard(pathSource, seperatorSource),
                                                                        E_EnumCmdType.eEnumCmd_First);

            foreach (ST_FindFileEntry f in files)
            {
                var ret = false;
                string sourceFile = pathSource + seperatorSource + f.sFileName;
                string sinkFile = pathSink + seperatorSink + f.sFileName;
                if (!IsValidFilename(f.sFileName))
                    ;
                else if (f.fileAttributes.bDirectory)
                {
                    AdsFileCtrl.FB_CreateDir(amsNetIDSink, port, sinkFile, E_OpenPath.PATH_GENERIC);
                    ret = DownloadFolderContentToTarget(amsNetIDSource, sourceFile, amsNetIDSink, sinkFile, osNameSource, osNameSink, chunksize);
                }
                else
                {
                    ret = TransferFileFromSourceToSink(amsNetIDSource, sourceFile, amsNetIDSink, sinkFile, chunksize);
                }
                return_value &= ret;
            }

            return return_value;
        }

        /// <summary>
        /// Copy all folder content from a local folder recusively to a remote folder
        /// </summary>
        /// <param name="amsNetIDSource">NetID of the source remote system</param>
        /// <param name="pathSource">Path of the local folder to be copied</param>
        /// <param name="amsNetIDSink">NetID of the sink remote system</param>
        /// <param name="pathSink">Path of the remote folder where data is copied</param>
        /// <param name="osNameSource">Name of the source operating system</param>
        /// <param name="osNameSink">Name of the sink operating system</param>
        /// <returns>success</returns>
        public static bool DownloadFolderContentToTarget(string amsNetIDSource, string pathSource, string amsNetIDSink, string pathSink, string osNameSource, string osNameSink)
        {
            return DownloadFolderContentToTarget(amsNetIDSource, pathSource, amsNetIDSink, pathSink, osNameSource, osNameSink, DefaultChunksize);
        }

        /// <summary>
        /// Upload all folder content from a remote folder recusively to a local folder
        /// </summary>
        /// <param name="amsNetID">NetID of the remote system</param>
        /// <param name="localPath">Path of the local folder, needs to be empty</param>
        /// <param name="remotePath">Path on the remote system as data source</param>
        /// <param name="osName">Name of the remote operating system</param>
        /// <param name="chunksize">How big do we want our chunks of data on the ads line</param>
        /// <returns>Success</returns>
        public static bool UploadFolderContentFromTarget(string amsNetID, string localPath, string remotePath, string osName, int chunksize)
        {
            bool return_value = true;

            int port = (int)TwinCAT.Ads.AmsPort.SystemService;
            string seperator = OsHelper.getSeperatorByOsName(osName);

            return_value &= DeleteLocalDirectoryContent(localPath);


            List<ST_FindFileEntry> files = AdsFileCtrl.FB_EnumFindFileList(amsNetID,
                                                                        port,
                                                                        GetPathWithWildcard(remotePath, seperator),
                                                                        E_EnumCmdType.eEnumCmd_First);

            foreach (ST_FindFileEntry f in files)
            {
                var ret = false;
                string localFile = localPath + OsHelper.getEngineeringSeperator() + f.sFileName;
                string remoteFile = remotePath + seperator + f.sFileName;
                if (!IsValidFilename(f.sFileName))
                    ;
                else if (f.fileAttributes.bDirectory)
                {
                    AdsFileCtrl.FB_CreateDir(AmsNetId.Local.ToString(), port, localFile, E_OpenPath.PATH_GENERIC);
                    ret = UploadFolderContentFromTarget(amsNetID, localFile, remoteFile, osName, chunksize);
                }
                else
                {
                    ret = UploadFileFromTarget(amsNetID, localFile, remoteFile, chunksize);
                }
                return_value &= ret;
            }

            return return_value;
        }

        /// <summary>
        /// Upload all folder content from a remote folder recusively to a local folder
        /// </summary>
        /// <param name="amsNetID">NetID of the remote system</param>
        /// <param name="localPath">Path of the local folder, needs to be empty</param>
        /// <param name="remotePath">Path on the remote system as data source</param>
        /// <param name="osName">Name of the remote operating system</param>
        /// <returns>Success</returns>
        public static bool UploadFolderContentFromTarget(string amsNetID, string localPath, string remotePath, string osName)
        {
            return UploadFolderContentFromTarget(amsNetID, localPath, remotePath, osName, DefaultChunksize);
        }

        /// <summary>
        /// Delete a remote boot directory
        /// </summary>
        /// <param name="amsNetID">NetID of the remote system</param>
        /// <param name="osName">Name of the remote operating system</param>
        /// <returns>Success</returns>
        public static bool DeleteRemoteBootDirectoryContent(string amsNetID, string osName)
        {
            string bootfolder = OsHelper.getBootProjFolderByOsName(osName);

            var ret = DeleteRemoteDirectoryContent(amsNetID, bootfolder, osName);

            return ret;
        }

        /// <summary>
        /// Delete the content of a remote directory
        /// </summary>
        /// <param name="amsNetID">NetID of the remote system</param>
        /// <param name="path">Path of the remote folder to be emptied</param>
        /// <param name="osName">Name of the remote operating system</param>
        /// <returns>Success</returns>
        public static bool DeleteRemoteDirectoryContent(string amsNetID, string path, string osName)
        {
            string seperator = OsHelper.getSeperatorByOsName(osName);

            int port = (int)TwinCAT.Ads.AmsPort.SystemService;
            bool return_value = true;

            List<ST_FindFileEntry> files = AdsFileCtrl.FB_EnumFindFileList(amsNetID,
                                                                        port,
                                                                        GetPathWithWildcard(path, seperator),
                                                                        E_EnumCmdType.eEnumCmd_First);

            foreach (ST_FindFileEntry f in files)
            {
                string filepath = path + seperator + f.sFileName;
                var ret = true;
                if (!IsValidFilename(f.sFileName))
                    ;
                else if (f.fileAttributes.bDirectory)
                {
                    ret = DeleteRemoteDirectoryContent(amsNetID, filepath, seperator);
                    ret = AdsFileCtrl.FB_RemoveDir(amsNetID, port, filepath, E_OpenPath.PATH_GENERIC);
                }
                else
                {
                    ret = AdsFileCtrl.FB_FileDelete(amsNetID, port, filepath, E_OpenPath.PATH_GENERIC);
                }
                return_value &= ret;
            }

            return return_value;
        }

        private static string AddCntToLocalPath(string path, string cnt = "2")
        {
            var split = path.Split('.');
            if (split.Length < 2)
                return path;

            split[split.Length - 2] += cnt;

            string end = string.Join(".", split);

            return end;
        }

        private static string MakeSurePathEndsWithSeperator(string path, string seperator)
        {
            if (!path.EndsWith(seperator))
                path += seperator;
            return path;
        }

        private static string GetPathWithWildcard(string path, string seperator)
        {
            return MakeSurePathEndsWithSeperator(path, seperator) + "*.*";
        }

        private static bool IsValidFilename(string filename)
        {
            if (filename == ".")
                return false;
            if (filename == "..")
                return false;
            if (filename == "LoggedEvents.db")
                return false;
            return true;
        }

        private static bool DeleteLocalDirectoryContent(string path)
        {
            var files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                FileAttributes attr = File.GetAttributes(file);

                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    Directory.Delete(file, true);
                else
                    File.Delete(file);
            }

            return true;
        }

        private static string GetFileFolderName(string path, string seperator)
        {
            // if we have no path, return empty
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            string normalizedPath = string.Empty;
            int lastindex = 0;

            if (seperator.Contains("/"))
            {
                // BSD style
                normalizedPath = path.Replace('\\', '/');

                // Find the last slash
                lastindex = normalizedPath.LastIndexOf('/');
            }
            else
            {
                // Windows Style
                normalizedPath = path.Replace('/', '\\');

                // Find the last backslash
                lastindex = normalizedPath.LastIndexOf('\\');
            }

            if (lastindex <= 0)
                return path;

            return path.Substring(lastindex + 1);
        }

    }
}
