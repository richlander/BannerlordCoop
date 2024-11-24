using GameInterface.Services.Registry;
using System.Threading;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.ObjectSystem;

namespace GameInterface.Services.Settlements;
internal class SettlementRegistry : RegistryBase<Settlement>
{
    private const string SettlementPrefix = $"Coop{nameof(Settlement)}";
    private static int InstanceCounter = 0;

    public SettlementRegistry(IRegistryCollection collection) : base(collection) { }

    public override void RegisterAll()
    {
        var campaignObjectManager = Campaign.Current?.CampaignObjectManager;

        if (campaignObjectManager == null)
        {
            Logger.Error("Unable to register objects when CampaignObjectManager is null");
            return;
        }

        foreach (var settlement in campaignObjectManager.Settlements)
        {
            base.RegisterExistingObject(settlement.StringId, settlement);
        }
    }

    public override bool RegisterExistingObject(string id, object obj)
    {
        var result = base.RegisterExistingObject(id, obj);

        AddToCampaignObjectManager(obj);

        return result;
    }

    protected override string GetNewId(Settlement settlement)
    {
        return $"{SettlementPrefix}_{Interlocked.Increment(ref InstanceCounter)}";
    }

    private void AddToCampaignObjectManager(object obj)
    {
        if (TryCast(obj, out var _) == false) return;

        var objectManager = Campaign.Current?.CampaignObjectManager;

        if (objectManager == null) return;

        objectManager.Settlements = MBObjectManager.Instance.GetObjectTypeList<Settlement>();
    }
}
