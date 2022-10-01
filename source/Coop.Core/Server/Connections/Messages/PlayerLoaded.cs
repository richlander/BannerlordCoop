﻿namespace Coop.Core.Server.Connections.Messages
{
    public readonly struct PlayerLoaded
    {
        public PlayerLoaded(string playerId)
        {
            PlayerId = playerId;
        }

        public string PlayerId { get; }
    }
}
