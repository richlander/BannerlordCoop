using Common.Logging;
using Common.Messaging;
using GameInterface.Services.MobileParties.Extensions;
using GameInterface.Services.MobileParties.Messages.Behavior;
using HarmonyLib;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace GameInterface.Services.MobileParties.Patches;

/// <summary>
/// Patches for player encounters
/// </summary>

[HarmonyPatch(typeof(EncounterManager))]
internal class EncounterManagerPatches
{
    private static ILogger Logger = LogManager.GetLogger<EncounterManagerPatches>();

    private static bool inSettlement = false;
    private static MethodInfo Start => typeof(PlayerEncounter).GetMethod(nameof(PlayerEncounter.Start));
    private static MethodInfo Init => typeof(PlayerEncounter).GetMethod(
        "Init",
        BindingFlags.NonPublic | BindingFlags.Instance,
        null,
        new Type[] { typeof(PartyBase), typeof(PartyBase), typeof(Settlement) },
        null);

    /// <summary>
    /// In the <see cref="EncounterManager.StartSettlementEncounter"/> method
    /// Replaces
    ///     PlayerEncounter.Start();
    ///     PlayerEncounter.Current.Init(attackerParty.Party, settlement.Party, settlement);
    /// With
    ///     EncounterManagerPatches.PlayerEncounterIntercept(attackerParty.Party, settlement.Party, settlement);
    /// </summary>
    /// <param name="instrs">previous instructions</param>
    /// <returns>patched instructions</returns>
    [HarmonyTranspiler]
    [HarmonyPatch(nameof(EncounterManager.StartSettlementEncounter))]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instrs)
    {
        var intercept = typeof(EncounterManagerPatches)
            .GetMethod(nameof(PlayerEncounterIntercept), BindingFlags.NonPublic | BindingFlags.Static);

        foreach (CodeInstruction instr in instrs)
        {
            if (instr.Calls(Start))
            {
                var newInstr = new CodeInstruction(OpCodes.Nop);
                newInstr.labels = instr.labels;
                newInstr.blocks = instr.blocks;
                yield return newInstr;
                continue;
            }
            if (instr.Calls(Init))
            {
                var newInstr = new CodeInstruction(OpCodes.Call, intercept);
                newInstr.labels = instr.labels;
                newInstr.blocks = instr.blocks;
                yield return newInstr;
                continue;
            }

            yield return instr;
        }
    }

    private static void PlayerEncounterIntercept(PlayerEncounter __instance, PartyBase attackerParty, PartyBase defenderParty, Settlement settlement = null)
    {
        if (inSettlement) return;

        attackerParty.MobileParty.Ai.DefaultBehaviorNeedsUpdate = true;
        var message = new StartSettlementEncounterAttempted(
            attackerParty.MobileParty.StringId,
            settlement.StringId);
        MessageBroker.Instance.Publish(attackerParty, message);

        inSettlement = true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerEncounter), nameof(PlayerEncounter.Finish))]
    private static void PlayerEncounterFinishPatch(bool forcePlayerOutFromSettlement)
    {
        inSettlement = false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(EncounterManager.HandleEncounterForMobileParty))]
    internal static bool HandleEncounterForMobilePartyPatch(ref MobileParty mobileParty)
    {
        // Skip this method if party is not controlled
        if (mobileParty.IsPartyControlled() == false) return false;

        return true;
    }
}
