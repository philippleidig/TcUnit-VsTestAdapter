using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TcUnit.TestAdapter
{
    public class TestSettings : TestRunSettings
    {
        public TestSettings() : base("TcUnit")
        {
            
        }
        public override XmlElement ToXml()
        {
            throw new NotImplementedException();
        }
    }
}
