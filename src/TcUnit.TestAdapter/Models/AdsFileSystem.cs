using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using static TcUnit.TestAdapter.Models.AdsFileSystemTypes;
using TwinCAT.Ads;

namespace TcUnit.TestAdapter.Models
{
    public class AdsFileSystem
    {
        private readonly AdsClient adsClient;

        public AdsFileSystem()
        {
            adsClient = new AdsClient();
        }

        public bool Connect(string NetID, int Port)
        {
            if (!adsClient.IsConnected)
                adsClient.Connect(NetID, Port);

            return adsClient.IsConnected;
        }

        public bool Disconnect()
        {
            if (!adsClient.IsConnected)
                return false;
            adsClient.Disconnect();
            return true;
        }

        public bool Dispose()
        {
            if (adsClient.IsDisposed)
                return false;
            if (adsClient.IsConnected)
                Disconnect();
            adsClient.Dispose();
            return true;
        }

        public bool CreateDirectory(string path, AdsDirectory directory)
        {
            if (!adsClient.IsConnected)
                return false;

            bool ret = false;

            byte[] readData = new byte[0];
            byte[] writeData = new byte[path.Length + 1];

            using (MemoryStream writeStream = new MemoryStream(writeData))
            {
                using (BinaryWriter writer = new BinaryWriter(writeStream, Encoding.ASCII)) 
                {
                    writer.Write(path.ToCharArray());
                    writer.Write('\0');

                    try
                    {
                        adsClient.ReadWrite((uint)AdsIndexGroup.SYSTEMSERVICE_MKDIR, (uint)directory, readData.AsMemory(), writeData.AsMemory());
                        ret = true;
                    }
                    catch (AdsErrorException)
                    {
                        ret = false;
                    }
                    return ret;

                }
            }
        }


        public bool RemoveDirectory(string path, AdsDirectory directory)
        {
            if (!adsClient.IsConnected)
                return false;

            bool ret = false;

            byte[] readData = new byte[0];
            byte[] writeData = new byte[path.Length + 1];

            using (MemoryStream writeStream = new MemoryStream(writeData))
            {
                using (BinaryWriter writer = new BinaryWriter(writeStream, Encoding.ASCII))
                {
                    writer.Write(path.ToCharArray());
                    writer.Write('\0');

                    try
                    {
                        var value = adsClient.ReadWrite((int)AdsIndexGroup.SYSTEMSERVICE_RMDIR, (uint)directory, readData.AsMemory(), writeData.AsMemory());
                        ret = true;
                    }
                    catch (AdsErrorException)
                    {
                        ret = false;
                    }
                    return ret;
                }
            }
        }


        public bool CloseFile(ushort fileHandle)
        {
            if (!adsClient.IsConnected)
                return false;

            bool ret = false;

            byte[] readData = new byte[0];
            byte[] writeData = new byte[0];

            try
            {
                adsClient.ReadWrite((int)AdsIndexGroup.SYSTEMSERVICE_FCLOSE, fileHandle, readData.AsMemory(), writeData.AsMemory());
                ret = true;
            }
            catch (AdsErrorException)
            {
                ret = false;
            }
            return ret;
        }

        public bool DeleteFile(string path, AdsDirectory directory)
        {
            if (!adsClient.IsConnected)
                return false;

            bool ret = false;

            byte[] readData = new byte[0];
            byte[] writeData = new byte[path.Length + 1];

            using (MemoryStream writeStream = new MemoryStream(writeData))
            {
                using (BinaryWriter writer = new BinaryWriter(writeStream, Encoding.ASCII))
                {
                    writer.Write(path.ToCharArray());
                    writer.Write('\0');

                    try
                    {
                        adsClient.ReadWrite((int)AdsIndexGroup.SYSTEMSERVICE_FDELETE, (uint)directory << 16, readData.AsMemory(), writeData.AsMemory());
                        ret = true;
                    }
                    catch (AdsErrorException)
                    {
                        ret = false;
                    }
                    return ret;
                }
            }
        }

        public ushort OpenFile(string path, AdsDirectory directory, uint mode)
        {
            if (!adsClient.IsConnected)
                return 0;

            ushort ret = 0;
            int numReadBytes = 0;

            byte[] readData = new byte[sizeof(uint)];
            byte[] writeData = new byte[path.Length + 1];

            using (MemoryStream writeStream = new MemoryStream(writeData))
            {
                using (BinaryWriter writer = new BinaryWriter(writeStream, Encoding.ASCII))
                {
                    writer.Write(path.ToCharArray());
                    writer.Write('\0');

                    try
                    {
                        numReadBytes = adsClient.ReadWrite((int)AdsIndexGroup.SYSTEMSERVICE_FOPEN, (uint)directory << 16 | mode, readData.AsMemory(), writeData.AsMemory());
                        if (numReadBytes >= sizeof(uint))
                        {
                            using (MemoryStream readStream = new MemoryStream(readData))
                            {
                                using (BinaryReader reader = new BinaryReader(readStream, Encoding.Default))
                                {
                                    ret = (ushort)reader.ReadUInt32();
                                }
                            }
                        }
                        else
                        {
                            ret = 0;
                        }
                    }
                    catch (AdsErrorException)
                    {
                        ret = 0;
                    }
                    return ret;
                }
            }
        }

        public byte[] ReadFile(ushort fileHandle, uint readLength, out bool isEndOfFile)
        {
            if (!adsClient.IsConnected)
            {
                isEndOfFile = false;
                return null;
            }

            byte[] ret;
            isEndOfFile = false;
            int numReadBytes = 0;

            byte[] readData = new byte[readLength];
            byte[] writeData = new byte[0];

            try
            {
                numReadBytes = adsClient.ReadWrite((int)AdsIndexGroup.SYSTEMSERVICE_FREAD, fileHandle, readData.AsMemory(), writeData.AsMemory());
                if (numReadBytes > 0)
                {
                    using (MemoryStream readStream = new MemoryStream(readData))
                    {
                        using (BinaryReader reader = new BinaryReader(readStream, Encoding.Default))
                        {
                            ret = reader.ReadBytes(numReadBytes);
                        }
                    }
                }
                else
                {
                    isEndOfFile = true;
                    ret = null;
                }
            }
            catch (AdsErrorException)
            {
                ret = null;
            }
            return ret;
        }

        public byte[] ReadFile(ushort fileHandle, uint readLength)
        {
            bool dummy;
            return ReadFile(fileHandle, readLength, out dummy);
        }

        public bool RenameFile(string oldName, string newName, AdsDirectory directory)
        {
            if (!adsClient.IsConnected)
                return false;

            bool ret = false;

            byte[] readData = new byte[0];

            byte[] writeData = new byte[oldName.Length + 1 + newName.Length + 1];

            using (MemoryStream writeStream = new MemoryStream(writeData))
            {
                using (BinaryWriter writer = new BinaryWriter(writeStream, Encoding.ASCII))
                {
                    writer.Write(oldName.ToCharArray());
                    writer.Write('\0');
                    writer.Write(newName.ToCharArray());
                    writer.Write('\0');

                    try
                    {
                        if (oldName.Length + 1 + newName.Length + 1 <= 255)
                        {
                            adsClient.ReadWrite((int)AdsIndexGroup.SYSTEMSERVICE_FRENAME, (uint)directory << 16, readData.AsMemory(), writeData.AsMemory());
                            ret = true;
                        }
                        else
                        {
                            ret = false;
                        }
                    }
                    catch (AdsErrorException)
                    {
                        ret = false;
                    }
                    return ret;
                }
            }
        }

        public bool WriteFile(ushort fileHandle, byte[] writeBuffer, uint writeLength)
        {
            if (!adsClient.IsConnected)
                return false;

            bool ret = false;

            byte[] readData = new byte[0];

            try
            {
                adsClient.ReadWrite((int)AdsIndexGroup.SYSTEMSERVICE_FWRITE, fileHandle, readData.AsMemory(), writeBuffer.AsMemory(0, (int)writeLength));
                ret = true;
            }
            catch (AdsErrorException)
            {
                ret = false;
            }
            return ret;
        }

        public AdsFindFileSystemEntryResult FindFileSystemEntry(string path, AdsFileSystemCommandType command, ushort previousHandle)
        {
            if (!adsClient.IsConnected)
                return new AdsFindFileSystemEntryResult();

            AdsFindFileSystemEntryResult stFindFileResult = new AdsFindFileSystemEntryResult();
            AmsFileSystemEntry dataEntry = new AmsFileSystemEntry();

            dataEntry.File = previousHandle;


            if (command == AdsFileSystemCommandType.eEnumCmd_First && dataEntry.File != 0 || command == AdsFileSystemCommandType.eEnumCmd_Abort)
            {
                try
                {
                    adsClient.WriteAny((int)AdsIndexGroup.SYSTEMSERVICE_CLOSEHANDLE, 0, dataEntry.File);
                    dataEntry.File = 0;
                }
                catch (AdsErrorException err)
                {
                    stFindFileResult.Error = true;
                    stFindFileResult.ErrorID = (uint)err.ErrorCode;
                    return stFindFileResult;
                }
            }

            if (command == AdsFileSystemCommandType.eEnumCmd_First && dataEntry.File == 0 || command == AdsFileSystemCommandType.eEnumCmd_Next && dataEntry.File != 0)
            {
                try
                {
                    byte[] readData = new byte[Marshal.SizeOf(dataEntry)];

                    if (command == AdsFileSystemCommandType.eEnumCmd_First)
                    {
                        byte[] writeData = new byte[path.Length + 1];

                        using (MemoryStream writeStream = new MemoryStream(writeData))
                        {
                            using (BinaryWriter writer = new BinaryWriter(writeStream, Encoding.ASCII))
                            {
                                writer.Write(path.ToCharArray());
                                writer.Write('\0');

                                adsClient.ReadWrite((int)AdsIndexGroup.SYSTEMSERVICE_FFILEFIND, (int)AdsDirectory.PATH_GENERIC, readData.AsMemory(), writeData.AsMemory());
                            }
                        }
                    }
                    else
                    {
                        byte[] writeData = new byte[0];
                        adsClient.ReadWrite((int)AdsIndexGroup.SYSTEMSERVICE_FFILEFIND, dataEntry.File, readData.AsMemory(), writeData.AsMemory());
                    }

                    dataEntry = (AmsFileSystemEntry)ByteArrayToStruct(readData, typeof(AmsFileSystemEntry));

                    stFindFileResult.FileEntry.fileAttributes.ReadOnly = (dataEntry.FileAttributes & 1) != 0;
                    stFindFileResult.FileEntry.fileAttributes.Hidden = (dataEntry.FileAttributes & 2) != 0;
                    stFindFileResult.FileEntry.fileAttributes.System = (dataEntry.FileAttributes & 4) != 0;
                    stFindFileResult.FileEntry.fileAttributes.Directory = (dataEntry.FileAttributes & 16) != 0;
                    stFindFileResult.FileEntry.fileAttributes.Archive = (dataEntry.FileAttributes & 32) != 0;
                    stFindFileResult.FileEntry.fileAttributes.Device = (dataEntry.FileAttributes & 64) != 0;
                    stFindFileResult.FileEntry.fileAttributes.Normal = (dataEntry.FileAttributes & 128) != 0;
                    stFindFileResult.FileEntry.fileAttributes.Temporary = (dataEntry.FileAttributes & 256) != 0;
                    stFindFileResult.FileEntry.fileAttributes.SparseFile = (dataEntry.FileAttributes & 512) != 0;
                    stFindFileResult.FileEntry.fileAttributes.ReparsePoint = (dataEntry.FileAttributes & 1024) != 0;
                    stFindFileResult.FileEntry.fileAttributes.Compressed = (dataEntry.FileAttributes & 2048) != 0;
                    stFindFileResult.FileEntry.fileAttributes.Offline = (dataEntry.FileAttributes & 4096) != 0;
                    stFindFileResult.FileEntry.fileAttributes.NotContentIndexed = (dataEntry.FileAttributes & 8192) != 0;
                    stFindFileResult.FileEntry.fileAttributes.Encrypted = (dataEntry.FileAttributes & 16384) != 0;

                    stFindFileResult.FileEntry.CreationTime = dataEntry.CreationTime;
                    stFindFileResult.FileEntry.LastAccessTime = dataEntry.LastAccessTime;
                    stFindFileResult.FileEntry.LastWriteTime = dataEntry.LastWriteTime;
                    stFindFileResult.FileEntry.FileSize = (dataEntry.FileSizeHigh << 32) + dataEntry.FileSizeLow;
                    stFindFileResult.FileEntry.FileName = dataEntry.FileName;
                    stFindFileResult.FileEntry.AlternateFileName = dataEntry.AlternateFileName;
                    stFindFileResult.File = dataEntry.File;
                }
                catch (AdsErrorException err)
                {
                    if ((int)err.ErrorCode == 1804)
                    {
                        stFindFileResult.EOF = true;
                    }
                    else
                    {
                        stFindFileResult.Error = true;
                        stFindFileResult.ErrorID = (uint)err.ErrorCode;
                        return stFindFileResult;
                    }
                }
            }
            return stFindFileResult;
        }

        public List<AdsFileSystemEntry> EnumerateFiles(string sPathName, AdsFileSystemCommandType eCmd)
        {
            if (!adsClient.IsConnected)
                return null;

            List<AdsFileSystemEntry> fileList = new List<AdsFileSystemEntry>();
            AdsFindFileSystemEntryResult Entry = new AdsFindFileSystemEntryResult();

            while (!Entry.EOF && !Entry.Error)
            {
                if (fileList.Count == 0)
                {
                    Entry = FindFileSystemEntry(sPathName, eCmd, 0);
                }
                else
                {
                    Entry = FindFileSystemEntry(sPathName, AdsFileSystemCommandType.eEnumCmd_Next, Entry.File);
                }

                if (!Entry.EOF)
                {
                    fileList.Add(Entry.FileEntry);
                }
            }
            return fileList;
        }

        public static object ByteArrayToStruct(byte[] array, Type structType)
        {
            int offset = 0;
            if (structType.StructLayoutAttribute.Value != LayoutKind.Sequential)
                throw new ArgumentException("structType ist keine Struktur oder nicht Sequentiell.");

            int size = Marshal.SizeOf(structType);
            if (array.Length < offset + size)
                throw new ArgumentException("Byte-Array hat die falsche Länge.");

            byte[] tmp = new byte[size];
            Array.Copy(array, offset, tmp, 0, size);

            GCHandle structHandle = GCHandle.Alloc(tmp, GCHandleType.Pinned);
            object structure = Marshal.PtrToStructure(structHandle.AddrOfPinnedObject(), structType);
            structHandle.Free();

            return structure;
        }

        public static bool CreateDirectory(string NetID, int Port, string sPathName, AdsDirectory ePath)
        {
            AdsFileSystem ctrl = new AdsFileSystem();
            if (!ctrl.Connect(NetID, Port))
                return false;

            bool ret = ctrl.CreateDirectory(sPathName, ePath);

            ctrl.Dispose();

            return ret;
        }


        public static bool CloseFile(string NetID, int Port, ushort nFileHdl)
        {
            AdsFileSystem ctrl = new AdsFileSystem();
            if (!ctrl.Connect(NetID, Port))
                return false;

            bool ret = ctrl.CloseFile(nFileHdl);

            ctrl.Dispose();

            return ret;
        }

        public static bool DeleteFile(string NetID, int Port, string sPathName, AdsDirectory ePath)
        {
            AdsFileSystem ctrl = new AdsFileSystem();
            if (!ctrl.Connect(NetID, Port))
                return false;

            bool ret = ctrl.DeleteFile(sPathName, ePath);

            ctrl.Dispose();

            return ret;
        }

        public static ushort OpenFile(string NetID, int Port, string sPathName, AdsDirectory ePath, uint nMode)
        {
            AdsFileSystem ctrl = new AdsFileSystem();
            if (!ctrl.Connect(NetID, Port))
                return 0;

            ushort ret = ctrl.OpenFile(sPathName, ePath, nMode);

            ctrl.Dispose();

            return ret;
        }


        public static byte[] ReadFile(string NetID, int Port, ushort nFileHdl, uint nReadLen, out bool bEOF)
        {
            bEOF = false;

            AdsFileSystem ctrl = new AdsFileSystem();
            if (!ctrl.Connect(NetID, Port))
                return null;

            byte[] ret = ctrl.ReadFile(nFileHdl, nReadLen, out bEOF);

            ctrl.Dispose();

            return ret;
        }

        public static byte[] ReadFile(string NetID, int Port, ushort nFileHdl, uint nReadLen)
        {
            bool dummy;

            return ReadFile(NetID, Port, nFileHdl, nReadLen, out dummy);
        }

        public static bool RenameFile(string NetID, int Port, string sOldName, string sNewName, AdsDirectory ePath)
        {
            AdsFileSystem ctrl = new AdsFileSystem();
            if (!ctrl.Connect(NetID, Port))
                return false;

            bool ret = ctrl.RenameFile(sOldName, sNewName, ePath);

            ctrl.Dispose();

            return ret;
        }

        public static bool WriteFile(string NetID, int Port, ushort nFileHdl, byte[] aWriteBuff, uint nWriteLen)
        {
            AdsFileSystem ctrl = new AdsFileSystem();
            if (!ctrl.Connect(NetID, Port))
                return false;

            bool ret = ctrl.WriteFile(nFileHdl, aWriteBuff, nWriteLen);

            ctrl.Dispose();

            return ret;
        }

        public static bool RemoveDirectory(string NetID, int Port, string sPathName, AdsDirectory ePath)
        {
            AdsFileSystem ctrl = new AdsFileSystem();
            if (!ctrl.Connect(NetID, Port))
                return false;

            bool ret = ctrl.RemoveDirectory(sPathName, ePath);

            ctrl.Dispose();

            return ret;
        }

        public static AdsFindFileSystemEntryResult FindFileSystemEntry(string NetID, int Port, string sPathName, AdsFileSystemCommandType eCmd, ushort PreviousFhandle)
        {
            AdsFileSystem ctrl = new AdsFileSystem();
            if (!ctrl.Connect(NetID, Port))
                return new AdsFindFileSystemEntryResult();

            AdsFindFileSystemEntryResult ret = ctrl.FindFileSystemEntry(sPathName, eCmd, PreviousFhandle);

            ctrl.Dispose();

            return ret;
        }

        public static List<AdsFileSystemEntry> EnumerateFiles(string NetID, int Port, string sPathName, AdsFileSystemCommandType eCmd)
        {
            AdsFileSystem ctrl = new AdsFileSystem();
            if (!ctrl.Connect(NetID, Port))
                return null;

            List<AdsFileSystemEntry> ret = ctrl.EnumerateFiles(sPathName, eCmd);

            ctrl.Dispose();

            return ret;
        }
    }
}
