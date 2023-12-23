using System;

namespace TcUnit.TestAdapter.Models
{
    public enum AdsLogLevel
    {
        Hint = 1,
        Warning = 2,
        Error = 4,
        Log = 16,
        MessageBox = 32,
        Resource = 64,
        String = 128,
        DefMsgIdx = 256,
        LERROR = 20,
        LRERROR = 84,
        LBERROR = 52,
        LBRERROR = 116
    }

    public class AdsLogEntry
    {
        public DateTime TimeRaised;
        public AdsLogLevel LogLevel;
        public int AdsPort;
        public string Sender;
        public string Message;

        public override string ToString()
        {
            return string.Format("{0} ({1}): {2}", Sender, AdsPort, Message);
        }
    }
}
