using Common.Messaging;
using TaleWorlds.CampaignSystem.Party;

namespace GameInterface.Services.Encounters.Messages
{
    internal class StartBattle : IMessage
    {
        public MobileParty Attacker { get; }
        public MobileParty Defender { get; }

        public StartBattle(MobileParty attacker, MobileParty defender)
        {
            Attacker = attacker;
            Defender = defender;
        }
    }
}