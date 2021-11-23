namespace Hokm
{
    public class Game
    {
        public Team Team1 { get; init; }
        
        public Team Team2 { get; init; }
        
        public TrumpCaller Caller { get; init; }
        
        
        public Game(Team team1, Team team2, TrumpCaller caller)
        {
            Team1 = team1;
            Team2 = team2;
            Caller = caller;
        }
        
    }
}