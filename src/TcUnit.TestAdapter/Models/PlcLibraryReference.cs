﻿using System.Collections.Generic;

namespace TcUnit.TestAdapter.Models
{
    public class PlcLibraryReference
    {
        public string Name { get; set; }

        public Dictionary<string, string> Parameters = new Dictionary<string, string>();
    }
}
