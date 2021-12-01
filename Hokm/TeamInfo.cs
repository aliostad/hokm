using System;

namespace Hokm
{
    public class TeamInfo
    {
        public Guid Id { get; init; }

        public string Name { get; init; }

        public IPlayerInfo Player1 { get; init; }
        
        public IPlayerInfo Player2 { get; init; }

    }
}