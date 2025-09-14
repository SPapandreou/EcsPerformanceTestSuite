using System.Text.Json;
using EcsTestSuiteWrapper;
using UprofWrapper.Config;

const string configPath = "config.json";

if (!File.Exists(configPath))
{
    Console.Error.WriteLine($"Config file not found: {configPath}");
    Environment.Exit(1);
}

AppConfig config;

try
{
    config = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(configPath)) ??
             throw new InvalidOperationException($"Config deserialization returned null: {configPath}");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Failed to parse config file: {configPath}");
    Console.Error.WriteLine(ex.Message);
    Environment.Exit(1);
    throw;
}

var outputDir = args[0];
var pid = int.Parse(args[1]);

var processArgs = $"collect --config data_access -p {pid} -o {outputDir}";

var process = new ProcessWrapper(config.UprofPath, processArgs);
process.Start();

var spinWait = new SpinWait();

while (!Directory.EnumerateDirectories(outputDir).Any())
{
    spinWait.SpinOnce();
}


await Task.Delay(500);

Console.WriteLine("ready");

var stdIn = "";

while (stdIn == null || !stdIn.Contains("stop"))
{
    stdIn = Console.ReadLine();
}

await process.Stop();

Environment.Exit(0);