using System.Collections.Generic;

namespace Core.Main
{
    public class TestRunFileEntry
    {
        public string TestCase { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
}