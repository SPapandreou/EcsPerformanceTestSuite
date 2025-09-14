using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Core.Tests
{
    public class TestResults
    {
        public string TestCase;
        public Dictionary<string, object> Parameters = new();
        public Dictionary<string, double> UprofData = new();
        public Dictionary<string, List<double>> TimeSeriesData = new();
        public Dictionary<string, double> KeyValues = new();

        public void WriteToFile(string outputDirectory)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            string path = Path.Combine(outputDirectory, "TestResults.json");
            File.WriteAllText(path, json);
        }
    }
}