namespace Hokm
{
    public class GameInfo
    {
        public int GameNumber { get; set; }
        
        public GameScore Score { get; set; }
        
        public TrickInfo CurrentTrick { get; set; }
        
        public PlayerPosition TrumpCaller { get; set; }
    }
}