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

            var g = new Game(Score, Team1, Team2, CurrentTrumpCaller);
            g.BanterUttered += OnBanterUttered;
            g.TrickCompleted += OnTrickCompleted;
            
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

            return g;
        }

        private void OnTrickCompleted(object sender, TrickCompletedEventArgs e)
        {
            
        }

        private void OnBanterUttered(object sender, BanterUtteredEventArgs e)
        {
            
        }
    }
}