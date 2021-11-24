using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CardGame;

namespace Hokm
{
    public class Game
    {
        public PlayerPosition Caller { get; init; }
        
        private List<PlayerPosition> _playingOrder;

        private Suit _trumpSuit;
        
        public event EventHandler TrickCompleted; 
        
        public Game(Team team1, 
            Team team2, 
            PlayerPosition caller, 
            TimeSpan? delay = null)
        {
            Team1 = team1;
            Team2 = team2;
            Caller = caller;

            _playingOrder = FindPlayingOrder(caller);

        }

        public Team Team1 { get; init; }
        
        public Team Team2 { get; init; }

        
        internal static List<PlayerPosition> FindPlayingOrder(PlayerPosition startingPosition)
        {
            return Enumerable.Range(0, 4)
                .Select(x => (x + (int)startingPosition) % 4)
                .Cast<PlayerPosition>().ToList();
        }
        
        internal IPlayer GetPlayer(PlayerPosition position)
        {
            return position switch
            {
                PlayerPosition.Team1Player1 => Team1.Player1,
                PlayerPosition.Team1Player2 => Team1.Player2,
                PlayerPosition.Team2Player1 => Team2.Player1,
                PlayerPosition.Team2Player2 => Team2.Player2,
                _ => throw new ArgumentException($"Invalid position: {position}")
            };
        }

        protected void OnTrickCompleted(TrickCompletedEventArgs args)
        {
            TrickCompleted?.Invoke(this, args);
        }
        
        public async Task<Suit> DealAsync()
        {
            var trumpCaller = GetPlayer(Caller);
            var deck = new Deck().Shuffle();
            await trumpCaller.ReceiveHandAsync(deck.Deal(5));
            _trumpSuit = await trumpCaller.CallTrumpSuitAsync();
            await GetPlayer(_playingOrder[1]).ReceiveHandAsync(deck.Deal(5));
            await GetPlayer(_playingOrder[2]).ReceiveHandAsync(deck.Deal(5));
            await GetPlayer(_playingOrder[3]).ReceiveHandAsync(deck.Deal(5));

            for (int i = 0; i < 2; i++)
            {
                foreach (var position in _playingOrder)
                {
                    await GetPlayer(position).ReceiveHandAsync(deck.Deal(4));
                }
            }
            
            return _trumpSuit;
        }

        public TrickOutcome PlayTrick(TimeSpan? delay = null)
        {
            var cardsPlayed = new List<Card>();

            throw new NotImplementedException();
        }

        public static int DecideWinnerCard(IEnumerable<Card> cards, Suit trumpSuit)
        {
            var copy = cards.ToList();
            var startingSuit = copy[0].Suit;
            var (cardValue, index) = copy.Select(
                (card, index) => (GetCardValue(card, startingSuit, trumpSuit), index))
                .Max();
            return index;
        }

        private static int GetCardValue(Card card, Suit startingSuit, Suit trumpSuit)
        {
            var value = ((int)card.Rank + 11) % 13;
            if (card.Suit == trumpSuit)
                value += 100;
            if (card.Suit != startingSuit && card.Suit != trumpSuit)
                return 0;
            else
                return value;
        }

    }
}