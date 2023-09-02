using System;
using System.Collections.Generic;
using System.Text;

namespace Beckhoff.App.TcHelper.AdsFileCtrl
{
    public class OsHelper
    {
        public static string EngineeringOsName = "Windowsx64";
        public static void getSeperatorByOsName(string osName, out string seperator)
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

        public static string getSeperatorByOsName(string osName)
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

        public static string getEngineeringSeperator()
        {
            return getSeperatorByOsName(EngineeringOsName);
        }

        public static void getEntryPointByOsName(string osName, out string entryPoint)
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

        public static string getEntryPointByOsName(string osName)
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

        public static string getEngineeringEntryPoint()
        {
            return getEntryPointByOsName(EngineeringOsName);
        }

        public static void getBootProjFolderByOsName(string osName, out string bootProjPath)
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

        public static string getBootProjFolderByOsName(string osName)
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

        public static string getEngineeringBootFolder()
        {
            return getBootProjFolderByOsName(EngineeringOsName);
        }
    }
}
