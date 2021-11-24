using System.Collections.Generic;
using CardGame;

namespace Hokm
{
    public record TrickOutcome
    {
        public TrumpUsage TrumpUsage { get; init; }
        
        public PlayerPosition Winner { get; init; }
        
        public IEnumerable<Card> CardsPlayed { get; init; }
    }
    
    public record TrickReportForPlayer : TrickOutcome
    {
        public bool YourTeamWon { get; init; }
    }
}