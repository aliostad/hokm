using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CardGame.Tests
{
    public class DeckTests
    {
        private readonly ITestOutputHelper _output;

        public DeckTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void Shuffle_Test()
        {
            var deck = new Deck();
            var c1 = deck.Peek();
            deck.Shuffle();
            var c2 = deck.Peek();
            deck.Shuffle();
            var c3 = deck.Peek();
            deck.Shuffle();
            var c4 = deck.Peek();
            
            Assert.True(c1 != c2 || c2 != c3 || c3 != c4);
        }
        
        [Fact]
        public void Deal_Test()
        {
            var deck = new Deck();
            var hand = deck.Shuffle().Deal(5).ToArray();
            foreach (var card in hand)
            {
                _output.WriteLine(card.ToString());
            }
        }

        
    }
}