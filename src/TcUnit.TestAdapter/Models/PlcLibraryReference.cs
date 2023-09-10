using System;
using System.Collections.Generic;
using System.Text;

namespace TcUnit.TestAdapter.Models
{
    public class PlcLibraryReference
    {
        public string Name { get; set; }

        public Dictionary<string, string> Parameters = new Dictionary<string, string>();
    }
}
