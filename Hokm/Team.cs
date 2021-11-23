using System;

namespace Hokm
{
    public class Team
    {
        private Guid Id { get; init; }

        private string Name { get; init; }

        public IPlayer Player1 { get; init; }
        
        public IPlayer Player2 { get; init; }
        
    }
}