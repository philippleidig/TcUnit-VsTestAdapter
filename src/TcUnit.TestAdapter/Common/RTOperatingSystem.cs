using System.Collections.Generic;
using System.Linq;

namespace TcUnit.TestAdapter.Common
{
    public static class RTOperatingSystem
    {
        public static string EngineeringOsName = "Windowsx64";

        public static void GetSeperatorByOsName(string osName, out string seperator)
        {
            if (osName.Contains("BSD"))
            {
                seperator = "/";
            }
            else if (osName.Contains("CE"))
            {
                seperator = "\\";
            }
            else
            {
                seperator = "\\";
            }
        }

        public static string GetSeperatorByOsName(string osName)
        {
            if (osName.Contains("BSD"))
            {
                return "/";
            }
            else if (osName.Contains("CE"))
            {
                return "\\";
            }
            else
            {
                return "\\";
            }
        }

        public static string GetEngineeringSeperator()
        {
            return GetSeperatorByOsName(EngineeringOsName);
        }

        public static void GetEntryPointByOsName(string osName, out string entryPoint)
        {
            if (osName.Contains("BSD"))
            {
                entryPoint = "";
            }
            else if (osName.Contains("CE"))
            {
                entryPoint = "\\Temp";
            }
            else
            {
                entryPoint = "C:";
            }
        }

        public static string GetEntryPointByOsName(string osName)
        {
            if (osName.Contains("BSD"))
            {
                return "";
            }
            else if (osName.Contains("CE"))
            {
                return "\\Temp";
            }
            else
            {
                return "C:";
            }
        }

        public static string GetEngineeringEntryPoint()
        {
            return GetEntryPointByOsName(EngineeringOsName);
        }

        public static void GetBootProjFolderByOsName(string osName, out string bootProjPath)
        {
            if (osName.Contains("BSD"))
            {
                bootProjPath = "/usr/local/etc/TwinCAT/3.1/Boot";
            }
            else if (osName.Contains("CE"))
            {
                bootProjPath = "\\Hard Disk\\TwinCAT\\3.1\\Boot";
            }
            else
            {
                bootProjPath = "C:\\TwinCAT\\3.1\\Boot";
            }
        }

        public static string GetBootProjFolderByOsName(string osName)
        {
            if (osName.Contains("BSD"))
            {
                return "/usr/local/etc/TwinCAT/3.1/Boot";
            }
            else if (osName.Contains("CE"))
            {
                return "\\Hard Disk\\TwinCAT\\3.1\\Boot";
            }
            else
            {
                return "C:\\TwinCAT\\3.1\\Boot";
            }
        }

        public static string GetEngineeringBootFolder()
        {
            return GetBootProjFolderByOsName(EngineeringOsName);
        }

        public static string GetBuildConfigurationFromRTPlatform(RTPlatform rtPlatform)
        {
            AvailableRTPlattforms.TryGetValue(rtPlatform, out var buildConfiguration);
            return buildConfiguration;
        }

        public static RTPlatform GetRTPlatformFromBuildConfiguration(string buildConfiguration)
        {
            var platform = AvailableRTPlattforms.FirstOrDefault(x => x.Value == buildConfiguration).Key;
            return platform;
        }

        public static IDictionary<RTPlatform, string> AvailableRTPlattforms = new Dictionary<RTPlatform, string>
        {
            { RTPlatform.WinCE, "TwinCAT CE7 (ARMV7)" },
            { RTPlatform.WinNT, "TwinCAT RT (x64)" },
            { RTPlatform.TcBSD, "TwinCAT RT (x64)" },
            { RTPlatform.TcRTOS, "TwinCAT OS (ARMT2)" }
        };

        public static RTPlatform GetRTPlatform(string osName)
        {

            if (osName.StartsWith("TC/BSD") || osName.StartsWith("TwinCAT/BSD"))
            {
                return RTPlatform.TcBSD;
            }
            else if (osName.StartsWith("TC/RTOS"))
            {
                return RTPlatform.TcRTOS;
            }
            else if (osName.StartsWith("Windows 10"))
            {
                return RTPlatform.WinNT;
            }
            else if (osName.StartsWith("Windows 11"))
            {
                return RTPlatform.WinNT;
            }
            else if (osName == "Win NT")
            {
                return RTPlatform.WinNT;
            }
            else if (osName == "Win 2000")
            {
                return RTPlatform.WinNT;
            }
            else if (osName == "Win XP" || osName == "Microsoft Windows XP")
            {
                return RTPlatform.WinNT;
            }
            else if (osName.StartsWith("Vista"))
            {
                return RTPlatform.WinNT;
            }
            else if (osName.StartsWith("Windows 7"))
            {
                return RTPlatform.WinNT;
            }
            else if (osName.StartsWith("Win7"))
            {
                return RTPlatform.WinNT;
            }
            else if (osName.StartsWith("Windows 8.1"))
            {
                return RTPlatform.WinNT;
            }
            else if (osName.StartsWith("Windows 8"))
            {
                return RTPlatform.WinNT;
            }
            else if (osName == "Win CE (4.20)")
            {
                return RTPlatform.WinCE;
            }
            else if (osName == "Win CE (5.0)")
            {
                return RTPlatform.WinCE;
            }
            else if (osName == "Win CE (6.0)")
            {
                return RTPlatform.WinCE;
            }
            else
            {
                return RTPlatform.Unknown;
            }
        }

    }
}
