using GameInterface.AutoSync;
using HarmonyLib;
using TaleWorlds.CampaignSystem;

namespace GameInterface.Services.Heroes
{
    internal class HeroSync : IAutoSync
    {
        public HeroSync(IAutoSyncBuilder autoSyncBuilder)
        {
            autoSyncBuilder.AddProperty(AccessTools.Property(typeof(Hero), nameof(Hero.StaticBodyProperties)));
            
        
        
        
        
        }
    }
}
