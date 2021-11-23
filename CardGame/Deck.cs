using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGame
{
    public class Deck
    {
        protected Stack<Card> _cards = new Stack<Card>();

        public Deck()
        {
            for (var i = 0; i < 4; i++)
            {
                var suit = (Suit)i;
                for (var j = 1; j < 14; j++)
                {
                    var card = new Card(suit, (Rank)j);
                    _cards.Push(card);
                }
            }
        }

        public Card Peek()
        {
            return _cards.Peek();
        }
        
        public Deck Shuffle()
        {
            _cards = new Stack<Card>(_cards.OrderBy(card => Guid.NewGuid()));
            return this;
        }

        public IEnumerable<Card> Deal(int n)
        {
            for (int i = 0; i < n; i++)
            {
                if (_cards.Count == 0)
                    throw new IndexOutOfRangeException("Trying to deal from an empty deck.");
                
                yield return _cards.Pop();
            }
        }

    }
}