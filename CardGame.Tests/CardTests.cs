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
        
    }
}