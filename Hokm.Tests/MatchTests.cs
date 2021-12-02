using System.Linq;
using System.Threading;
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
            var match = HaveYouGotAMatch();
            await match.StartAsync();
            var game = await match.RunGameAsync(new CancellationToken());
            _output.WriteLine(game.Score.TricksWonByTeam1.ToString());
        }
        
        [Fact]
        public async Task Runs5GamesFine()
        {
            var verbose = false;
            var match = HaveYouGotAMatch();
            await match.StartAsync();

            if (verbose)
            {
                match.MatchEvent += (sender, args) =>
                {
                    if (args.EventType == EventType.CardPlayed)
                    {
                        var e = (CardPlayedEventArgs)args.OriginalEventArgs;
                        _output.WriteLine($"{e.PlayerPlayingTheCard} played {e.Cards.Last()}");
                    }
                };
            }
            
            for (int i = 0; i < 5; i++)
            {
                var game = await match.RunGameAsync(new CancellationToken());
                _output.WriteLine(game.Score.TricksWonByTeam1.ToString());                
            }
        }

        
        public static Match HaveYouGotAMatch(ITestOutputHelper output = null)
        {
            var players = PlayerPositions.All.ToDictionary(
                p => p, 
                pp => new PuppetPlayer(output));
            
            return new Match(new Team()
                {
                    Player1 = players[PlayerPosition.Team1Player1],
                    Player2 = players[PlayerPosition.Team1Player2]
                },
                new Team()
                {
                    Player1 = players[PlayerPosition.Team2Player1],
                    Player2 = players[PlayerPosition.Team2Player2]
                });
        }
    }
}