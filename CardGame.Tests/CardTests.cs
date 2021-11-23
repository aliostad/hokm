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
    }
}