using Common.Messaging;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace GameInterface.Services.Equipments.Messages.Events;
internal record VolunteerTypesArrayUpdated(Hero Instance, CharacterObject Item, int Index) : IEvent
{
    public Hero Instance { get; } = Instance;
    public CharacterObject Item { get; } = Item;
    public int Index { get; } = Index;
}
