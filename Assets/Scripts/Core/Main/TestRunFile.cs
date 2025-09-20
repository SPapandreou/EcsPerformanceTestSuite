using System.Collections.Generic;
using System.IO;
using Core.Tests;
using Newtonsoft.Json;

namespace Core.Main
{
    public class TestRunFile
    {
        public List<TestRunFileEntry> TestRuns { get; set; } = new();

        public void Add(TestCase testCase)
        {
            TestRuns.Add(testCase.GetTestRunFileEntry());
        }

        public void WriteToFile(string path)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json);
        }
        
        public static TestRunFile Load(string path)
        {
            return JsonConvert.DeserializeObject<TestRunFile>(File.ReadAllText(path));
        }
        
    }
}