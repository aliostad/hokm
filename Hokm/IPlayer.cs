using System;
using System.Collections.Generic;
using CardGame;

namespace Hokm
{
    public interface IPlayer
    {
        Guid Id { get; set; }

        string Name { get; }

        void ReceiveHand(IEnumerable<Card> cards);

        Suit CallTrumpSuit();

        Card Play(IEnumerable<Card> playedByOthers);
    }
}