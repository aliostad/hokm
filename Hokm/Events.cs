using System;

namespace Hokm
{
    public class TrickCompletedEventArgs : EventArgs
    {
        public TrickOutcome Outcome { get; init; }
    }
}