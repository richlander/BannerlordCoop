﻿using Common.Logging;
using Common.Messaging;
using HarmonyLib;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using GameInterface.Policies;
using GameInterface.Services.Equipments.Messages.Events;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.Recruitment;
using TaleWorlds.CampaignSystem.Settlements;
using GameInterface.Services.Heroes.Messages.Collections;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.Party.PartyComponents;

namespace GameInterface.Services.Heroes.Patches;

[HarmonyPatch]
internal class HeroCollectionPatches
{
    private static readonly ILogger Logger = LogManager.GetLogger<HeroCollectionPatches>();

    private static IEnumerable<MethodBase> TargetMethods()
    {
        foreach (var method in AccessTools.GetDeclaredMethods(typeof(Hero)))
        {
            yield return method;
        }
        yield return AccessTools.Method(typeof(RecruitmentVM), nameof(RecruitmentVM.OnDone));
        yield return AccessTools.Method(typeof(RecruitmentCampaignBehavior), nameof(RecruitmentCampaignBehavior.RecruitVolunteersFromNotable));
        yield return AccessTools.Method(typeof(RecruitmentCampaignBehavior), nameof(RecruitmentCampaignBehavior.UpdateVolunteersOfNotablesInSettlement));
        yield return AccessTools.Method(typeof(RecruitmentCampaignBehavior), nameof(RecruitmentCampaignBehavior.ApplyInternal));
        yield return AccessTools.Method(typeof(Town), nameof(Town.DailyGarrisonAdjustment));
    }

    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var stack = new Stack<CodeInstruction>();

        var VolunteerArrayType = AccessTools.Field(typeof(Hero), nameof(Hero.VolunteerTypes));
        var arrayAssignIntercept = AccessTools.Method(typeof(HeroCollectionPatches), nameof(ArrayAssignIntercept));
        foreach (var instruction in instructions)
        {
            if (stack.Count > 0 && instruction.opcode == OpCodes.Stelem_Ref)
            {
                stack.Pop();

                var newInstr = new CodeInstruction(OpCodes.Call, arrayAssignIntercept);
                newInstr.labels = instruction.labels;

                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return newInstr;
                continue;
            }

            if (instruction.opcode == OpCodes.Ldfld && instruction.operand as FieldInfo == VolunteerArrayType)
            {
                stack.Push(instruction);
            }

            yield return instruction;
        }
    }

    public static void ArrayAssignIntercept(CharacterObject[] VolunteerTypes, int index, CharacterObject value, Hero instance)
    {
        // Call original if we call this function
        if (CallOriginalPolicy.IsOriginalAllowed())
        {
            VolunteerTypes[index] = value;
            return;
        }

        if (ModInformation.IsClient)
        {
            Logger.Error("Client created unmanaged {name}\n"
                + "Callstack: {callstack}", typeof(Equipment), Environment.StackTrace);
            return;
        }
        var message = new VolunteerTypesArrayUpdated(instance, value, index);
        MessageBroker.Instance.Publish(instance, message);

        VolunteerTypes[index] = value;
    }

    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> ChildrenTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var stack = new Stack<CodeInstruction>();

        var childrenAddMethod = typeof(MBList<Hero>).GetMethod("Add");
        var childrenAddIntercept = AccessTools.Method(typeof(HeroCollectionPatches), nameof(ChildrenAddIntercept));

        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Callvirt && instruction.operand as MethodInfo == childrenAddMethod)
            {

                var newInstr = new CodeInstruction(OpCodes.Call, childrenAddIntercept);
                newInstr.labels = instruction.labels;

                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return newInstr;

            }
            else
            {
                yield return instruction;
            }
        }
    }

    public static void ChildrenAddIntercept(MBList<Hero> children, Hero value, Hero instance)
    {
        // Call original if we call this function
        if (CallOriginalPolicy.IsOriginalAllowed())
        {
            children.Add(value);
            return;
        }

        if (ModInformation.IsClient)
        {
            Logger.Error("Client created unmanaged {name}\n"
                + "Callstack: {callstack}", typeof(Equipment), Environment.StackTrace);
            return;
        }
        var message = new ChildrenListUpdated(instance, value);
        MessageBroker.Instance.Publish(instance, message);

        children.Add(value);
    }
    /*
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> CaravanTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var stack = new Stack<CodeInstruction>();

        var caravanAddMethod = typeof(List<PartyComponent>).GetMethod("Add");
        var caravanAddIntercept = AccessTools.Method(typeof(HeroCollectionPatches), nameof(CaravanAddIntercept));

        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Callvirt && instruction.operand as MethodInfo == caravanAddMethod)
            {

                var newInstr = new CodeInstruction(OpCodes.Call, caravanAddIntercept);
                newInstr.labels = instruction.labels;

                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return newInstr;

            }
            else
            {
                yield return instruction;
            }
        }
    }

    public static void CaravanAddIntercept(MBList<Hero> children, Hero value, Hero instance)
    {
        // Call original if we call this function
        if (CallOriginalPolicy.IsOriginalAllowed())
        {
            children.Add(value);
            return;
        }

        if (ModInformation.IsClient)
        {
            Logger.Error("Client created unmanaged {name}\n"
                + "Callstack: {callstack}", typeof(Equipment), Environment.StackTrace);
            return;
        }
        var message = new ChildrenListUpdated(instance, value);
        MessageBroker.Instance.Publish(instance, message);

        children.Add(value);
    }
    */
}


