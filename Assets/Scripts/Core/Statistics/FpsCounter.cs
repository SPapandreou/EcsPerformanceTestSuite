using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core
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
        
        private Stopwatch _stopwatch;
        private long _lastTicks;

        private CancellationTokenSource _cts;

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
                
                TotalTime = (double)currentTicks / Stopwatch.Frequency;
                AverageFps = TotalFrames / TotalTime;
                _lastTicks = currentTicks;
                await UniTask.NextFrame(token);
            }
            
            _stopwatch.Stop();
            TotalTimeSpan = _stopwatch.Elapsed;
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