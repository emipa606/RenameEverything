using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace RenameEverything
{
    public static class Patch_GenMapUI
    {
        [HarmonyPatch(typeof(GenMapUI))]
        [HarmonyPatch(nameof(GenMapUI.DrawThingLabel))]
        [HarmonyPatch(new[] {typeof(Thing), typeof(string), typeof(Color)})]
        public static class Patch_DrawThingLabel
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var instructionList = instructions.ToList();

                var adjustPositionIfNamedInfo =
                    AccessTools.Method(typeof(Patch_DrawThingLabel), nameof(AdjustPositionIfNamed));

                foreach (var codeInstruction in instructionList)
                {
                    var instruction = codeInstruction;

                    // Look for the -0.4; change to -0.66 if named
                    if (instruction.opcode == OpCodes.Ldc_R4 && (float) instruction.operand == -0.4f)
                    {
                        yield return instruction; // -0.4f
                        yield return new CodeInstruction(OpCodes.Ldarg_0); // thing
                        instruction =
                            new CodeInstruction(OpCodes.Call,
                                adjustPositionIfNamedInfo); // AdjustPositionIfNamed(-0.4f, thing)
                    }

                    yield return instruction;
                }
            }

            public static float AdjustPositionIfNamed(float original, Thing thing)
            {
                if (RenameUtility.CanDrawThingName(thing))
                {
                    return original - 0.26f;
                }

                return original;
            }

            public static void Postfix(Thing thing)
            {
                RenameUtility.DrawThingName(thing);
            }
        }
    }
}