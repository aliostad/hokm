using System.Diagnostics.CodeAnalysis;

namespace Hokm
{
    public enum PlayerPosition
    {
        Team1Player1,
        Team2Player1,
        Team1Player2,
        Team2Player2
    }

    public static class PlayerPositions
    {
        public static readonly PlayerPosition[] All = new[]
        {
            PlayerPosition.Team1Player1,
            PlayerPosition.Team1Player2,
            PlayerPosition.Team2Player1,
            PlayerPosition.Team2Player2
        };

        public static bool IsTeam1(PlayerPosition position)
        {
            return position is PlayerPosition.Team1Player1 or PlayerPosition.Team1Player2;
        }
    }
}