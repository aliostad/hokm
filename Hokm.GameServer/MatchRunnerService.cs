using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Hokm.GameServer
{
    public class MatchRunnerService : IHostedService
    {
        private CancellationTokenSource _cancellation = new CancellationTokenSource();

        public bool? IsRunning { get; private set; } = false;
        private MatchRunner _matchRunner = null;
        private IMatchReportHub _reportHub;
        private Task _task;
        
        public MatchRunnerService(IMatchReportHub reportHub)
        {
            _reportHub = reportHub;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            IsRunning = true;
            _task = Task.Run(Work, _cancellation.Token);
            return Task.CompletedTask;
        }

        private async Task Work()
        {
            while (!_cancellation.IsCancellationRequested)
            {
                var mach = new Match(new Team()
                {
                    Id = Guid.NewGuid(),
                    Name = "Savants",
                    Player1 = new DummyPlayer("Akhmet"),
                    Player2 = new DummyPlayer("Subar")
                }, new Team()
                {
                    Id = Guid.NewGuid(),
                    Name = "Rudals",
                    Player1 = new DummyPlayer("Tofat"),
                    Player2 = new DummyPlayer("Shaami")
                });
                
                _matchRunner = new MatchRunner(mach);
                _reportHub.ReportMatch(mach);
                _matchRunner.Start();
                while (!_cancellation.IsCancellationRequested && !_matchRunner.IsCompleted)
                {
                    await Task.Delay(50, _cancellation.Token);
                }
                _reportHub.StopReportingMatch(mach);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            IsRunning = false;
            _cancellation.Cancel();
            _matchRunner.Stop();
            return Task.CompletedTask;
        }
    }
}