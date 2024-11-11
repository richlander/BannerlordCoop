using Common;
using Common.Logging;
using Common.Messaging;
using Common.Util;
using GameInterface.Policies;
using GameInterface.Services.Heroes.Patches;
using GameInterface.Services.MobileParties.Data;
using GameInterface.Services.MobileParties.Messages.Lifetime;
using GameInterface.Services.ObjectManager;
using HarmonyLib;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.ObjectSystem;

namespace GameInterface.Services.MobileParties.Patches;

/// <summary>
/// Patches for lifecycle of <see cref="MobileParty"/> objects.
/// </summary>
[HarmonyPatch(typeof(MobileParty))]
internal class MobilePartyInteractionPatch
{
    private static readonly ILogger Logger = LogManager.GetLogger<MobilePartyInteractionPatch>();


    //[HarmonyPatch("TaleWorlds.CampaignSystem.Map.IMapEntity.OnPartyInteraction")]
    //[HarmonyPrefix]
    //private static bool PartyInteractionPrefix(ref MobileParty __instance, MobileParty engagingParty)
    //{
    //    //Call original if we call this function

    //    if (CallOriginalPolicy.IsOriginalAllowed()) return true;

    //    if (ModInformation.IsClient)
    //    {
    //        Logger.Error("Client destroyed unmanaged {name}\n"
    //            + "Callstack: {callstack}", typeof(MobileParty), Environment.StackTrace);
    //        return false;
    //    }

    //    MessageBroker.Instance.Publish(__instance, new PartyDestroyed(__instance));

    //    return true;
    //}
}