using Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameInterface.Services.Battles.Messages
{
    internal class BattleStarted : IMessage
    {
        public string AttackerId { get; }
        public string DefenderId { get; }

        public BattleStarted(string attackerId, string defenderId)
        {
            AttackerId = attackerId;
            DefenderId = defenderId;
        }
    }
}
