using System;

namespace Hokm
{
    public class Team
    {
        public Guid Id { get; init; }

        public string Name { get; init; }

        public IPlayer Player1 { get; init; }
        
        public IPlayer Player2 { get; init; }
    }
    
    public static class TeamExtensions 
    {
        public static TeamInfo ToInfo(this Team team)
        {
            return new TeamInfo()
            {
                Id = team.Id,
                Name = team.Name,
                Player1 = team.Player1.ToInfo(),
                Player2 = team.Player2.ToInfo()
            };
        }
    }
}