using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TcUnit.TestAdapter
{
    [SettingsName("TcUnit")]
    public class SettingsProvider : ISettingsProvider
    {
        public void Load(XmlReader reader)
        {
            
        }
    }
}
