using Common.Logging;
using Common.Messaging;
using GameInterface.Policies;
using GameInterface.Services.CultureObjects.Messages;
using HarmonyLib;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;

namespace GameInterface.Services.CultureObjects.Patches
{
    [HarmonyPatch]
    internal class CultureObjectLifetimePatches
    {
        private static readonly ILogger Logger = LogManager.GetLogger<CultureObjectLifetimePatches>();

        [HarmonyPatch(typeof(CultureObject), MethodType.Constructor)]
        [HarmonyPrefix]
        private static bool ctorPrefix(ref CultureObject __instance)
        {
            // Call original if we call this function
            if (CallOriginalPolicy.IsOriginalAllowed()) return true;

            if (ModInformation.IsClient)
            {
                Logger.Error("Client created unmanaged {name}\n"
                    + "Callstack: {callstack}", typeof(CultureObject), Environment.StackTrace);

                return true;
            }

            var message = new CultureObjectCreated(__instance);

            MessageBroker.Instance.Publish(null, message);

            return true;
        }
    }

    [HarmonyPatch]
    internal class MBObjectManagerLifetimePatches
    {
        private static ILogger Logger = LogManager.GetLogger<MBObjectManagerLifetimePatches>();

        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(MBObjectManager), nameof(MBObjectManager.CreateObject), new Type[] { typeof(string) }).MakeGenericMethod(typeof(CultureObject));
        }

        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> CreateFromTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instr in instructions)
            {
                if (instr.Calls(AccessTools.PropertySetter(typeof(MBObjectBase), nameof(MBObjectBase.StringId))))
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                }
                else
                {
                    yield return instr;
                }
            }
        }
    }
}
