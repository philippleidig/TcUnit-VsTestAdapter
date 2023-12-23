using System;
using System.IO;

using TwinCAT.Ads;

using TcUnit.TestAdapter.Models;

namespace TcUnit.TestAdapter.Services
{
    public class EventRaisedEventArgs
    {
        public EventRaisedEventArgs(AdsLogEntry eventMessage) { Event = eventMessage; }
        public AdsLogEntry Event { get; } 
    }

    public class TwinCATEventListener
    {
        private readonly string target;
        private readonly AmsNetId amsNetId;
        private readonly AdsClient adsClient;

        private uint handle;

        public delegate void EventRaisedEventHandler(object sender, EventRaisedEventArgs e);
        public event EventRaisedEventHandler EventRaised;

        public TwinCATEventListener(AmsNetId target)
        {
            this.target = target.ToString();
            amsNetId = target;
            adsClient = new AdsClient();
            Connect();
        }
        public TwinCATEventListener(string target) 
            : this(new AmsNetId(target))
        {


        }

        public void Connect()
        {
            adsClient.Connect(amsNetId, (int)AmsPort.Logger);
            adsClient.AdsNotification += OnEventOccured;

            var settings = new NotificationSettings(AdsTransMode.CyclicInContext, 0, 0);
            handle = adsClient.AddDeviceNotification(0x1, 0xffff, 1024, settings, null);        
        }

        public void Disconnect()
        {
            adsClient.AdsNotification -= OnEventOccured;

            adsClient.DeleteDeviceNotification(handle);
            adsClient.Disconnect();
        }

        private void OnEventOccured(object sender, AdsNotificationEventArgs e)
        {
            var eventMessage = ParseEvent(e.Data);
            EventRaised?.Invoke(this, new EventRaisedEventArgs(eventMessage));
        }

        private AdsLogEntry ParseEvent(ReadOnlyMemory<byte> eventData)
        {   
            using (MemoryStream ms = new MemoryStream(eventData.ToArray()))
            {
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    var timestamp = reader.ReadInt64();
                    var timeRaised = DateTime.FromFileTime(timestamp);
                    var logLevel = (AdsLogLevel)reader.ReadInt32();
                    var senderPort = reader.ReadInt32();
                    var senderData = reader.ReadBytes(16);
                    var sender = System.Text.Encoding.UTF8.GetString(senderData).Trim('\0');
                    var messageLength = reader.ReadInt32();
                    var messageData = reader.ReadBytes(messageLength);
                    var message = System.Text.Encoding.UTF8.GetString(messageData).Trim('\0');

                    return new AdsLogEntry
                    {
                        TimeRaised = timeRaised,
                        AdsPort = senderPort,
                        Sender = sender,
                        Message = message,
                        LogLevel = logLevel             
                    };
                }
            }
        }
    }
}
