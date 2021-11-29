using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CardGame;

namespace Hokm
{
    /// <summary>
    /// A round of Hokm when the hand is dealt and played until one team wins 7 'tricks'
    ///
    ///
    /// Anti-clockwise:
    ///
    /// ---------------------------------------------------
    /// |              |  Team1Player1   |                |
    /// ---------------------------------------------------
    /// | Team2Player1 |                 |  Team2Player2  |
    /// ---------------------------------------------------
    /// |              |  Team1Player2   |                |
    /// ---------------------------------------------------
    ///
    /// 
    /// </summary>
    public class Game
    {
        public PlayerPosition Caller { get; init; }
        
        private Suit _trumpSuit;

        private int _currentTrickNumber = 0;

        private PlayerPosition _currentTrickStarter;
        
        private Dictionary<PlayerPosition, PlayerShadow> _shadows = 
                PlayerPositions.All.ToDictionary(x => x, y => new PlayerShadow());

        private Func<IEnumerable<Card>, IEnumerable<Card>> _suffler;

        public event EventHandler<TrickCompletedEventArgs> TrickCompleted;

        public event EventHandler<BanterUtteredEventArgs> BanterUttered;  
        
        public Game(Team team1, 
            Team team2, 
            PlayerPosition caller,
            Func<IEnumerable<Card>, IEnumerable<Card>> suffler = null,
            TimeSpan? delay = null)
        {
            _suffler = suffler;
            Team1 = team1;
            Team2 = team2;
            Caller = caller;
            _currentTrickStarter = caller;
        }

        public Team Team1 { get; init; }
        
        public Team Team2 { get; init; }

        public GameScore Score { get; } = new GameScore();
        
        internal static List<PlayerPosition> BuildPlayingOrder(PlayerPosition startingPosition)
        {
            return Enumerable.Range(0, 4)
                .Select(x => (x + (int)startingPosition) % 4)
                .Cast<PlayerPosition>().ToList();
        }
        
        // you could implement this in a hashtable but the cost of the execution is negligible either way
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
        
        protected void OnBanterUttered(BanterUtteredEventArgs args)
        {
            BanterUttered?.Invoke(this, args);
        }

        public async Task<Suit> DealAsync()
        {
            var playingOrder = BuildPlayingOrder(Caller);
            var trumpCaller = GetPlayer(Caller);
            var deck = new Deck(_suffler).Shuffle();

            foreach (var position in playingOrder)
            {
                var cards = deck.Deal(5).ToArray();
                var player = GetPlayer(position);
                _shadows[position].ReceiveHand(cards);
                await player.ReceiveHandAsync(cards);
                if (position == Caller)
                {
                    _trumpSuit = await trumpCaller.CallTrumpSuitAsync();
                }
            }

            
            for (int i = 0; i < 2; i++)
            {
                foreach (var position in playingOrder)
                {
                    var cards = deck.Deal(4).ToArray();
                    _shadows[position].ReceiveHand(cards);
                    await GetPlayer(position).ReceiveHandAsync(cards);
                }
            }
            
            return _trumpSuit;
        }

        public async Task<TrickOutcome> PlayTrickAsync(TimeSpan? inBetweenDelay = null)
        {
            _currentTrickNumber++;
            var playingOrder = BuildPlayingOrder(_currentTrickStarter);
            var cardsPlayed = new List<Card>();
            
            foreach (var position in playingOrder)
            {
                var p = GetPlayer(position);
                if (inBetweenDelay.HasValue)
                    await Task.Delay(inBetweenDelay.Value);
                
                var card = await p.PlayAsync(_currentTrickNumber, cardsPlayed, _trumpSuit);
                var result = _shadows[position].ValidateAndPlay(card, cardsPlayed.Count == 0 ? card.Suit : cardsPlayed[0].Suit);
                if (!result.IsValid)
                {
                    throw new InvalidPlayException(result.Error);
                    // TODO
                }
                cardsPlayed.Add(card);
            }

            int index = DecideWinnerCard(cardsPlayed, _trumpSuit);
            _currentTrickStarter = playingOrder[index];
            Score.RegisterWin(_currentTrickStarter);
            var outcome = new TrickOutcome()
            {
                Winner = _currentTrickStarter,
                CardsPlayed = cardsPlayed,
                TrumpUsage = GetUsage(cardsPlayed, _trumpSuit)
            };

            OnTrickCompleted(new TrickCompletedEventArgs(){ Outcome = outcome});
            
            foreach (var position in playingOrder)
            {
                var p = GetPlayer(position);
                var banter = await p.InformTrickOutcomeAsync(outcome);
                if (banter != null) 
                    OnBanterUttered(new BanterUtteredEventArgs() { Banter = banter, PlayerInfo = p});
            }
            
            return outcome;
        }

        internal static TrumpUsage GetUsage(List<Card> cards, Suit trumpSuit)
        {
            if (cards[0].Suit == trumpSuit)
                return TrumpUsage.Mandatory;
            
            return cards.Count(c => c.Suit == trumpSuit) switch
            {
                0 => TrumpUsage.NotUsed,
                1 => TrumpUsage.UsedOnce,
                _ => TrumpUsage.UsedMultiple
            };

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