using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CardGame;

namespace Hokm.GameServer
{
    public class DummyPlayer : IPlayer
    {
        public DummyPlayer(string name)
        {
            Name = name;
        }
      
        public Guid Id { get; } = Guid.NewGuid();
        
        public string Name { get; init; }

        public PlayerPosition CurrentCaller { get; set; }
        
        public List<Card> Cards { get; } = new List<Card>();
        
        public IDictionary<PlayerPosition, IPlayerInfo> Infos { get; set; }
        
        public PlayerPosition MyPosition { get; set; }
        
        public TrickOutcome LastOutcome { get; set; }
        
        public Task ReceiveHandAsync(IEnumerable<Card> cards)
        {
            Cards.AddRange(cards);

            if (Cards.Count != 5 && Cards.Count != 9 && Cards.Count != 13)
                throw new InvalidOperationException($"The puppet has {Cards.Count}!");

            return Task.CompletedTask;
        }

        public Task<Suit> CallTrumpSuitAsync()
        {
            return Task.FromResult(Cards[0].Suit);
        }

        public async Task<Card> PlayAsync(int trickNumber, IEnumerable<Card> playedByOthers, Suit trumpSuit)
        {
            var card = await InternalPlayAsync(trickNumber, playedByOthers, trumpSuit);
            Cards.Remove(card);
            return card;
        }

        private Task<Card> InternalPlayAsync(int trickNumber, IEnumerable<Card> playedByOthers, Suit trumpSuit)
        {
            var local = playedByOthers.ToArray();
            if (local.Length == 0)
                return Task.FromResult(Cards.OrderBy(x => Guid.NewGuid()).First());

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

        public Task NewGameAsync(MatchScore currentMatchScore, PlayerPosition caller)
        {
            Cards.Clear();
            CurrentCaller = caller;
            return Task.CompletedTask;
        }

        public Task<string> GameFinished(GameOutcome outcome, GameScore currentScore)
        {
            
            return Task.FromResult<string>("No luck!");
        }

        public Task NewMatchAsync(IDictionary<PlayerPosition, IPlayerInfo> playerInfos, PlayerPosition yourPosition)
        {
            Infos = playerInfos;
            MyPosition = yourPosition;
            return Task.CompletedTask;
        }

        public Task MatchFinished(MatchScore score)
        {
            return Task.CompletedTask;
        }
    }

}