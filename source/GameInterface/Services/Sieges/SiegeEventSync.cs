using GameInterface.AutoSync;
using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem.Siege;

namespace GameInterface.Services.Sieges
{
    internal class SiegeEventSync : IAutoSync
    {
        public SiegeEventSync(IAutoSyncBuilder autoSyncBuilder)
        {
            // Fields
            autoSyncBuilder.AddField(AccessTools.Field(typeof(SiegeEvent), nameof(SiegeEvent.BesiegedSettlement)));
            autoSyncBuilder.AddField(AccessTools.Field(typeof(SiegeEvent), nameof(SiegeEvent.BesiegerCamp)));
            autoSyncBuilder.AddField(AccessTools.Field(typeof(SiegeEvent), nameof(SiegeEvent._isBesiegerDefeated)));

            // Props
            autoSyncBuilder.AddProperty(AccessTools.Property(typeof(SiegeEvent), nameof(SiegeEvent.SiegeStartTime)));
        }
    }
}
