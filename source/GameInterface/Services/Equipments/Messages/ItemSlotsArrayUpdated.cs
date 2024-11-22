using Common.Messaging;
using TaleWorlds.Core;

namespace GameInterface.Services.MapEvents.Messages;
internal record ItemSlotsArrayUpdated(Equipment Instance, EquipmentElement Value, int Index) : IEvent
{
    public Equipment Instance { get; } = Instance;
    public EquipmentElement Value { get; } = Value;
    public int Index { get; } = Index;
}
