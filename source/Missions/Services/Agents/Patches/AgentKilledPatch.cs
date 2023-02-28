﻿    using Common.Messaging;
using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace Missions.Services.Agents.Patches
{
    [HarmonyPatch(typeof(Agent))]
    internal class AgentKilledPatch
    {
        [HarmonyPatch(MethodType.Setter)]
        [HarmonyPatch(nameof(Agent.Health))]
        [HarmonyPostfix]
        private static void OnHealthChanged(ref Agent __instance)
        {
            if(__instance.Health <= 0)
            {
                MessageBroker.Instance.Publish(__instance, new AgentDied(__instance));
            }
        }
    }
}
