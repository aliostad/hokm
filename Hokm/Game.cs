using System;
using System.Linq;
using System.Collections.Generic;
using CardGame;

namespace Hokm
{
    public class Game
    {
        public Team Team1 { get; init; }
        
        public Team Team2 { get; init; }
        
        public PlayerPosition Caller { get; init; }
        

        private List<PlayerPosition> _playingOrder;

        private Suit _trumpSuit;
        
        public Game(Team team1, Team team2, PlayerPosition caller)
        {
            Team1 = team1;
            Team2 = team2;
            Caller = caller;

            _playingOrder = Enumerable.Range(0, 4)
                .Select(x => (x + (int)caller) % 4)
                .Cast<PlayerPosition>().ToList();

        }

        internal IPlayer GetPlayer(PlayerPosition position)
        {
            return position switch
            {
                PlayerPosition.Team1Player1 => Team1.Player1,
                PlayerPosition.Team1Player2 => Team1.Player2,
                PlayerPosition.Team2Player1 => Team2.Player1,
                PlayerPosition.Team2Player2 => Team2.Player2,
                _ => throw new ArgumentException($"Invalid position: {position}")
            };
        }

        public Suit Deal()
        {
            var trumpCaller = GetPlayer(Caller);
            var deck = new Deck().Shuffle();
            trumpCaller.ReceiveHand(deck.Deal(5));
            _trumpSuit = trumpCaller.CallTrumpSuit();
            GetPlayer(_playingOrder[1]).ReceiveHand(deck.Deal(5));
            GetPlayer(_playingOrder[2]).ReceiveHand(deck.Deal(5));
            GetPlayer(_playingOrder[3]).ReceiveHand(deck.Deal(5));

            for (int i = 0; i < 2; i++)
            {
                foreach (var position in _playingOrder)
                {
                    GetPlayer(position).ReceiveHand(deck.Deal(4));
                }
            }
            
            return _trumpSuit;
        }
        
        
        
    }
}