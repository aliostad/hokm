using System;
using System.Collections.Generic;
using CardGame;

namespace Hokm
{
    public class PlayerShadow
    {
        private HashSet<Card> _dealtCards = new HashSet<Card>();
        private HashSet<Card> _playedCards = new HashSet<Card>();
        
        public void ReceiveHand(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                if (_dealtCards.Contains(card))
                    throw new InvalidOperationException($"Card {card} already exists.");
                _dealtCards.Add(card);
            }
        }

        public ValidationResult ValidateAndPlay(Card card)
        {
            if (_playedCards.Contains(card))
                return ValidationResult.ErrorResult($"Card {card} already played.");

            if (!_dealtCards.Contains(card))
                return ValidationResult.ErrorResult($"You cannot play a Card {card} you do NOT have.");

            _playedCards.Add(card);
            _dealtCards.Remove(card);
            return ValidationResult.Valid;
        }
        
    }
}