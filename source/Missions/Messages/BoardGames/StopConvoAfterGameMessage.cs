﻿using Common.Messaging;
using SandBox.BoardGames.MissionLogics;
using TaleWorlds.MountAndBlade;

namespace Missions.Messages.Agents
{
    public readonly struct StopConvoAfterGameMessage : IEvent
    {
        public StopConvoAfterGameMessage(bool n)
        {
        }
    }
}