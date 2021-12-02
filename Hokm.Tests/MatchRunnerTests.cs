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
        public async Task Runs_fine()
        {
            var match = MatchTests.HaveYouGotAMatch();
            var c = new CancellationToken();
            while (!match.Score.IsCompleted)
            {
                var game = await match.RunGameAsync(c);
                _output.WriteLine($"Team1: {match.Score.Team1Points}, Team2: {match.Score.Team2Points} ({game.TrumpCaller})");
            }
        }
    }
}