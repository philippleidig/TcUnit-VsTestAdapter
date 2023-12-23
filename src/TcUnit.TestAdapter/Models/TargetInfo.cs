using System;
using TcUnit.TestAdapter.Common;

namespace TcUnit.TestAdapter.Models
{
    public class TargetInfo
    {
        public string TargetType { get; set; }
        public string HardwareModel { get; set; }
        public string HardwareSerialNo { get; set; }
        public string HardwareVersion { get; set; }
        public string HardwareDate { get; set; }
        public string HardwareCPU { get; set; }

        public string ImageDevice { get; set; }
        public string ImageVersion { get; set; }
        public string ImageLevel { get; set; }
        public string ImageOsName { get; set; }
        public string ImageOsVersion { get; set; }

        public RTPlatform RTPlatform { get; set; }

        public Version TwinCATVersion { get; set; }

        public bool IsTwinCATBSD => RTPlatform.Equals(RTPlatform.TcBSD);
        public bool IsWindowsNT => RTPlatform.Equals(RTPlatform.WinNT);
        public bool IsWindowsCE => RTPlatform.Equals(RTPlatform.WinCE);

    }
}
