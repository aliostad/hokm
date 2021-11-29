using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardGame;

namespace Hokm.Tests
{
    public class PuppetPlayer : IPlayer
    {
        public Guid Id { get; } = Guid.NewGuid();
        
        public string Name { get; init; }

        public PlayerPosition Caller { get; set; }
        
        public List<Card> Cards { get; } = new List<Card>();
        
        public IDictionary<PlayerPosition, IPlayerInfo> Infos { get; set; }

        
        public TrickOutcome LastOutcome { get; set; }
        
        public Task ReceiveHandAsync(IEnumerable<Card> cards)
        {
            Cards.AddRange(cards);
            return Task.CompletedTask;
        }

        public Task<Suit> CallTrumpSuitAsync()
        {
            return Task.FromResult(Cards[0].Suit);
        }

        public Task<Card> PlayAsync(int trickNumber, IEnumerable<Card> playedByOthers, Suit trumpSuit)
        {
            var local = playedByOthers.ToArray();
            if (local.Length == 0)
                return Task.FromResult(Cards[5]);

            var sameSuit = Cards.FirstOrDefault(x => x.Suit == local[0].Suit);
            if (sameSuit != null)
                return Task.FromResult<Card>(sameSuit);

            var trump = Cards.FirstOrDefault(x => x.Suit == trumpSuit);
            if (trump != null)
                return Task.FromResult<Card>(trump);

            return Task.FromResult<Card>(Cards[0]);

        }

        public Task<string> InformTrickOutcomeAsync(TrickOutcome outcome)
        {
            LastOutcome = outcome;
            return Task.FromResult<string>("Wat??");
        }

        public Task<string> BanterAsync()
        {
            return Task.FromResult<string>("Wat??");
        }

        public Task NewGame(IDictionary<PlayerPosition, IPlayerInfo> playerInfos, PlayerPosition caller)
        {
            Caller = caller;
            Infos = playerInfos;
            return Task.CompletedTask;
        }
    }
}