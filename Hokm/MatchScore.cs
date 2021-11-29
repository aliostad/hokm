using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace Hokm
{
    public class MatchScore
    {
        public MatchScore(int bestOf = 13)
        {
            BestOf = bestOf;
            if (bestOf % 2 == 0)
                throw new ArgumentException("bestOf must be odd.");
            _limit = (bestOf + 1) / 2;
        }
        
        public Dictionary<PlayerPosition, int> TricksWon { get; } = 
            PlayerPositions.All
                .ToDictionary(k => k, v => 0);

        private int _limit;
        
        public int Team1Points { get; private set; } = 0;
        
        public int Team2Points { get; private set; } = 0;

        public int BestOf { get; init; }
        
        
        public bool IsCompleted =>  Team1Points >= _limit && Team2Points >= _limit;
        
        
        public void RegisterGameWin(GameScore score, PlayerPosition caller)
        {
            var point = 1;
            
            // was kot
            if (score.TricksWonByTeam1 == 0 || score.TricksWonByTeam2 == 0)
                point++;
            
            // was trump called kot
            var team1Won = score.TricksWonByTeam1 > score.TricksWonByTeam2;
            var team1WasCaller = PlayerPositions.IsTeam1(caller);

            if (point > 1 && (team1Won ^ team1WasCaller))
                point++;

            if (team1Won)
                Team1Points += point;
            else
                Team2Points += point;

        } 
    }
}