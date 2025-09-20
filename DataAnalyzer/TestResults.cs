using System.IO;
using Newtonsoft.Json;

namespace DataAnalyzer;

public class TestResults
{
    public string TestCase = string.Empty;
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

    public static TestResults ReadFromFolder(string outputDirectory)
    {
        string json = File.ReadAllText(Path.Combine(outputDirectory, "TestResults.json"));
        return JsonConvert.DeserializeObject<TestResults>(json) ?? new TestResults();
    }
}