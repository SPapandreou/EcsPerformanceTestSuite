using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Core.Configuration;
using Core.Tests;
using Core.uProf;
using Cysharp.Threading.Tasks;
using Debug = UnityEngine.Debug;
using R3;

namespace Core.Uprof
{
    public sealed class NativeUprofWrapper : IUprofWrapper
    {
        private readonly AppConfig _config;
        private NativeProcess _process;
        private UniTaskCompletionSource _readyTcs;
        private bool _disposed;
        private bool _isRunning;

        public NativeUprofWrapper(AppConfig config)
        {
            _config = config;
        }

        private void ClearTempFiles()
        {
            if (Directory.Exists(_config.UprofTemp))
            {
                Directory.Delete(_config.UprofTemp, true);
            }

            Directory.CreateDirectory(_config.UprofTemp);
        }

        private void CreateProcess()
        {
            _process?.Dispose();
            _readyTcs = new UniTaskCompletionSource();
            _process = new NativeProcess(_config.UprofWrapperPath,
                $"\"{_config.UprofTemp}\" {Process.GetCurrentProcess().Id}",
                $"{Path.GetDirectoryName(_config.UprofWrapperPath)}");

            _process.StdOut.Subscribe(x=>
            {
                if (x.Contains("ready"))
                {
                    _readyTcs.TrySetResult();
                }
                Debug.Log(x);
            });
            _process.StdErr.Subscribe(Debug.Log);

            _process.Start();
        }

        public async UniTask StartProfiling()
        {
            if (!_config.UprofEnable) return;

            if (_isRunning) return;
            
            ClearTempFiles();
            CreateProcess();
            
            await _readyTcs.Task;
            await UniTask.SwitchToMainThread();

            _isRunning = true;
        }

        public async UniTask StopProfiling(string outputDirectory, TestResults testResults)
        {
            if (!_isRunning) return;

            await _process.WriteLineAsync("stop");

            await UniTask.SwitchToThreadPool();
            _process.WaitForExit();
            await UniTask.SwitchToMainThread();
            
            var sourceDirectory = Directory.EnumerateDirectories(_config.UprofTemp).First();
            var targetDirectory = Path.Combine(outputDirectory, "UprofOutput");

            Directory.Move(sourceDirectory, targetDirectory);

            Directory.Delete(_config.UprofTemp, true);
            _process.Dispose();
            _process = null;

            var reportProcess =
                new NativeProcess(_config.UprofBinaryPath, $"report -i {targetDirectory}", targetDirectory);
            reportProcess.StdOut.Subscribe(Debug.Log);
            reportProcess.StdErr.Subscribe(Debug.Log);
            reportProcess.Start();
            
            await UniTask.SwitchToThreadPool();
            reportProcess.WaitForExit();
            await UniTask.SwitchToMainThread();
            reportProcess.Dispose();

            ParseUprofReport(targetDirectory, testResults);
        }

        public void Dispose()
        {
            _process?.Dispose();
        }
        
        private void ParseUprofReport(string directory, TestResults testResults)
        {
            var lines = File.ReadAllLines(Path.Combine(directory, "report.csv"));

            var durationIndex = Array.FindIndex(lines, l => l.StartsWith("Profile Duration:"));
            if (durationIndex < 0)
            {
                throw new Exception("Duration line not found.");
            }
            
            var durationLine = lines[durationIndex].Split(',');
            testResults.UprofData["Duration"] = double.Parse(durationLine[1].Replace("\"", "").Replace(" seconds", ""));
            
            var headerIndex = Array.FindIndex(lines, l => l.StartsWith("\"10 HOTTEST PROCESSES"));
            if (headerIndex < 0 || headerIndex + 2 >= lines.Length)
                throw new Exception("Process metrics section not found.");

            var dataLine = lines[headerIndex + 2];
            var fields = dataLine.Split(',');
            
            testResults.UprofData["CyclesNotInHalt"] = double.Parse(fields[1], CultureInfo.InvariantCulture);
            testResults.UprofData["RetiredInstructions"] = double.Parse(fields[2], CultureInfo.InvariantCulture);
            testResults.UprofData["Cpi"] = double.Parse(fields[3], CultureInfo.InvariantCulture);
            testResults.UprofData["L1DcAccesses"] = double.Parse(fields[4], CultureInfo.InvariantCulture);
            testResults.UprofData["L1DcMissPercent"] = double.Parse(fields[5], CultureInfo.InvariantCulture);
            testResults.UprofData["L1RefillsDr"] = double.Parse(fields[6], CultureInfo.InvariantCulture);
            testResults.UprofData["L1RefillsCache"] = double.Parse(fields[7], CultureInfo.InvariantCulture);
            testResults.UprofData["L1RefillsL2"] = double.Parse(fields[8], CultureInfo.InvariantCulture);
            testResults.UprofData["PercentL1Dr"] = double.Parse(fields[9], CultureInfo.InvariantCulture);
            testResults.UprofData["PercentL1Cache"] = double.Parse(fields[10], CultureInfo.InvariantCulture);
            testResults.UprofData["PercentL1L2"] = double.Parse(fields[11], CultureInfo.InvariantCulture);
            testResults.UprofData["MisalignedLoads"] = double.Parse(fields[12], CultureInfo.InvariantCulture);
            testResults.UprofData["L1DtLbMisses"] = double.Parse(fields[13], CultureInfo.InvariantCulture);
            testResults.UprofData["L2DtLbMisses"] = double.Parse(fields[14], CultureInfo.InvariantCulture);
            testResults.UprofData["L1DcAccessRate"] = double.Parse(fields[15], CultureInfo.InvariantCulture);
            testResults.UprofData["L1DcMissRate"] = double.Parse(fields[16], CultureInfo.InvariantCulture);
            testResults.UprofData["L1DcMissRatio"] = double.Parse(fields[17], CultureInfo.InvariantCulture);
            testResults.UprofData["L1DtLbMissRate"] = double.Parse(fields[18], CultureInfo.InvariantCulture);
            testResults.UprofData["L2DtLbMissRate"] = double.Parse(fields[19], CultureInfo.InvariantCulture);
        }
    }
}