using Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using TaleWorlds.CampaignSystem.Party;

namespace GameInterface.Services.Battles.Messages
{
    internal class BattleStarted : IMessage
    {
        public MobileParty Attacker { get; }
        public MobileParty Defender { get; }

        public BattleStarted(MobileParty attacker, MobileParty defender)
        {
            Attacker = attacker;
            Defender = defender;
        }
    }
}
