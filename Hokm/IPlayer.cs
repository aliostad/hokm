using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CardGame;

namespace Hokm
{
    public interface IPlayer
    {
        Guid Id { get; }

        string Name { get; }

        Task ReceiveHandAsync(IEnumerable<Card> cards);

        Task<Suit> CallTrumpSuitAsync();

        Task<Card> PlayAsync(IEnumerable<Card> playedByOthers);

        Task<string> BanterAsync();

        Task NewGame();
    }
}