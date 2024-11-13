using Common.Messaging;
using Common.Util;
using GameInterface.Services.Battles.Messages;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;

namespace GameInterface.Services.MobileParties.Patches
{
    /// <summary>
    /// Patches the StartBattle in PlayerEncounter, only runs on local client
    /// </summary>
    [HarmonyPatch(typeof(PlayerEncounter))]
    public class PlayerEncounterPatch
    {

        [HarmonyPatch("StartBattleInternal")]
        [HarmonyPrefix]
        public static bool Prefix(ref PlayerEncounter __instance)
        {
            if (AllowedThread.IsThisThreadAllowed()) return true;

            var message = new PlayerStartBattle();

            MessageBroker.Instance.Publish(__instance, message);

            return false;
        }
    }

    //[HarmonyPatch(typeof(EncounterGameMenuBehavior))]
    //public class TestPatching2
    //{
    //    [HarmonyPatch("game_menu_encounter_leave_on_condition")]
    //    [HarmonyPrefix]
    //    public static bool Prefix(MenuCallbackArgs args)
    //    {
    //        if (AllowedThread.IsThisThreadAllowed()) return true;

    //        var message = new PlayerStartBattle();

    //        MessageBroker.Instance.Publish(null, message);

    //        return false;
    //    }

    //    [HarmonyPatch("game_menu_encounter_leave_on_consequence")]
    //    [HarmonyPrefix]
    //    public static bool Prefix2(MenuCallbackArgs args)
    //    {
    //        if (AllowedThread.IsThisThreadAllowed()) return true;

    //        return false;
    //    }
    //}

    [HarmonyPatch(typeof(MapEventHelper))]
    public class Testing3
    {
        [HarmonyPatch("CanLeaveBattle")]
        [HarmonyPrefix]
        public static void Prefix(MobileParty mobileParty)
        {
            if (ModInformation.IsClient)
            {
                ;
            }
        }
    }
}