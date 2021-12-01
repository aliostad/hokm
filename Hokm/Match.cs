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

        public event EventHandler<GameStartedEventArgs> GameStarted; 

        public event EventHandler<GameFinishedEventArgs> GameFinished;

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
            g.TrickCompleted += OnTrickCompleted;
            
            OnGameStarted(new GameStartedEventArgs() {Game = g});
            
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
            g.TrickCompleted -= OnTrickCompleted;

            OnGameFinished(new GameFinishedEventArgs() {Game = g});
            _currentGame = null;
            
            return g;
        }

        private void OnTrickCompleted(object sender, TrickCompletedEventArgs e)
        {
            
        }

        private void OnBanterUttered(object sender, BanterUtteredEventArgs e)
        {
            
        }

        protected void OnGameStarted(GameStartedEventArgs args)
        {
            GameStarted?.Invoke(this, args);
        }

        protected void OnGameFinished(GameFinishedEventArgs args)
        {
            GameFinished?.Invoke(this, args);
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