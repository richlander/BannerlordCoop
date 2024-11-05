using Common.Messaging;
using TaleWorlds.CampaignSystem.MapEvents;

namespace GameInterface.Services.MapEventParties.Messages
{
    internal record MapEventPartyCreated : IEvent
    {
        public MapEventParty MapEventParty { get; }

        public MapEventPartyCreated(MapEventParty mapEventParty)
        {
            MapEventParty = mapEventParty;
        }
    }
}
