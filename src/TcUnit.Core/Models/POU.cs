using System;
using System.Collections.Generic;
using System.Text;

namespace TcUnit.TestAdapter.Models
{
    public abstract class POU
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string FilePath { get; set; }
    }
}
