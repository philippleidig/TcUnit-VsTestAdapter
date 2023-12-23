using System.Collections.Generic;
using System.IO;
using System;
using System.Text;

using TwinCAT;
using TwinCAT.Ads;

namespace TcUnit.TestAdapter.Models
{
    public static class TwinCATEnvironment
    {
        public static List<AmsRouteEntry> GetRemoteRoutes()
        {
            uint count = 0;
            List<AmsRouteEntry> remoteRoutes = new List<AmsRouteEntry>();

            using (AdsClient adsClient = new AdsClient())
            {
                adsClient.Connect(AmsNetId.Local, AmsPort.SystemService);

                if (!adsClient.IsConnected)
                    throw new Exception("Failed to connect to local TwinCAT runtime for reading remote ADS routes info.");

                for (; ; )
                {
                    try
                    {
                        AmsRouteEntry amsRouteEntry = new AmsRouteEntry();

                        var readData = new byte[2092];

                        using (MemoryStream ms = new MemoryStream(readData))
                        {
                            using (BinaryReader reader = new BinaryReader(ms))
                            {
                                adsClient.Read(0x323, count, readData.AsMemory());

                                amsRouteEntry.NetId = new AmsNetId(reader.ReadBytes(6));
                                amsRouteEntry.TransportType = (RouteTransportType)reader.ReadUInt16();
                                amsRouteEntry.Flags = reader.ReadUInt32();
                                amsRouteEntry.Timeout = TimeSpan.FromSeconds(reader.ReadUInt32());

                                reader.ReadUInt32(); // spare
                                reader.ReadUInt32(); // spare
                                reader.ReadUInt32(); // spare
                                reader.ReadUInt32(); // spare

                                var ipAddressLength = reader.ReadInt32();
                                var nameLength = reader.ReadInt32();

                                var additionalAmsNetIdsCount = reader.ReadUInt32();

                                var address = reader.ReadBytes(ipAddressLength);
                                amsRouteEntry.Address = Encoding.UTF8.GetString(address);

                                var name = reader.ReadBytes(nameLength);
                                amsRouteEntry.Name = Encoding.UTF8.GetString(name);

                                remoteRoutes.Add(amsRouteEntry);
                            }
                        }

                        continue;
                    }
                    catch (Exception ex)
                    {
                        if (!(ex is AdsException))
                        {
                            continue;
                        }
                    }
                    finally
                    {
                        count++;
                    }

                    break;
                }
                adsClient.Disconnect();
            }

            return remoteRoutes;
        }
    }
}
