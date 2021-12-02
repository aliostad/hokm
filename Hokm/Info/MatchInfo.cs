namespace Hokm
{
    public class MatchInfo
    {
        public MatchScore Score { get; set; }
        
        public TeamInfo Team1 { get; set; }

        public TeamInfo Team2 { get; set; }
        
        public GameInfo CurrentGame { get; set; }
    }
}