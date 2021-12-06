using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Hokm.Tests
{
    public class MatchRunnerTests
    {
        private readonly ITestOutputHelper _output;

        public MatchRunnerTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public async Task Normalla_Runs_fine()
        {
            var match = MatchTests.HaveYouGotAMatch();
            var c = new CancellationToken();
            while (!match.Score.IsCompleted)
            {
                var game = await match.RunGameAsync(c);
                _output.WriteLine($"Team1: {match.Score.Team1Points}, Team2: {match.Score.Team2Points} ({game.TrumpCaller})");
            }
        }

        [Fact]
        public async Task Using_TaskRunner()
        {
            var m = MatchTests.HaveYouGotAMatch();
            var mr = new MatchRunner(m, TimeSpan.FromMilliseconds(5));
            m.MatchEvent += (sender, args) =>
            {
                if (args.EventType == EventType.GameFinished)
                {
                    _output.WriteLine($"Team1: {m.Score.Team1Points}, Team2: {m.Score.Team2Points}");
                }
            };

            mr.Start();
            
            while (!mr.IsCompleted)
            {
                Thread.Sleep(50);
            }
            
            _output.WriteLine("finished!");
        }
        
        
    }
}