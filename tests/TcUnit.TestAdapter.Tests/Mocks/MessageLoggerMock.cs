using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcUnit.TestAdapter.Tests.Mocks
{
    internal class MessageLoggerMock : IMessageLogger
    {
        public List<string> Errors = new List<string>();
        public List<string> Warnings = new List<string>();
        public List<string> Informations = new List<string>();

        public void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            switch(testMessageLevel)
            {
                case TestMessageLevel.Informational:
                    Informations.Add(message);
                    break;

                case TestMessageLevel.Warning:
                    Warnings.Add(message);
                    break;

                case TestMessageLevel.Error:
                    Errors.Add(message);
                    break;
            }
        }
    }
}
