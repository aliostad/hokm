using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hokm
{
    public class MatchRunner
    {

        private Task _task;
        private CancellationTokenSource _cancellation = new CancellationTokenSource();
        private TimeSpan? _inBetweenDelay;
        private Match _match;

        public bool? IsRunning { get; private set; } = false;

        public bool IsCompleted { get; private set; } = false;

        public MatchRunner(Match match, TimeSpan? inBetweenDelay = null)
        {
            _match = match;
            _inBetweenDelay = inBetweenDelay;
        }
        
        public void Start()
        {
            if (IsRunning.HasValue && IsRunning.Value)
                throw new InvalidOperationException("Already running.");

            if (!IsRunning.HasValue)
                throw new InvalidOperationException("A match runner can be ever started once.");
            IsRunning = true;

            _task = Task.Run(Work);
        }

        private async Task Work()
        {
            while (!_cancellation.IsCancellationRequested && !_match.Score.IsCompleted)
            {
                await _match.RunGameAsync(_cancellation.Token, _inBetweenDelay);
                if (_inBetweenDelay.HasValue)
                    await Task.Delay(_inBetweenDelay.Value, _cancellation.Token);
            }

            IsCompleted = true;
            IsRunning = null;
        }
        
        public void Stop()
        {
            IsRunning = null;
            _cancellation.Cancel();
        }
    }
}