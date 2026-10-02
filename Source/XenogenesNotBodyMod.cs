using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace XenogenesNotBodyMod;

public class XenogenesNotBodyMod : Mod
{
    public XenogenesNotBodyMod(ModContentPack content) : base(content) {
        LongEventHandler.QueueLongEvent(action: HarmonyPatches,
            textKey: null,
            doAsynchronously: true,
            exceptionHandler: null
        );
    }

    private static void HarmonyPatches() {
        Harmony harmony = new Harmony("Hali.XenoNotBodyMod");
        harmony.PatchAll();
    }
}

// Babbys first time using annotations :partyEmoji:
[HarmonyPatch(typeof(GeneUtility), nameof(GeneUtility.AddedAndImplantedPartsWithXenogenesCount))]
public static class BbpThoughtWorkerBodyPuristDisgust
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        var codeMatcher = new CodeMatcher(instructions);
        /*
            // int num = pawn.health.hediffSet.CountAddedAndImplantedParts();
            IL_0000: ldarg.0
            IL_0001: ldfld class Verse.Pawn_HealthTracker Verse.Pawn::health
            IL_0006: ldfld class Verse.HediffSet Verse.Pawn_HealthTracker::hediffSet
            IL_000b: call int32 Verse.HediffUtility::CountAddedAndImplantedParts(class Verse.HediffSet)
            IL_0010: stloc.0

            // if (ModsConfig.BiotechActive && pawn.genes != null && pawn.genes.Xenogenes.Any())
            // putting code here
            IL_0011: call bool Verse.ModsConfig::get_BiotechActive()
            IL_0016: brfalse.s IL_0036

            IL_0018: ldarg.0
            IL_0019: ldfld class RimWorld.Pawn_GeneTracker Verse.Pawn::genes
            IL_001e: brfalse.s IL_0036

         */



        MethodInfo fieldLookingFor = AccessTools.PropertyGetter(typeof(ModsConfig), nameof(ModsConfig.BiotechActive));

        codeMatcher.MatchStartForward(new CodeMatch(OpCodes.Call, fieldLookingFor))
            .ThrowIfInvalid("Could not find call to ModsConfig.BiotechActive");
        codeMatcher.InsertAndAdvance(
            new CodeMatch(OpCodes.Ldloc_0),
            new CodeMatch(OpCodes.Ret)
        );

        return codeMatcher.InstructionsInRange(0, codeMatcher.Pos - 1);
    }
}