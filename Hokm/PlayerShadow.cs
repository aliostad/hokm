using System;
using System.Collections.Generic;
using System.Linq;
using CardGame;

namespace Hokm
{
    public class PlayerShadow
    {
        internal HashSet<Card> _handCards = new HashSet<Card>();
        private HashSet<Card> _playedCards = new HashSet<Card>();
        
        public void ReceiveHand(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                if (_handCards.Contains(card))
                    throw new InvalidOperationException($"Card {card} already exists.");
                _handCards.Add(card);
            }

            if (_handCards.Count != 5 && _handCards.Count != 9 && _handCards.Count != 13)
                throw new InvalidOperationException($"The shadow has {_handCards.Count}!");
        }

        public ValidationResult ValidateAndPlay(Card card, Suit playedSuit)
        {
            if (_playedCards.Contains(card))
                return ValidationResult.ErrorResult($"Card {card} already played.");

            if (!_handCards.Contains(card))
                return ValidationResult.ErrorResult($"You cannot play a Card {card} you do NOT have.");

            if (card.Suit != playedSuit && _handCards.Any(x =>x.Suit == playedSuit))
                return ValidationResult.ErrorResult($"You have cards of {playedSuit}, you must play them.");
            
            _playedCards.Add(card);
            _handCards.Remove(card);
            return ValidationResult.Valid;
        }
        
    }
}