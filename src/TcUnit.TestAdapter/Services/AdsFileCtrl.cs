using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using static Beckhoff.App.TcHelper.AdsFileCtrl.Types;
using TwinCAT.Ads;

namespace Beckhoff.App.TcHelper.AdsFileCtrl
{
    public class AdsFileCtrl
    {
        private AdsClient tcClientFileCtrl;

        public AdsFileCtrl()
        {
            tcClientFileCtrl = new AdsClient();
        }

        public bool Connect(string NetID, int Port)
        {
            if (!tcClientFileCtrl.IsConnected)
                tcClientFileCtrl.Connect(NetID, Port);

            return tcClientFileCtrl.IsConnected;
        }

        public bool Disconnect()
        {
            if (!tcClientFileCtrl.IsConnected)
                return false;
            tcClientFileCtrl.Disconnect();
            return true;
        }

        public bool Dispose()
        {
            if (tcClientFileCtrl.IsDisposed)
                return false;
            if (tcClientFileCtrl.IsConnected)
                Disconnect();
            tcClientFileCtrl.Dispose();
            return true;
        }

        /// <summary>
        /// FB_CreateDir
        /// Create new directory
        /// </summary>
        /// <param name="sPathName"></param>
        /// <param name="ePath"></param>
        /// <returns>true if succeeded, false if ads error occured</returns>
        public bool FB_CreateDir(string sPathName, E_OpenPath ePath)
        {
            if (!tcClientFileCtrl.IsConnected)
                return false;

            bool ret = false;

            byte[] readData = new byte[0];

            byte[] writeData = new byte[sPathName.Length + 1];
            MemoryStream writeStream = new MemoryStream(writeData);
            BinaryWriter writer = new BinaryWriter(writeStream, System.Text.Encoding.ASCII);
            writer.Write(sPathName.ToCharArray());
            writer.Write('\0'); //add terminating zero

            try
            {
                tcClientFileCtrl.ReadWrite((uint)E_IdxGrp.SYSTEMSERVICE_MKDIR, (uint)ePath, readData.AsMemory(), writeData.AsMemory());
                ret = true;
            }
            catch (AdsErrorException)
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// FB_EOF
        /// Test for end-of-file
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <returns>false if not eof or ads error occured</returns>
        public bool FB_EOF(UInt16 nFileHdl)
        {
            if (!tcClientFileCtrl.IsConnected)
                return false;

            bool bEOF = false;
            UInt32 iEOF = 0;

            byte[] readData = new byte[sizeof(UInt32)];
            byte[] writeData = new byte[0];

            try
            {
                tcClientFileCtrl.ReadWrite((int)E_IdxGrp.SYSTEMSERVICE_FEOF, nFileHdl, readData.AsMemory(), writeData.AsMemory());
                MemoryStream readStream = new MemoryStream(readData);
                BinaryReader reader = new BinaryReader(readStream, System.Text.Encoding.Default);
                iEOF = reader.ReadUInt32();
                bEOF = !(iEOF == 0);
            }
            catch (AdsErrorException)
            {
                bEOF = false;
            }
            return bEOF;
        }

        /// <summary>
        /// FB_FileClose
        /// Close a File
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <returns>true if succeeded, false if ads error occured</returns>
        public bool FB_FileClose(UInt16 nFileHdl)
        {
            if (!tcClientFileCtrl.IsConnected)
                return false;

            bool ret = false;

            byte[] readData = new byte[0];
            byte[] writeData = new byte[0];

            try
            {
                tcClientFileCtrl.ReadWrite((int)E_IdxGrp.SYSTEMSERVICE_FCLOSE, nFileHdl, readData.AsMemory(), writeData.AsMemory());
                ret = true;
            }
            catch (AdsErrorException)
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// FB_FileDelete
        /// Delete a File
        /// </summary>
        /// <param name="sPathName"></param>
        /// <param name="ePath"></param>
        /// <returns>true if succeeded, false if ads error occured</returns>
        public bool FB_FileDelete(string sPathName, E_OpenPath ePath)
        {
            if (!tcClientFileCtrl.IsConnected)
                return false;

            bool ret = false;

            byte[] readData = new byte[0];

            byte[] writeData = new byte[sPathName.Length + 1];
            MemoryStream writeStream = new MemoryStream(writeData);
            BinaryWriter writer = new BinaryWriter(writeStream, System.Text.Encoding.ASCII);
            writer.Write(sPathName.ToCharArray());
            writer.Write('\0'); //add terminating zero

            try
            {
                tcClientFileCtrl.ReadWrite((int)E_IdxGrp.SYSTEMSERVICE_FDELETE, (UInt32)ePath << 16, readData.AsMemory(), writeData.AsMemory());
                ret = true;
            }
            catch (AdsErrorException)
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// FB_FileGets
        /// Read a String from File
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <returns></returns>
        public string FB_FileGets(UInt16 nFileHdl, out bool bEOF)
        {
            if (!tcClientFileCtrl.IsConnected)
            {
                bEOF = false;
                return null;
            }

            string ret;
            int numReadBytes = 0;
            bEOF = false;

            byte[] readData = new byte[254];     // T_MaxString - 1 (T_MaxString-Größe in der PLC sind 255)

            byte[] writeData = new byte[0];

            try
            {
                numReadBytes = tcClientFileCtrl.ReadWrite((int)E_IdxGrp.SYSTEMSERVICE_FGETS, nFileHdl, readData.AsMemory(), writeData.AsMemory());

                if (numReadBytes > 0)
                {
                    MemoryStream readStream = new MemoryStream(readData);
                    BinaryReader reader = new BinaryReader(readStream, System.Text.Encoding.Default);
                    ret = reader.ReadString();
                    bEOF = false;
                }
                else
                {
                    ret = string.Empty;
                    bEOF = true;
                }
            }
            catch (AdsErrorException)
            {
                ret = string.Empty;
            }
            return ret;
        }

        /// <summary>
        /// FB_FileGets
        /// Read a String from File
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <returns></returns>
        public string FB_FileGets(UInt16 nFileHdl)
        {
            bool dummy;
            return FB_FileGets(nFileHdl, out dummy);
        }

        /// <summary>
        /// FB_FileOpen
        /// Open a File
        /// </summary>
        /// <param name="sPathName"></param>
        /// <param name="ePath"></param>
        /// <param name="nMode"></param>
        /// <returns></returns>
        public UInt16 FB_FileOpen(string sPathName, E_OpenPath ePath, UInt32 nMode)
        {
            if (!tcClientFileCtrl.IsConnected)
                return 0;

            UInt16 ret = 0;
            int numReadBytes = 0;

            byte[] readData = new byte[sizeof(UInt32)];

            byte[] writeData = new byte[sPathName.Length + 1];
            MemoryStream writeStream = new MemoryStream(writeData);
            BinaryWriter writer = new BinaryWriter(writeStream, System.Text.Encoding.ASCII);
            writer.Write(sPathName.ToCharArray());
            writer.Write('\0'); //add terminating zero

            try
            {
                numReadBytes = tcClientFileCtrl.ReadWrite((int)E_IdxGrp.SYSTEMSERVICE_FOPEN, ((UInt32)ePath << 16) | nMode, readData.AsMemory(), writeData.AsMemory());
                if (numReadBytes >= sizeof(UInt32))
                {
                    MemoryStream readStream = new MemoryStream(readData);
                    BinaryReader reader = new BinaryReader(readStream, System.Text.Encoding.Default);
                    ret = (UInt16)reader.ReadUInt32();
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

        /// <summary>
        /// FB_FilePuts
        /// Write String to File
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <param name="sLine"></param>
        /// <returns></returns>
        public bool FB_FilePuts(UInt16 nFileHdl, string sLine)
        {
            if (!tcClientFileCtrl.IsConnected)
                return false;

            bool ret = false;

            byte[] readData = new byte[0];

            byte[] writeData = new byte[sLine.Length + 1];
            MemoryStream writeStream = new MemoryStream(writeData);
            BinaryWriter writer = new BinaryWriter(writeStream, System.Text.Encoding.ASCII);
            writer.Write(sLine.ToCharArray());
            writer.Write('\0'); //add terminating zero

            try
            {
                tcClientFileCtrl.ReadWrite((int)E_IdxGrp.SYSTEMSERVICE_FPUTS, nFileHdl, readData.AsMemory(), writeData.AsMemory());
                ret = true;
            }
            catch (AdsErrorException)
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// FB_FileRead
        /// Read n Bytes from File
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <param name="nReadLen"></param>
        /// <param name="pReadBuff"></param>
        /// <returns></returns>
        public byte[] FB_FileRead(UInt16 nFileHdl, UInt32 nReadLen, out bool bEOF)
        {
            if (!tcClientFileCtrl.IsConnected)
            {
                bEOF = false;
                return null;
            }

            byte[] ret;
            bEOF = false;
            int numReadBytes = 0;

            byte[] readData = new byte[nReadLen];

            byte[] writeData = new byte[0];

            try
            {
                numReadBytes = tcClientFileCtrl.ReadWrite((int)E_IdxGrp.SYSTEMSERVICE_FREAD, nFileHdl, readData.AsMemory(), writeData.AsMemory());
                if (numReadBytes > 0)
                {
                    MemoryStream readStream = new MemoryStream(readData);
                    BinaryReader reader = new BinaryReader(readStream, System.Text.Encoding.Default);
                    ret = reader.ReadBytes(numReadBytes);
                }
                else
                {
                    bEOF = true;
                    ret = null;
                }
            }
            catch (AdsErrorException)
            {
                ret = null;
            }
            return ret;
        }

        /// <summary>
        /// FB_FileRead
        /// Read n Bytes from File
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <param name="nReadLen"></param>
        /// <param name="pReadBuff"></param>
        /// <returns></returns>
        public byte[] FB_FileRead(UInt16 nFileHdl, UInt32 nReadLen)
        {
            bool dummy;
            return FB_FileRead(nFileHdl, nReadLen, out dummy);
        }

        /// <summary>
        /// FB_FileRename
        /// Renames a File
        /// </summary>
        /// <param name="sOldName"></param>
        /// <param name="sNewName"></param>
        /// <param name="ePath"></param>
        /// <returns></returns>
        public bool FB_FileRename(string sOldName, string sNewName, E_OpenPath ePath)
        {
            if (!tcClientFileCtrl.IsConnected)
                return false;

            bool ret = false;

            byte[] readData = new byte[0];

            byte[] writeData = new byte[sOldName.Length + 1 + sNewName.Length + 1];
            MemoryStream writeStream = new MemoryStream(writeData);
            BinaryWriter writer = new BinaryWriter(writeStream, System.Text.Encoding.ASCII);
            writer.Write(sOldName.ToCharArray());
            writer.Write('\0');
            writer.Write(sNewName.ToCharArray());
            writer.Write('\0');

            try
            {
                if ((sOldName.Length + 1 + sNewName.Length + 1) <= 255)
                {
                    tcClientFileCtrl.ReadWrite((int)E_IdxGrp.SYSTEMSERVICE_FRENAME, (UInt32)ePath << 16, readData.AsMemory(), writeData.AsMemory());
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

        /// <summary>
        /// FB_FileSeek
        /// Moves the file pointer (if any) associated with stream to a new location that is offset bytes from origin. You can use FB_FileSeek to reposition the pointer anywhere in a file. The pointer can also be positioned beyond the end of the file, FB_FileSeek clears the end-of-file indicator.
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <param name="nSeekPos"></param>
        /// <param name="eOrigin"></param>
        /// <returns></returns>
        public bool FB_FileSeek(UInt16 nFileHdl, UInt32 nSeekPos, E_SeekOrigin eOrigin)
        {
            if (!tcClientFileCtrl.IsConnected)
                return false;

            bool ret;

            byte[] readData = new byte[0];

            byte[] writeData = new byte[2 * sizeof(Int32)];
            MemoryStream writeStream = new MemoryStream(writeData);
            BinaryWriter writer = new BinaryWriter(writeStream, System.Text.Encoding.ASCII);
            writer.Write((uint)nSeekPos);
            writer.Write((uint)eOrigin);

            try
            {
                tcClientFileCtrl.ReadWrite((int)E_IdxGrp.SYSTEMSERVICE_FSEEK, nFileHdl, readData.AsMemory(), writeData.AsMemory());
                ret = true;
            }
            catch (AdsErrorException)
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// FB_FileTell
        /// Gets the current position of a file pointer
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <returns></returns>
        public Int32 FB_FileTell(UInt16 nFileHdl)
        {
            if (!tcClientFileCtrl.IsConnected)
                return 0;

            Int32 ret = 0;
            int numReadBytes = 0;

            byte[] readData = new byte[sizeof(Int32)];

            byte[] writeData = new byte[0];

            try
            {
                numReadBytes = tcClientFileCtrl.ReadWrite((int)E_IdxGrp.SYSTEMSERVICE_FTELL, nFileHdl, readData.AsMemory(), writeData.AsMemory());
                if (numReadBytes > 0)
                {
                    MemoryStream readStream = new MemoryStream(readData);
                    BinaryReader reader = new BinaryReader(readStream, System.Text.Encoding.Default);
                    ret = reader.ReadInt32();
                }
                else
                {
                    ret = -1;
                }
            }
            catch (AdsErrorException)
            {
                ret = -1;
            }
            return ret;
        }

        /// <summary>
        /// FB_FileWrite
        /// Writes data to the file stream
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <param name="aWriteBuff"></param>
        /// <param name="nWriteLen"></param>
        /// <returns></returns>
        public bool FB_FileWrite(UInt16 nFileHdl, byte[] aWriteBuff, UInt32 nWriteLen)
        {
            if (!tcClientFileCtrl.IsConnected)
                return false;

            bool ret = false;

            byte[] readData = new byte[0];

            try
            {
                tcClientFileCtrl.ReadWrite((int)E_IdxGrp.SYSTEMSERVICE_FWRITE, nFileHdl, readData.AsMemory(), aWriteBuff.AsMemory(0, (int)nWriteLen));
                ret = true;
            }
            catch (AdsErrorException)
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// FB_RemoveDir
        /// Remove (delete) directory
        /// </summary>
        /// <param name="sPathName"></param>
        /// <param name="ePath"></param>
        /// <returns></returns>
        public bool FB_RemoveDir(string sPathName, E_OpenPath ePath)
        {
            if (!tcClientFileCtrl.IsConnected)
                return false;

            bool ret = false;

            byte[] readData = new byte[0];

            byte[] writeData = new byte[sPathName.Length + 1];
            MemoryStream writeStream = new MemoryStream(writeData);
            BinaryWriter writer = new BinaryWriter(writeStream, System.Text.Encoding.ASCII);
            writer.Write(sPathName.ToCharArray());
            writer.Write('\0'); //add terminating zero

            try
            {
                var value = tcClientFileCtrl.ReadWrite((int)E_IdxGrp.SYSTEMSERVICE_RMDIR, (uint)ePath, readData.AsMemory(), writeData.AsMemory());
                ret = true;
            }
            catch (AdsErrorException)
            {
                ret = false;
            }
            return ret;
        }

        /// <summary>
        /// FB_EnumFindFileEntry
        /// This function block searches a directory for a file or subdirectory whose name matches the specified file name.
        /// </summary>
        /// <param name="sPathName"></param>
        /// <param name="eCmd"></param>
        /// <returns></returns>
        public ST_FindFileResult FB_EnumFindFileEntry(string sPathName, E_EnumCmdType eCmd, UInt16 PreviousFhandle)
        {
            if (!tcClientFileCtrl.IsConnected)
                return new ST_FindFileResult();

            ST_FindFileResult stFindFileResult = new ST_FindFileResult();
            ST_AmsFindFileSystemEntry dataEntry = new ST_AmsFindFileSystemEntry();

            dataEntry.hFile = PreviousFhandle;


            if ((eCmd == E_EnumCmdType.eEnumCmd_First && dataEntry.hFile != 0) || (eCmd == E_EnumCmdType.eEnumCmd_Abort))
            {
                try
                {
                    tcClientFileCtrl.WriteAny((int)E_IdxGrp.SYSTEMSERVICE_CLOSEHANDLE, 0, dataEntry.hFile);
                    dataEntry.hFile = 0;
                }
                catch (AdsErrorException err)
                {
                    stFindFileResult.bErr = true;
                    stFindFileResult.ErrId = (UInt32)err.ErrorCode;
                    return stFindFileResult;
                }
            }

            if ((eCmd == E_EnumCmdType.eEnumCmd_First && dataEntry.hFile == 0) || (eCmd == E_EnumCmdType.eEnumCmd_Next && dataEntry.hFile != 0))
            {
                try
                {
                    byte[] readData = new byte[Marshal.SizeOf(dataEntry)];

                    if (eCmd == E_EnumCmdType.eEnumCmd_First)
                    {
                        byte[] writeData = new byte[sPathName.Length + 1];
                        MemoryStream writeStream = new MemoryStream(writeData);
                        BinaryWriter writer = new BinaryWriter(writeStream, System.Text.Encoding.ASCII);
                        writer.Write(sPathName.ToCharArray());
                        writer.Write('\0'); //add terminating zero

                        tcClientFileCtrl.ReadWrite((int)E_IdxGrp.SYSTEMSERVICE_FFILEFIND, (int)E_OpenPath.PATH_GENERIC, readData.AsMemory(), writeData.AsMemory());
                    }
                    else
                    {
                        byte[] writeData = new byte[0];
                        tcClientFileCtrl.ReadWrite((int)E_IdxGrp.SYSTEMSERVICE_FFILEFIND, dataEntry.hFile, readData.AsMemory(), writeData.AsMemory());
                    }

                    dataEntry = (ST_AmsFindFileSystemEntry)ByteArrayToStruct(readData, typeof(ST_AmsFindFileSystemEntry));

                    stFindFileResult.FileEntry.fileAttributes.bReadOnly = (dataEntry.dwFileAttributes & 1) != 0;
                    stFindFileResult.FileEntry.fileAttributes.bHidden = (dataEntry.dwFileAttributes & 2) != 0;
                    stFindFileResult.FileEntry.fileAttributes.bSystem = (dataEntry.dwFileAttributes & 4) != 0;
                    stFindFileResult.FileEntry.fileAttributes.bDirectory = (dataEntry.dwFileAttributes & 16) != 0;
                    stFindFileResult.FileEntry.fileAttributes.bArchive = (dataEntry.dwFileAttributes & 32) != 0;
                    stFindFileResult.FileEntry.fileAttributes.bDevice = (dataEntry.dwFileAttributes & 64) != 0;
                    stFindFileResult.FileEntry.fileAttributes.bNormal = (dataEntry.dwFileAttributes & 128) != 0;
                    stFindFileResult.FileEntry.fileAttributes.bTemporary = (dataEntry.dwFileAttributes & 256) != 0;
                    stFindFileResult.FileEntry.fileAttributes.bSparseFile = (dataEntry.dwFileAttributes & 512) != 0;
                    stFindFileResult.FileEntry.fileAttributes.bReparsePoint = (dataEntry.dwFileAttributes & 1024) != 0;
                    stFindFileResult.FileEntry.fileAttributes.bCompressed = (dataEntry.dwFileAttributes & 2048) != 0;
                    stFindFileResult.FileEntry.fileAttributes.bOffline = (dataEntry.dwFileAttributes & 4096) != 0;
                    stFindFileResult.FileEntry.fileAttributes.bNotContentIndexed = (dataEntry.dwFileAttributes & 8192) != 0;
                    stFindFileResult.FileEntry.fileAttributes.bEncrypted = (dataEntry.dwFileAttributes & 16384) != 0;

                    stFindFileResult.FileEntry.creationTime = dataEntry.creationTime;
                    stFindFileResult.FileEntry.lastAccessTime = dataEntry.lastAccessTime;
                    stFindFileResult.FileEntry.lastWriteTime = dataEntry.lastWriteTime;
                    stFindFileResult.FileEntry.fileSize = (dataEntry.nFileSizeHigh << 32) + dataEntry.nFileSizeLow;
                    stFindFileResult.FileEntry.sFileName = dataEntry.sFileName;
                    stFindFileResult.FileEntry.sAlternateFileName = dataEntry.sAlternateFileName;
                    stFindFileResult.hFile = dataEntry.hFile;
                }
                catch (AdsErrorException err)
                {
                    if ((int)err.ErrorCode == 1804)
                    {
                        stFindFileResult.EOF = true;
                    }
                    else
                    {
                        stFindFileResult.bErr = true;
                        stFindFileResult.ErrId = (UInt32)err.ErrorCode;
                        return stFindFileResult;
                    }
                }
            }
            return stFindFileResult;
        }

        /// <summary>
        /// FB_EnumFindFileList
        /// This function block searches a directory for a file or subdirectory whose name matches the specified file name.
        /// </summary>
        /// <param name="sPathName"></param>
        /// <param name="eCmd"></param>
        /// <returns></returns>
        public List<ST_FindFileEntry> FB_EnumFindFileList(string sPathName, E_EnumCmdType eCmd)
        {
            if (!tcClientFileCtrl.IsConnected)
                return null;

            List<ST_FindFileEntry> fileList = new List<ST_FindFileEntry>();
            ST_FindFileResult Entry = new ST_FindFileResult();

            while (!Entry.EOF && !Entry.bErr)
            {
                if (fileList.Count == 0)
                {
                    Entry = FB_EnumFindFileEntry(sPathName, eCmd, 0);
                }
                else
                {
                    Entry = FB_EnumFindFileEntry(sPathName, E_EnumCmdType.eEnumCmd_Next, Entry.hFile);
                }

                if (!Entry.EOF)
                {
                    fileList.Add(Entry.FileEntry);
                }
            }
            return fileList;
        }

        /// <summary>
        /// NT_StartProcess
        /// Starts a Process on the target system via ADS
        /// </summary>
        /// <param name="PATHSTR"></param>
        /// <param name="DIRNAME"></param>
        /// <param name="COMNDLINE"></param>
        /// <returns></returns>
        public bool NT_StartProcess(string PATHSTR, string DIRNAME, string COMNDLINE)
        {
            if (!tcClientFileCtrl.IsConnected)
                return false;

            bool ret = false;

            ST_AmsStartProcessReq req = new ST_AmsStartProcessReq();

            req.LenPath = (UInt32)PATHSTR.Length;
            req.LenDir = (UInt32)DIRNAME.Length;
            req.LenComLine = (UInt32)COMNDLINE.Length;

            req.pData = new ST_AmsStartProcessReqData[3];
            req.pData[0].data = PATHSTR;
            req.pData[1].data = DIRNAME;
            req.pData[2].data = COMNDLINE;

            try
            {
                tcClientFileCtrl.WriteAny((int)E_IdxGrp.SYSTEMSERVICE_STARTPROCESS, 0, req);
                ret = true;
            }
            catch (AdsErrorException)
            {
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// ByteArrayToStruct
        /// Helperfunction to convert a ByteArray into a Struct
        /// </summary>
        /// <param name="array"></param>
        /// <param name="structType"></param>
        /// <returns></returns>
        public static object ByteArrayToStruct(byte[] array, Type structType)
        {
            int offset = 0;
            if (structType.StructLayoutAttribute.Value != LayoutKind.Sequential)
                throw new ArgumentException("structType ist keine Struktur oder nicht Sequentiell.");

            int size = Marshal.SizeOf(structType);
            if (array.Length < (offset + size))
                throw new ArgumentException("Byte-Array hat die falsche Länge.");

            byte[] tmp = new byte[size];
            Array.Copy(array, offset, tmp, 0, size);

            GCHandle structHandle = GCHandle.Alloc(tmp, GCHandleType.Pinned);
            object structure = Marshal.PtrToStructure(structHandle.AddrOfPinnedObject(), structType);
            structHandle.Free();

            return structure;
        }

        /// <summary>
        /// FB_CreateDir
        /// Create new directory
        /// Static wrapper
        /// </summary>
        /// <param name="sPathName"></param>
        /// <param name="ePath"></param>
        /// <returns></returns>
        public static bool FB_CreateDir(string NetID, int Port, string sPathName, E_OpenPath ePath)
        {
            AdsFileCtrl ctrl = new AdsFileCtrl();
            if (!ctrl.Connect(NetID, Port))
                return false;

            bool ret = ctrl.FB_CreateDir(sPathName, ePath);

            ctrl.Dispose();

            return ret;
        }

        /// <summary>
        /// FB_EOF
        /// Test for end-of-file
        /// Static wrapper
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <returns></returns>
        public static bool FB_EOF(string NetID, int Port, UInt16 nFileHdl)
        {
            AdsFileCtrl ctrl = new AdsFileCtrl();
            if (!ctrl.Connect(NetID, Port))
                return false;

            bool ret = ctrl.FB_EOF(nFileHdl);

            ctrl.Dispose();

            return ret;
        }

        /// <summary>
        /// FB_FileClose
        /// Close a File
        /// Static wrapper
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <returns></returns>
        public static bool FB_FileClose(string NetID, int Port, UInt16 nFileHdl)
        {
            AdsFileCtrl ctrl = new AdsFileCtrl();
            if (!ctrl.Connect(NetID, Port))
                return false;

            bool ret = ctrl.FB_FileClose(nFileHdl);

            ctrl.Dispose();

            return ret;
        }

        /// <summary>
        /// FB_FileDelete
        /// Delete a File
        /// Static Wrapper
        /// </summary>
        /// <param name="sPathName"></param>
        /// <param name="ePath"></param>
        /// <returns></returns>
        public static bool FB_FileDelete(string NetID, int Port, string sPathName, E_OpenPath ePath)
        {
            AdsFileCtrl ctrl = new AdsFileCtrl();
            if (!ctrl.Connect(NetID, Port))
                return false;

            bool ret = ctrl.FB_FileDelete(sPathName, ePath);

            ctrl.Dispose();

            return ret;
        }

        /// <summary>
        /// FB_FileGets
        /// Read a String from File
        /// Static Wrapper
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <returns></returns>
        public static string FB_FileGets(string NetID, int Port, UInt16 nFileHdl, out bool bEOF)
        {
            bEOF = false;

            AdsFileCtrl ctrl = new AdsFileCtrl();
            if (!ctrl.Connect(NetID, Port))
                return null;

            string ret = ctrl.FB_FileGets(nFileHdl, out bEOF);

            ctrl.Dispose();

            return ret;
        }

        /// <summary>
        /// FB_FileGets
        /// Read a String from File
        /// Static Wrapper
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <returns></returns>
        public static string FB_FileGets(string NetID, int Port, UInt16 nFileHdl)
        {
            bool dummy;

            return FB_FileGets(NetID, Port, nFileHdl, out dummy);
        }

        /// <summary>
        /// FB_FileOpen
        /// Open a File
        /// Static Wrapper
        /// </summary>
        /// <param name="sPathName"></param>
        /// <param name="ePath"></param>
        /// <param name="nMode"></param>
        /// <returns></returns>
        public static UInt16 FB_FileOpen(string NetID, int Port, string sPathName, E_OpenPath ePath, UInt32 nMode)
        {
            AdsFileCtrl ctrl = new AdsFileCtrl();
            if (!ctrl.Connect(NetID, Port))
                return 0;

            UInt16 ret = ctrl.FB_FileOpen(sPathName, ePath, nMode);

            ctrl.Dispose();

            return ret;
        }

        /// <summary>
        /// FB_FilePuts
        /// Write String to File
        /// Static Wrapper
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <param name="sLine"></param>
        /// <returns></returns>
        public static bool FB_FilePuts(string NetID, int Port, UInt16 nFileHdl, string sLine)
        {
            AdsFileCtrl ctrl = new AdsFileCtrl();
            if (!ctrl.Connect(NetID, Port))
                return false;

            bool ret = ctrl.FB_FilePuts(nFileHdl, sLine);

            ctrl.Dispose();

            return ret;
        }

        /// <summary>
        /// FB_FileRead
        /// Read n Bytes from File
        /// Static Wrapper
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <param name="nReadLen"></param>
        /// <param name="pReadBuff"></param>
        /// <returns></returns>
        public static byte[] FB_FileRead(string NetID, int Port, UInt16 nFileHdl, UInt32 nReadLen, out bool bEOF)
        {
            bEOF = false;

            AdsFileCtrl ctrl = new AdsFileCtrl();
            if (!ctrl.Connect(NetID, Port))
                return null;

            byte[] ret = ctrl.FB_FileRead(nFileHdl, nReadLen, out bEOF);

            ctrl.Dispose();

            return ret;
        }

        /// <summary>
        /// FB_FileRead
        /// Read n Bytes from File
        /// Static Wrapper
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <param name="nReadLen"></param>
        /// <param name="pReadBuff"></param>
        /// <returns></returns>
        public static byte[] FB_FileRead(string NetID, int Port, UInt16 nFileHdl, UInt32 nReadLen)
        {
            bool dummy;

            return FB_FileRead(NetID, Port, nFileHdl, nReadLen, out dummy);
        }

        /// <summary>
        /// FB_FileRename
        /// Renames a File
        /// Static Wrapper
        /// </summary>
        /// <param name="sOldName"></param>
        /// <param name="sNewName"></param>
        /// <param name="ePath"></param>
        /// <returns></returns>
        public static bool FB_FileRename(string NetID, int Port, string sOldName, string sNewName, E_OpenPath ePath)
        {
            AdsFileCtrl ctrl = new AdsFileCtrl();
            if (!ctrl.Connect(NetID, Port))
                return false;

            bool ret = ctrl.FB_FileRename(sOldName, sNewName, ePath);

            ctrl.Dispose();

            return ret;
        }

        /// <summary>
        /// FB_FileSeek
        /// Moves the file pointer (if any) associated with stream to a new location that is offset bytes from origin. 
        /// You can use FB_FileSeek to reposition the pointer anywhere in a file. The pointer can also be positioned beyond the end of the file, FB_FileSeek clears the end-of-file indicator.
        /// Static Wrapper
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <param name="nSeekPos"></param>
        /// <param name="eOrigin"></param>
        /// <returns></returns>
        public static bool FB_FileSeek(string NetID, int Port, UInt16 nFileHdl, UInt32 nSeekPos, E_SeekOrigin eOrigin)
        {
            AdsFileCtrl ctrl = new AdsFileCtrl();
            if (!ctrl.Connect(NetID, Port))
                return false;

            bool ret = ctrl.FB_FileSeek(nFileHdl, nSeekPos, eOrigin);

            ctrl.Dispose();

            return ret;
        }

        /// <summary>
        /// FB_FileTell
        /// Gets the current position of a file pointer
        /// Static Wrapper
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <returns></returns>
        public static Int32 FB_FileTell(string NetID, int Port, UInt16 nFileHdl)
        {
            AdsFileCtrl ctrl = new AdsFileCtrl();
            if (!ctrl.Connect(NetID, Port))
                return 0;

            Int32 ret = ctrl.FB_FileTell(nFileHdl);

            ctrl.Dispose();

            return ret;
        }

        /// <summary>
        /// FB_FileWrite
        /// Writes data to the file stream
        /// Static Wrapper
        /// </summary>
        /// <param name="nFileHdl"></param>
        /// <param name="aWriteBuff"></param>
        /// <param name="nWriteLen"></param>
        /// <returns></returns>
        public static bool FB_FileWrite(string NetID, int Port, UInt16 nFileHdl, byte[] aWriteBuff, UInt32 nWriteLen)
        {
            AdsFileCtrl ctrl = new AdsFileCtrl();
            if (!ctrl.Connect(NetID, Port))
                return false;

            bool ret = ctrl.FB_FileWrite(nFileHdl, aWriteBuff, nWriteLen);

            ctrl.Dispose();

            return ret;
        }

        /// <summary>
        /// FB_RemoveDir
        /// Remove (delete) directory
        /// Static Wrapper
        /// </summary>
        /// <param name="sPathName"></param>
        /// <param name="ePath"></param>
        /// <returns></returns>
        public static bool FB_RemoveDir(string NetID, int Port, string sPathName, E_OpenPath ePath)
        {
            AdsFileCtrl ctrl = new AdsFileCtrl();
            if (!ctrl.Connect(NetID, Port))
                return false;

            bool ret = ctrl.FB_RemoveDir(sPathName, ePath);

            ctrl.Dispose();

            return ret;
        }

        /// <summary>
        /// FB_EnumFindFileEntry
        /// This function block searches a directory for a file or subdirectory whose name matches the specified file name.
        /// Static Wrapper
        /// </summary>
        /// <param name="sPathName"></param>
        /// <param name="eCmd"></param>
        /// <returns></returns>
        public static ST_FindFileResult FB_EnumFindFileEntry(string NetID, int Port, string sPathName, E_EnumCmdType eCmd, UInt16 PreviousFhandle)
        {
            AdsFileCtrl ctrl = new AdsFileCtrl();
            if (!ctrl.Connect(NetID, Port))
                return new ST_FindFileResult();

            ST_FindFileResult ret = ctrl.FB_EnumFindFileEntry(sPathName, eCmd, PreviousFhandle);

            ctrl.Dispose();

            return ret;
        }

        /// <summary>
        /// FB_EnumFindFileList
        /// This function block searches a directory for a file or subdirectory whose name matches the specified file name.
        /// Static Wrapper
        /// </summary>
        /// <param name="sPathName"></param>
        /// <param name="eCmd"></param>
        /// <returns></returns>
        public static List<ST_FindFileEntry> FB_EnumFindFileList(string NetID, int Port, string sPathName, E_EnumCmdType eCmd)
        {
            AdsFileCtrl ctrl = new AdsFileCtrl();
            if (!ctrl.Connect(NetID, Port))
                return null;

            List<ST_FindFileEntry> ret = ctrl.FB_EnumFindFileList(sPathName, eCmd);

            ctrl.Dispose();

            return ret;
        }

        /// <summary>
        /// NT_StartProcess
        /// Starts a Process on the target system via ADS
        /// Static Wrapper
        /// </summary>
        /// <param name="PATHSTR"></param>
        /// <param name="DIRNAME"></param>
        /// <param name="COMNDLINE"></param>
        /// <returns></returns>
        public static bool NT_StartProcess(string NetID, int Port, string PATHSTR, string DIRNAME, string COMNDLINE)
        {
            AdsFileCtrl ctrl = new AdsFileCtrl();
            if (!ctrl.Connect(NetID, Port))
                return false;

            bool ret = ctrl.NT_StartProcess(PATHSTR, DIRNAME, COMNDLINE);

            ctrl.Dispose();

            return ret;
        }
    }
}
