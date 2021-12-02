using System;

namespace Hokm
{
    public interface IPlayerInfo
    {
        Guid Id { get; }

        string Name { get; }

    }

    public class PlayerInfo : IPlayerInfo
    {
        public Guid Id { get; init; }

        public string Name { get; init; }
    }
}