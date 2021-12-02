using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hokm
{
    public class Match
    {
        public Team Team1 { get; init; }

        public Team Team2 { get; init; }

        public MatchScore Score { get; init; }

        public PlayerPosition CurrentTrumpCaller { get; private set; }

        public int CurrentGameNumber { get; private set; }

        public event EventHandler<MatchEventArgs> MatchEvent;

        private Dictionary<PlayerPosition, IPlayer> _players = new Dictionary<PlayerPosition, IPlayer>();

        private Game _currentGame = null;


        public Match(Team team1,
            Team team2,
            int bestOf = 13)
        {
            Team1 = team1;
            Team2 = team2;
            Score = new MatchScore(bestOf);
            CurrentTrumpCaller = PlayerPositions.All.OrderBy(x => Guid.NewGuid()).First();
            _players[PlayerPosition.Team1Player1] = team1.Player1;
            _players[PlayerPosition.Team1Player2] = team1.Player2;
            _players[PlayerPosition.Team2Player1] = team2.Player1;
            _players[PlayerPosition.Team2Player2] = team2.Player2;
        }

        public async Task StartAsync()
        {
            var infos = _players.ToDictionary(x => x.Key, y => y.Value.ToInfo());

            foreach (var kv in _players)
            {
                await kv.Value.NewMatchAsync(infos, kv.Key);
            }
        }

        public async Task<Game> RunGameAsync(CancellationToken cancellationToken, TimeSpan? inBetweenDelay = null)
        {
            if (Score.IsCompleted)
                throw new InvalidOperationException("Match is finished, game cannot started");

            CurrentGameNumber++;
            var g = new Game(CurrentGameNumber, Score, Team1, Team2, CurrentTrumpCaller);
            _currentGame = g;
            g.BanterUttered += OnBanterUttered;
            g.TrickFinished += OnTrickFinished;
            g.CardPlayed += OnCardPlayed;
            g.CardsDealt += OnCardsDealt;
            g.TrickStarted += OnTrickStarted;
            
            RaiseEvent(EventType.GameStarted, null);

            // tell players the game is about to start
            foreach (var player in _players.Values)
            {
                await player.NewGameAsync(Score, CurrentTrumpCaller);
            }

            var trumpSuit = await g.StarteAndDealAsync(cancellationToken);

            if (trumpSuit.HasValue)
            {
                while ( !g.Score.IsCompleted)
                {
                    await g.PlayTrickAsync(cancellationToken, inBetweenDelay);
                    if (inBetweenDelay.HasValue)
                        await Task.Delay(inBetweenDelay.Value, cancellationToken);
                }

                foreach (var kv in g.Score.TricksWon)
                    Score.TricksWon[kv.Key] += kv.Value;

                var team1Won = g.Score.TricksWonByTeam1 > g.Score.TricksWonByTeam2;
                Score.RegisterGameWin(g.Score, CurrentTrumpCaller);
                var team1WasCaller = PlayerPositions.IsTeam1(CurrentTrumpCaller);
            
                if (team1Won ^ team1WasCaller) // if not the caller team has won, move forward for TrumpCaller
                    CurrentTrumpCaller = (PlayerPosition)(((int)CurrentTrumpCaller + 1) % 4);
            }
            
            g.BanterUttered -= OnBanterUttered;
            g.TrickFinished -= OnTrickFinished;
            g.CardPlayed -= OnCardPlayed;
            g.CardsDealt -= OnCardsDealt;
            g.TrickStarted -= OnTrickStarted;
            
            _currentGame = null;
            
            RaiseEvent(EventType.GameFinished, null);
            
            return g;
        }

        private void OnTrickStarted(object sender, EventArgs e)
        {
            RaiseEvent(EventType.TrickStarted, e);
        }

        private void OnCardsDealt(object sender, CardsDealtEventArgs e)
        {
            RaiseEvent(EventType.CardsDealt, e);
        }

        private void OnCardPlayed(object sender, CardPlayedEventArgs e)
        {
            RaiseEvent(EventType.CardPlayed, e);
        }

        private void OnTrickFinished(object sender, TrickFinishedEventArgs e)
        {
            RaiseEvent(EventType.TrickFinished, e);
        }

        private void OnBanterUttered(object sender, BanterUtteredEventArgs e)
        {
            
        }

        private void RaiseEvent(EventType eventType, EventArgs args)
        {
            OnMatchEvent(new MatchEventArgs()
            {
                Info = this.ToInfo(),
                EventType = eventType,
                OriginalEventArgs = args
            });            
        }
        
        protected void OnMatchEvent(MatchEventArgs args)
        {
            MatchEvent?.Invoke(this, args);
        }
        
        public MatchInfo ToInfo()
        {
            return new MatchInfo()
            {
                Score = Score,
                Team1 = Team1.ToInfo(),
                Team2 = Team2.ToInfo(),
                CurrentGame = _currentGame?.ToInfo()
            };
        }
    }
}