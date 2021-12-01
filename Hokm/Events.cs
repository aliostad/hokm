using System;
using System.Collections.Generic;
using CardGame;

namespace Hokm
{
    public class TrickFinishedEventArgs : EventArgs
    {
        public TrickOutcome Outcome { get; init; }
    }

    public class BanterUtteredEventArgs : EventArgs
    {
        public IPlayerInfo PlayerInfo { get; init; }
        
        public string Banter { get; init; }
    }
    
    public class CardPlayedEventArgs : EventArgs
    {
        public IEnumerable<Card> Cards { get; init; }
        
        public PlayerPosition StarterPlayer { get; init; }
        
        public Suit TrumpSuit { get; init; }
        
        public int TrickNumber { get; init; }
        
        public int GameNumber { get; set; }
    }

    public class GameStartedEventArgs : EventArgs
    {
        public Game Game { get; init; }
    }
    
    public class GameFinishedEventArgs : EventArgs
    {
        public Game Game { get; init; }
    }

    public class CardsDealtEventArgs : EventArgs
    {
        public IDictionary<PlayerPosition, IEnumerable<Card>> Hands { get; init; }
        
        public Suit TrumpSuit { get; init; }
    }

    public class MatchEventArgs : EventArgs
    {
        public MatchInfo Info { get; init; }
        
        public EventType EventType { get; init; }
    }
}