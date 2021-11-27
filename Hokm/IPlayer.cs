using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CardGame;

namespace Hokm
{
    public interface IPlayer : IPlayerInfo
    {
        Task ReceiveHandAsync(IEnumerable<Card> cards);

        Task<Suit> CallTrumpSuitAsync();

        Task<Card> PlayAsync(int trickNumber, IEnumerable<Card> playedByOthers);

        Task InformTrickOutcomeAsync(TrickOutcome outcome);
        
        Task<string> BanterAsync();

        Task NewGame(IDictionary<PlayerPosition, IPlayerInfo> playerInfos);
    }
}