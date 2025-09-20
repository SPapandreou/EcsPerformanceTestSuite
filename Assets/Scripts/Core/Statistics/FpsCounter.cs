using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Core.TestHud;
using Cysharp.Threading.Tasks;

namespace Core.Statistics
{
    public class FpsCounter : IDisposable
    {

        public int TotalFrames { get; private set; } = 0;
        public double CurrentFps { get; private set; } = 0;
        public double AverageFps { get; private set; } = 0;
        public double MinFps { get; private set; } = -1;
        public double MaxFps { get; private set; } = 0;
        public double TotalTime { get; private set; } = 0;
        public TimeSpan TotalTimeSpan { get; private set; }

        private readonly TestHudLogic _testHudLogic;
        
        private Stopwatch _stopwatch;
        private long _lastTicks;

        private readonly double[] _fpsBuffer = new double[300000];
        private int _lastUpdateFrames;
        private double _accumulatedFrameTime;
        private double _updateTime;

        private CancellationTokenSource _cts;

        public FpsCounter(TestHudLogic testHudLogic)
        {
            _testHudLogic = testHudLogic;
        }

        public void Start()
        {
            Stop();
            _cts = new CancellationTokenSource();

            TotalFrames = 0;
            CurrentFps = 0;
            AverageFps = 0;
            MinFps = -1;
            MaxFps = 0;
            TotalTime = 0;
            
            _stopwatch ??= new Stopwatch();
            _stopwatch.Stop();
            _stopwatch.Reset();
            
            _testHudLogic.SetFpsEnabled(true);
            
            RunFpsCounter(_cts.Token).Forget();
        }

        private async UniTask RunFpsCounter(CancellationToken token)
        {
            if (!_stopwatch.IsRunning)
            {
                _stopwatch.Start();
                await UniTask.NextFrame(token);
            }
            while (!token.IsCancellationRequested)
            {
                TotalFrames++;
                var currentTicks = _stopwatch.ElapsedTicks;
                var deltaTime = (double)(currentTicks - _lastTicks) / Stopwatch.Frequency;
                CurrentFps = 1 / deltaTime;

                if (MinFps < 0)
                {
                    MinFps = CurrentFps;
                }
                else
                {
                    MinFps = CurrentFps < MinFps ? CurrentFps : MinFps;
                }
                
                MaxFps = CurrentFps > MaxFps ? CurrentFps : MaxFps;

                var lastTime = TotalTime;
                TotalTime = (double)currentTicks / Stopwatch.Frequency;
                AverageFps = TotalFrames / TotalTime;
                _lastTicks = currentTicks;
                
                _lastUpdateFrames++;
                _updateTime += TotalTime - lastTime;
                _accumulatedFrameTime += CurrentFps;

                if (TotalFrames == 1)
                {
                    _testHudLogic.SetFps(CurrentFps);
                }
                
                if (_updateTime >= 0.2)
                {
                    _testHudLogic.SetFps(_accumulatedFrameTime / _lastUpdateFrames);
                    _updateTime = 0;
                    _lastUpdateFrames = 0;
                    _accumulatedFrameTime = 0;
                }
                
                _fpsBuffer[TotalFrames - 1] = CurrentFps;
                await UniTask.NextFrame(token);
            }
            
            _stopwatch.Stop();
            TotalTimeSpan = _stopwatch.Elapsed;
        }

        public List<double> GetFpsTimeSeries()
        {
            return _fpsBuffer[..TotalFrames].ToList();
        }

        public void Stop()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}