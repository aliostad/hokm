using System;

namespace Hokm
{
    public class TrickCompletedEventArgs : EventArgs
    {
        public TrickOutcome Outcome { get; init; }
    }

    public class BanterUtteredEventArgs : EventArgs
    {
        public IPlayerInfo PlayerInfo { get; init; }
        
        public string Banter { get; init; }
    }

    public class GameFinishedEventArgs : EventArgs
    {
        public GameScore Score { get; init; }
    }
}