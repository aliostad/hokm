using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace Hokm
{
    public class MatchScore
    {
        public MatchScore(int bestOf = 15)
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
        
        public int GamesWonByTeam1 { get; private set; } = 0;
        
        public int GamesWonByTeam2 { get; private set; } = 0;

        public int BestOf { get; init; }
        
        
        public bool IsCompleted =>  GamesWonByTeam1 >= _limit && GamesWonByTeam2 >= _limit;
        
        
        public void RegisterGameWin(bool isTeam1)
        {
            if (isTeam1)
                GamesWonByTeam1++;
            else
                GamesWonByTeam2++;
        }

    }
}