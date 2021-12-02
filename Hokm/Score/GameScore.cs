using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Hokm
{
    public record GameScore
    {
        public int TricksWonByTeam1 => TricksWon[PlayerPosition.Team1Player1] + TricksWon[PlayerPosition.Team1Player2];
        
        public int TricksWonByTeam2 => TricksWon[PlayerPosition.Team2Player1] + TricksWon[PlayerPosition.Team2Player2];
        
        public Dictionary<PlayerPosition, int> TricksWon { get; } = 
            PlayerPositions.All
            .ToDictionary(k => k, v => 0);

        public void RegisterWin(PlayerPosition position)
        {
            TricksWon[position]++;
        }
        
        public bool IsCompleted => TricksWonByTeam1 >= 7 || TricksWonByTeam2 >= 7;
    }
}