using System.Transactions;

namespace Hokm
{
    public record GameScore
    {
        public int TricksWonByTeam1 { get; set; }
        
        public int TricksWonByTeam2 { get; set; }

        public bool IsCompleted => TricksWonByTeam1 >= 7 || TricksWonByTeam2 >= 7;
    }
}