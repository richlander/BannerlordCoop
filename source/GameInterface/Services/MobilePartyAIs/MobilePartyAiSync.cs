﻿using GameInterface.AutoSync;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Party;

namespace GameInterface.Services.MobilePartyAIs;
internal class MobilePartyAiSync : IAutoSync
{
    public MobilePartyAiSync(IAutoSyncBuilder autoSyncBuilder)
    {
        autoSyncBuilder.AddProperty(AccessTools.Property(typeof(MobilePartyAi), nameof(MobilePartyAi.AiBehaviorPartyBase)));

        autoSyncBuilder.AddField(AccessTools.Field(typeof(MobilePartyAi), nameof(MobilePartyAi._mobileParty)));
    }
}
