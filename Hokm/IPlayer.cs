using System;
using System.Collections.Generic;
using CardGame;

namespace Hokm
{
    public interface IPlayer
    {
        Guid Id { get; }

        string Name { get; }

        void ReceiveHand(IEnumerable<Card> cards);

        Suit CallTrumpSuit();

        Card Play(IEnumerable<Card> playedByOthers);

        string Banter();

        void NewGame();
    }
}