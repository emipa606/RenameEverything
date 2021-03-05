﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace RenameEverything
{
    public static class Patch_Infused_GenMapUI_DrawThingLabel_Patch
    {
        public static class ManualPatch_Postfix
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var instructionList = instructions.ToList();

                var adjustPositionIfNamedInfo = AccessTools.Method(typeof(Patch_GenMapUI.Patch_DrawThingLabel),
                    nameof(Patch_GenMapUI.Patch_DrawThingLabel.AdjustPositionIfNamed));

                foreach (var codeInstruction in instructionList)
                {
                    var instruction = codeInstruction;

                    // Look for the -0.66; change to -0.92 if named
                    if (instruction.opcode == OpCodes.Ldc_R4 && (float) instruction.operand == -0.66f)
                    {
                        yield return instruction; // -0.66f
                        yield return new CodeInstruction(OpCodes.Ldarg_0); // thing
                        instruction =
                            new CodeInstruction(OpCodes.Call,
                                adjustPositionIfNamedInfo); // AdjustPositionIfNamed(-0.66f, thing)
                    }

                    yield return instruction;
                }
            }
        }
    }
}