using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TcUnit.TestAdapter.Models
{
    public class AdsFileSystemTypes
    {
        public enum AdsFileOpenMode : uint
        {
            FOPEN_MODEREAD = 0x1,                     
            FOPEN_MODEWRITE = 0x2,                    
            FOPEN_MODEAPPEND = 0x4,                                   
            FOPEN_MODEPLUS = 0x8,          
            FOPEN_MODEBINARY = 0x10,         
            FOPEN_MODETEXT = 0x20          
        }

        public enum AdsDirectory : int
        {
            PATH_GENERIC = 1,             
            PATH_BOOTPRJ,                        
            PATH_BOOTDATA,              
            PATH_BOOTPATH,                     
            PATH_USERPATH1 = 11,        
            PATH_USERPATH2,             
            PATH_USERPATH3,             
            PATH_USERPATH4,             
            PATH_USERPATH5,             
            PATH_USERPATH6,             
            PATH_USERPATH7,             
            PATH_USERPATH8,             
            PATH_USERPATH9              
        }

        public enum AdsFileSystemSeekOrigin : int
        {
            SEEK_SET = 0,         
            SEEK_CUR,                   
            SEEK_END                   
        }

        public enum AdsIndexGroup : int
        {
            SYSTEMSERVICE_OPENCREATE = 100,
            SYSTEMSERVICE_OPENREAD = 101,
            SYSTEMSERVICE_OPENWRITE = 102,
            SYSTEMSERVICE_CREATEFILE = 110,
            SYSTEMSERVICE_CLOSEHANDLE = 111,
            SYSTEMSERVICE_FOPEN = 120,
            SYSTEMSERVICE_FCLOSE = 121,
            SYSTEMSERVICE_FREAD = 122,
            SYSTEMSERVICE_FWRITE = 123,
            SYSTEMSERVICE_FSEEK = 124,
            SYSTEMSERVICE_FTELL = 125,
            SYSTEMSERVICE_FGETS = 126,
            SYSTEMSERVICE_FPUTS = 127,
            SYSTEMSERVICE_FSCANF = 128,
            SYSTEMSERVICE_FPRINTF = 129,
            SYSTEMSERVICE_FEOF = 130,
            SYSTEMSERVICE_FDELETE = 131,
            SYSTEMSERVICE_FRENAME = 132,

            SYSTEMSERVICE_FFILEFIND = 133,

            SYSTEMSERVICE_MKDIR = 138,
            SYSTEMSERVICE_RMDIR = 139,
        }

        public enum AdsFileSystemCommandType
        {
            eEnumCmd_First = 0,    
            eEnumCmd_Next,    
            eEnumCmd_Abort      
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct AdsFileSystemEntryAttributes
        {
            [MarshalAs(UnmanagedType.I1)]
            public bool ReadOnly;                                            
            [MarshalAs(UnmanagedType.I1)]
            public bool Hidden;                                
            [MarshalAs(UnmanagedType.I1)]
            public bool System;                                   
            [MarshalAs(UnmanagedType.I1)]
            public bool Directory;                  
            [MarshalAs(UnmanagedType.I1)]
            public bool Archive;                                     
            [MarshalAs(UnmanagedType.I1)]
            public bool Device;                       
            [MarshalAs(UnmanagedType.I1)]
            public bool Normal;                                  
            [MarshalAs(UnmanagedType.I1)]
            public bool Temporary;                                                             
            [MarshalAs(UnmanagedType.I1)]
            public bool SparseFile;                   
            [MarshalAs(UnmanagedType.I1)]
            public bool ReparsePoint;                  
            [MarshalAs(UnmanagedType.I1)]
            public bool Compressed;                                                  
            [MarshalAs(UnmanagedType.I1)]
            public bool Offline;                                                    
            [MarshalAs(UnmanagedType.I1)]
            public bool NotContentIndexed;           
            [MarshalAs(UnmanagedType.I1)]
            public bool Encrypted;                                                 
        }

        public struct AdsFileTime
        {
            public uint LowDateTime;                  
            public uint HighDateTime;                 
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct AdsFileSystemEntry
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]     
            public string FileName;                                          
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]      
            public string AlternateFileName;                                             
            public AdsFileSystemEntryAttributes fileAttributes;                        
            public ulong FileSize;                                                  
            public AdsFileTime CreationTime;                                         
            public AdsFileTime LastAccessTime;                                                 
            public AdsFileTime LastWriteTime;                                                 
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct AmsFileSystemEntry
        {
            public ushort File;                     
            public ushort Reserved;

            public uint FileAttributes;                
            public AdsFileTime CreationTime;                 
            public AdsFileTime LastAccessTime;               
            public AdsFileTime LastWriteTime;                
            public uint FileSizeHigh;                    
            public uint FileSizeLow;                     
            public uint Reserved0;                       
            public uint Reserved1;                       
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string FileName;                      
            public uint Reserved2;                       
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string AlternateFileName;              
            public ushort Reserved3;                       
        }

        public struct AdsFindFileSystemEntryResult
        {
            public AdsFileSystemEntry FileEntry;
            public ushort File;
            public bool EOF;
            public bool Error;
            public uint ErrorID;
        }
    }
}
