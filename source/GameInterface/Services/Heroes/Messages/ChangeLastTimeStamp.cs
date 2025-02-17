﻿using Common.Messaging;

namespace GameInterface.Services.Heroes.Messages
{
    /// <summary>
    /// Client publish for LastTimeStampForActivity
    /// </summary>
    public record ChangeLastTimeStamp : ICommand
    {
        public int LastTimeStamp { get; }
        public string HeroId { get; }

        public ChangeLastTimeStamp(int lastTimeStamp, string heroId)
        {
            LastTimeStamp = lastTimeStamp;
            HeroId = heroId;
        }
    }
}
