using System.Collections.Generic;
using System.Linq;
using CardGame;
using Xunit;

namespace Hokm.Tests
{
    public class GameTests
    {
        [Theory]
        [InlineData(new[] {"A♣"}, Suit.Diamond, 0)]
        [InlineData(new[] {"A♣", "10♣", "2♣", "K♣"}, Suit.Diamond, 0)]
        [InlineData(new[] {"10♣", "A♠", "2♣", "K♣"}, Suit.Diamond, 3)]
        [InlineData(new[] {"A♣", "10♣", "2♦", "K♣"}, Suit.Diamond, 2)]
        
        //"♣", "♦", "♥", "♠"
        public void DecideWinner(IEnumerable<string> cardStrings, Suit trumpSuit, int expectedIndex)
        {
            var cards = cardStrings.Select(x => Card.FromString(x)).ToArray();
            var index = Game.DecideWinnerCard(cards, trumpSuit);
            Assert.Equal(expectedIndex, index);
        }
    }
}