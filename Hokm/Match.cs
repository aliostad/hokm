using System;
using System.Linq;
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


        private Game _currentGame = null;
        
        public Match(Team team1, 
            Team team2,
            int bestOf = 13)
        {
            Team1 = team1;
            Team2 = team2;
            Score = new MatchScore(bestOf);
            CurrentTrumpCaller = PlayerPositions.All.OrderBy(x => new Guid()).First();
        }
        
        public async Task<Game> RunGame(TimeSpan? inBetweenDelay = null)
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
            
            RaiseEvent(EventType.GameStarted);

            await g.DealAsync();

            while (!g.Score.IsCompleted)
            {
                await g.PlayTrickAsync(inBetweenDelay);
                if (inBetweenDelay.HasValue)
                    await Task.Delay(inBetweenDelay.Value);
            }

            foreach (var kv in g.Score.TricksWon)
                Score.TricksWon[kv.Key] += kv.Value;

            var team1Won = g.Score.TricksWonByTeam1 > g.Score.TricksWonByTeam2;
            Score.RegisterGameWin(g.Score, CurrentTrumpCaller);
            var team1WasCaller = PlayerPositions.IsTeam1(CurrentTrumpCaller);
            
            if (team1Won ^ team1WasCaller) // if not the caller team has won, move forward for TrumpCaller
                CurrentTrumpCaller = (PlayerPosition)(((int)CurrentTrumpCaller + 1) % 4);
            
            g.BanterUttered -= OnBanterUttered;
            g.TrickFinished -= OnTrickFinished;
            
            _currentGame = null;
            
            RaiseEvent(EventType.GameFinished);
            
            return g;
        }

        private void OnTrickStarted(object sender, EventArgs e)
        {
            RaiseEvent(EventType.TrickStarted);
        }

        private void OnCardsDealt(object sender, CardsDealtEventArgs e)
        {
            
        }

        private void OnCardPlayed(object sender, CardPlayedEventArgs e)
        {
            RaiseEvent(EventType.CardPlayed);
        }

        private void OnTrickFinished(object sender, TrickFinishedEventArgs e)
        {
            RaiseEvent(EventType.TrickFinished);
        }

        private void OnBanterUttered(object sender, BanterUtteredEventArgs e)
        {
            
        }

        private void RaiseEvent(EventType eventType)
        {
            OnMatchEvent(new MatchEventArgs()
            {
                Info = this.ToInfo(),
                EventType = eventType
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