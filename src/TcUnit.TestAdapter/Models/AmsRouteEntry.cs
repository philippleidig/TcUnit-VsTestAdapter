using System;
using TwinCAT.Ads;

namespace TcUnit.TestAdapter.Models
{
    public enum RouteTransportType : uint
    {
        None,
        TCP_IP,
        IIO_LIGHTBUS,
        PROFIBUS_DP,
        PCI_ISA_BUS,
        ADS_UDP,
        FATP_UDP,
        COM_PORT,
        USB,
        CAN_OPEN,
        DEVICE_NET,
        SSB,
        SOAP
    }

    public class AmsRouteEntry
    {
        public string Name { get; set; }
        public AmsNetId NetId { get; set; }
        public string Address { get; set; }
        public RouteTransportType TransportType { get; set; }
        public TimeSpan Timeout { get; set; }
        public uint Flags { get; set; }
    }
}
