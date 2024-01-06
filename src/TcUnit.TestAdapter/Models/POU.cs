using System;

namespace TcUnit.TestAdapter.Models
{
    public abstract class POU
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public string FilePath { get; set; }
        public string Declaration { get; set; }
        public StructuredTextImplementation Implementation { get; set; }
    }
}
