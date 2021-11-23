using System;
using Xunit;

namespace CardGame.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void ToString_Must_Return_Name()
        {
            var c = new Card(Suit.Diamond, Rank.Jack);
            Assert.Equal("J♦", c.ToString());
        }
        
        [Fact]
        public void Equality()
        {
            var c1 = new Card(Suit.Diamond, Rank.Jack);
            var c2 = new Card(Suit.Diamond, Rank.Jack);
            var c3 = new Card(Suit.Heart, Rank.Jack);
            var c4 = new Card(Suit.Diamond, Rank.Queen);
            Assert.Equal(c1, c2);
            Assert.NotEqual(c1, c3);
            Assert.NotEqual(c1, c4);
            Assert.True(c1.Equals(c2));
        }

        [Theory]
        [InlineData("J♣", Suit.Club, Rank.Jack)]
        [InlineData("10♣", Suit.Club, Rank.Ten)]
        [InlineData("2♥", Suit.Heart, Rank.Two)]
        [InlineData("K♦", Suit.Diamond, Rank.King)]
        [InlineData("6♠", Suit.Spade, Rank.Six)]
        public void FromStringWorks(string s, Suit suit, Rank rank)
        {
            Assert.Equal(new Card(suit, rank), Card.FromString(s));    
        }
        
    }
}