using Common.Logging;
using Common.Messaging;
using GameInterface.Policies;
using GameInterface.Services.CharacterObjects.Messages;
using HarmonyLib;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;

namespace GameInterface.Services.CharacterObjects.Patches
{
    /// <summary>
    /// Lifetime Patches for CharacterObjects
    /// </summary>
    [HarmonyPatch]
    internal class CharacterObjectLifetimePatches
    {
        private static ILogger Logger = LogManager.GetLogger<CharacterObjectLifetimePatches>();

        [HarmonyPatch(typeof(CharacterObject), MethodType.Constructor)]
        [HarmonyPrefix]
        private static bool CreateCharacterObjectPrefix(ref CharacterObject __instance)
        {
            // Call original if we call this function
            if (CallOriginalPolicy.IsOriginalAllowed()) return true;

            if (ModInformation.IsClient)
            {
                Logger.Error("Client created unmanaged {name}\n"
                    + "Callstack: {callstack}", typeof(CharacterObject), Environment.StackTrace);
                return false;
            }

            var message = new CharacterObjectCreated(__instance);

            MessageBroker.Instance.Publish(__instance, message);

            return true;
        }
    }


    [HarmonyPatch]
    internal class MBObjectManagerLifetimePatches
    {
        private static ILogger Logger = LogManager.GetLogger<MBObjectManagerLifetimePatches>();

        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(MBObjectManager), nameof(MBObjectManager.CreateObject), new Type[] { typeof(string) }).MakeGenericMethod(typeof(CharacterObject));
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
