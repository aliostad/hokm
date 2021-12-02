using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
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
    /// |              |  Team1Player1*  |                |
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
        public PlayerPosition TrumpCaller { get; init; }
        
        private Suit _trumpSuit;

        private int _currentTrickNumber = 0;

        private PlayerPosition _currentTrickStarter;
        
        private Dictionary<PlayerPosition, PlayerShadow> _shadows = 
                PlayerPositions.All.ToDictionary(x => x, y => new PlayerShadow());

        private readonly MatchScore _matchScore;
        private Func<IEnumerable<Card>, IEnumerable<Card>> _shuffler;

        public event EventHandler<TrickFinishedEventArgs> TrickFinished;

        public event EventHandler<BanterUtteredEventArgs> BanterUttered;

        public event EventHandler<GameFinishedEventArgs> GameFinished;

        public event EventHandler<CardPlayedEventArgs> CardPlayed;

        public event EventHandler<CardsDealtEventArgs> CardsDealt;

        public event EventHandler<EventArgs> TrickStarted;


        public Game(
            int gameNumber,
            MatchScore matchScore,
            Team team1, 
            Team team2, 
            PlayerPosition trumpCaller,
            Func<IEnumerable<Card>, IEnumerable<Card>> shuffler = null)
        {
            _matchScore = matchScore;
            _shuffler = shuffler;
            Team1 = team1;
            Team2 = team2;
            TrumpCaller = trumpCaller;
            _currentTrickStarter = trumpCaller;
            GameNumber = gameNumber;
        }

        public Team Team1 { get; init; }
        
        public Team Team2 { get; init; }

        public GameScore Score { get; } = new GameScore();
        
        public int GameNumber { get; init; }
        
        public TrickInfo CurrentTrick { get; private set; }
        
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

        protected void OnTrickFinished(TrickFinishedEventArgs args)
        {
            TrickFinished?.Invoke(this, args);

            Score.RegisterWin(args.Outcome.Winner);
            if (Score.IsCompleted)
                OnGameFinished(new GameFinishedEventArgs() {Game = this});
        }
        
        protected void OnBanterUttered(BanterUtteredEventArgs args)
        {
            BanterUttered?.Invoke(this, args);
        }

        protected void OnGameFinished(GameFinishedEventArgs args)
        {
            GameFinished?.Invoke(this, args);
        }
        
        protected void OnCardPlayed(CardPlayedEventArgs args)
        {
            CardPlayed?.Invoke(this, args);
        }

        protected void OnCardsDealt(CardsDealtEventArgs args)
        {
            CardsDealt?.Invoke(this, args);
        }

        protected void OnTrickStarted(EventArgs args)
        {
            TrickStarted?.Invoke(this, args);
        }

        
        public async Task<Suit?> StarteAndDealAsync(CancellationToken cancellationToken)
        {
            var playingOrder = BuildPlayingOrder(TrumpCaller);
            var trumpCaller = GetPlayer(TrumpCaller);
            var deck = new Deck(_shuffler).Shuffle();

            foreach (var position in playingOrder)
            {
                if (cancellationToken.IsCancellationRequested)
                    return null;
                
                var cards = deck.Deal(5).ToArray();
                var player = GetPlayer(position);
                _shadows[position].ReceiveHand(cards);
                await player.ReceiveHandAsync(cards);
                if (position == TrumpCaller)
                {
                    _trumpSuit = await trumpCaller.CallTrumpSuitAsync();
                }
            }
            
            for (int i = 0; i < 2; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    return null;

                foreach (var position in playingOrder)
                {
                    var cards = deck.Deal(4).ToArray();
                    _shadows[position].ReceiveHand(cards);
                    await GetPlayer(position).ReceiveHandAsync(cards);
                }
            }
            
            OnCardsDealt(new CardsDealtEventArgs()
            {
                TrumpSuit = _trumpSuit,
                Hands = _shadows.ToDictionary(x => x.Key, 
                    kv => kv.Value._handCards.Select(y => y))
            });
            
            return _trumpSuit;
        }

        public async Task<TrickOutcome> PlayTrickAsync(CancellationToken cancellationToken, TimeSpan? inBetweenDelay = null)
        {
            _currentTrickNumber++;
            OnTrickStarted(EventArgs.Empty);
            
            var playingOrder = BuildPlayingOrder(_currentTrickStarter);
            var cardsPlayed = new List<Card>();
            CurrentTrick = new TrickInfo()
            {
                TrickNumber = _currentTrickNumber,
                Starter = _currentTrickStarter
            };
            
            foreach (var position in playingOrder)
            {
                if (cancellationToken.IsCancellationRequested)
                    return null;
                
                var p = GetPlayer(position);

                var card = await p.PlayAsync(_currentTrickNumber, cardsPlayed, _trumpSuit);
                var result = _shadows[position].ValidateAndPlay(card, cardsPlayed.Count == 0 ? card.Suit : cardsPlayed[0].Suit);
                if (!result.IsValid)
                {
                    throw new InvalidPlayException($"{position}: {result.Error}");
                    // TODO: alternative to raising exception is to play for the player
                }
                cardsPlayed.Add(card);
                CurrentTrick.CardsPlayed = cardsPlayed.ToArray();
                CurrentTrick.CurrentWinningPosition = playingOrder[DecideWinnerCard(cardsPlayed, _trumpSuit)];
                
                OnCardPlayed(new CardPlayedEventArgs()
                {
                    Cards = cardsPlayed.ToArray(), 
                    GameNumber = GameNumber, 
                    StarterPlayer = playingOrder[0], 
                    TrickNumber = _currentTrickNumber,
                    TrumpSuit = _trumpSuit,
                    PlayerPlayingTheCard = position
                });
                
                if (inBetweenDelay.HasValue)
                    await Task.Delay(inBetweenDelay.Value, cancellationToken);
            }

            int index = DecideWinnerCard(cardsPlayed, _trumpSuit);
            _currentTrickStarter = playingOrder[index];
            var outcome = new TrickOutcome()
            {
                Winner = _currentTrickStarter,
                CardsPlayed = cardsPlayed,
                TrumpUsage = GetUsage(cardsPlayed, _trumpSuit)
            };

            OnTrickFinished(new TrickFinishedEventArgs(){ Outcome = outcome});
            
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

        public GameInfo ToInfo()
        {
            return new GameInfo()
            {
                Score = Score,
                GameNumber = GameNumber,
                TrumpCaller = TrumpCaller,
                CurrentTrick = CurrentTrick
            };
        }

    }
}