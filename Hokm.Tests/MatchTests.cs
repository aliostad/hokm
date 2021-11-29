using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Hokm.Tests
{
    public class MatchTests
    {
        private readonly ITestOutputHelper _output;
        
        public MatchTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task RunsGameFine()
        {
            var players = PlayerPositions.All.ToDictionary(
                p => p, 
                pp => new PuppetPlayer());

            var match = new Match(new Team()
                {
                    Player1 = players[PlayerPosition.Team1Player1],
                    Player2 = players[PlayerPosition.Team1Player2]
                },
                new Team()
                {
                    Player1 = players[PlayerPosition.Team2Player1],
                    Player2 = players[PlayerPosition.Team2Player2]
                });

            var game = await match.RunGame();
            _output.WriteLine(game.Score.TricksWonByTeam1.ToString());
        }
    }
}