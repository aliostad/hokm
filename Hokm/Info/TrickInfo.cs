using System.Collections.Generic;
using CardGame;

namespace Hokm
{
    public class TrickInfo
    {
        public PlayerPosition Starter { get; init; }

        public IEnumerable<Card> CardsPlayed { get; set; }

        public PlayerPosition? CurrentWinningPosition { get; set; }
        
        public int TrickNumber { get; init; }
    }
}