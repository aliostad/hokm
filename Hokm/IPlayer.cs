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

        Task<Card> PlayAsync(int trickNumber, IEnumerable<Card> playedByOthers, Suit trumpSuit);

        // they could banter
        Task<string> InformTrickOutcomeAsync(TrickOutcome outcome);
        
        Task<string> BanterAsync();

        Task NewGameAsync(MatchScore currentMatchScore, PlayerPosition caller);

        Task<string> GameFinished(GameOutcome outcome, GameScore currentScore);

        Task NewMatchAsync(IDictionary<PlayerPosition, IPlayerInfo> playerInfos, PlayerPosition yourPosition);

        Task MatchFinished(MatchScore score);
    }

    public static class PlayerExtensions
    {
        public static IPlayerInfo ToInfo(this IPlayer player)
        {
            return new PlayerInfo() { Id = player.Id, Name = player.Name };
        }
    }
}