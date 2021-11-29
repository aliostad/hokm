using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardGame;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Hokm.Tests
{
    public class GameTests
    {
        private readonly ITestOutputHelper _output;
        
        public GameTests(ITestOutputHelper output)
        {
            _output = output;
        }

        //"♣", "♦", "♥", "♠"
        [Theory]
        [InlineData(new[] {"A♣"}, Suit.Diamond, 0)]
        [InlineData(new[] {"A♣", "10♣", "2♣", "K♣"}, Suit.Diamond, 0)]
        [InlineData(new[] {"10♣", "A♠", "2♣", "K♣"}, Suit.Diamond, 3)]
        [InlineData(new[] {"A♣", "10♣", "2♦", "K♣"}, Suit.Diamond, 2)]
        public void DecideWinner(IEnumerable<string> cardStrings, Suit trumpSuit, int expectedIndex)
        {
            var cards = cardStrings.Select(x => Card.FromString(x)).ToArray();
            var index = Game.DecideWinnerCard(cards, trumpSuit);
            Assert.Equal(expectedIndex, index);
        }

        [Fact]
        public async Task RunTrick_Works()
        {
            var players = PlayerPositions.All.ToDictionary(
                p => p, 
                pp => new PuppetPlayer());

            var game = new Game(new MatchScore(),
                new Team()
                {
                    Player1 = players[PlayerPosition.Team1Player1],
                    Player2 = players[PlayerPosition.Team1Player2]
                },
                new Team()
                {
                    Player1 = players[PlayerPosition.Team2Player1],
                    Player2 = players[PlayerPosition.Team2Player2]
                }, PlayerPosition.Team1Player1);

            await game.DealAsync();
            var outcome = await game.PlayTrickAsync();
            _output.WriteLine(string.Join("-", outcome.CardsPlayed.Select(x => x.ToString())));
            _output.WriteLine(outcome.Winner.ToString());
            _output.WriteLine(outcome.TrumpUsage.ToString());
            
            Assert.Equal(outcome, players[PlayerPosition.Team1Player1].LastOutcome);
            Assert.Equal(1, game.Score.TricksWonByTeam1 + game.Score.TricksWonByTeam2);
        }
        
    }
}