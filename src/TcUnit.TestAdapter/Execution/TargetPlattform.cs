using System.Collections.Generic;


namespace TcUnit.Core
{
    public class TargetPlattform
    {
        public static string ConvertFrom()
        {
            //var isTcBSD = targetInfo.OsImage.RTOperatingSystem.Platform == RTPlatform.TcBSD;
            //var isWindows10 = targetInfo.OsImage.RTOperatingSystem.Platform == RTPlatform.WinNT;
            //var isWindowsCE = targetInfo.OsImage.RTOperatingSystem.Platform == RTPlatform.WinCE;
            //var isTcRTOS = targetInfo.OsImage.RTOperatingSystem.Platform == RTPlatform.TcRTOS;

            //if (isTcBSD)
            //    return "TwinCAT RT (x64)";
            //
            //if (isWindows10)
            //    return "TwinCAT RT (x64)";
            //
            //if (isWindowsCE)
            //    return "TwinCAT CE7 (ARMV7)";
            //
            //if (isTcRTOS)
            //    return "TwinCAT OS (ARMT2)";

            return "";
        }

        public static IEnumerable<string> AvailablePlattforms = new List<string>
        {
            "TwinCAT CE7 (ARMV7)",
             "TwinCAT RT (x64)",
             "TwinCAT RT (x86)",
             "TwinCAT OS (ARMT2)"
        };
    }
}
