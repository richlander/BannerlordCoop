﻿using GameInterface.Services.Registry;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.ObjectSystem;

namespace GameInterface.Services.Settlements;
internal class SettlementRegistry : RegistryBase<Settlement>
{
    public static readonly string SettlementStringIdPrefix = "CoopSettlement";
    private int InstanceCounter = 0;

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
            Interlocked.Increment(ref InstanceCounter);
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
        settlement.StringId = $"{SettlementStringIdPrefix}_{Interlocked.Increment(ref InstanceCounter)}";
        return settlement.StringId;
    }

    private void AddToCampaignObjectManager(object obj)
    {
        if (TryCast(obj, out var _) == false) return;

        var objectManager = Campaign.Current?.CampaignObjectManager;

        if (objectManager == null) return;

        objectManager.Settlements = MBObjectManager.Instance.GetObjectTypeList<Settlement>();
    }
}
