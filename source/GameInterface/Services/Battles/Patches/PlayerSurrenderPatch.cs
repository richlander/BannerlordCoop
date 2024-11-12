﻿using Common;
using Common.Messaging;
using Common.Util;
using GameInterface.Policies;
using GameInterface.Services.Battles.Messages;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace GameInterface.Services.MobileParties.Patches
{
    /// <summary>
    /// Patches the surrender of the player party, only runs on local client
    /// </summary>
    [HarmonyPatch(typeof(EncounterGameMenuBehavior))]
    public class PlayerSurrenderPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("game_menu_encounter_surrender_on_consequence")]
        public static bool Prefix(MenuCallbackArgs args)
        {
            if (AllowedThread.IsThisThreadAllowed()) return true;

            if (CallOriginalPolicy.IsOriginalAllowed()) return true;

            BattleSideEnum enemySide = BattleSideEnum.None;

            if (MobileParty.MainParty.MapEvent.PlayerSide == BattleSideEnum.Defender)
            {
                enemySide = BattleSideEnum.Attacker;
            }
            else if (MobileParty.MainParty.MapEvent.PlayerSide == BattleSideEnum.Attacker)
            {
                enemySide = BattleSideEnum.Defender;
            }

            MessageBroker.Instance.Publish(null, new PlayerSurrender(
                MobileParty.MainParty.StringId,
                MobileParty.MainParty.MapEvent.GetMapEventSide(enemySide).LeaderParty.MobileParty.StringId,
                MobileParty.MainParty.LeaderHero.StringId));

            return true;
        }

        public static void RunStartPlayerCaptivity(PartyBase CaptorParty)
        {
            GameLoopRunner.RunOnMainThread(() =>
            {
                using (new AllowedThread())
                {
                    Campaign.Current.GetCampaignBehavior<EncounterGameMenuBehavior>()
                    .game_menu_encounter_surrender_on_consequence(
                        new MenuCallbackArgs(Campaign.Current.CurrentMenuContext, new TextObject())
                    );
                }
            });
        }
    }
}