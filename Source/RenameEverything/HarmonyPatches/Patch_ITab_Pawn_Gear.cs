﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RenameEverything
{
    public static class Patch_ITab_Pawn_Gear
    {
        [HarmonyPatch(typeof(ITab_Pawn_Gear))]
        [HarmonyPatch("DrawThingRow")]
        public static class Patch_DrawThingRow
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var instructionList = instructions.ToList();

                var wordWrapInfo = AccessTools.Property(typeof(Text), nameof(Text.WordWrap)).GetSetMethod();
                var wordWraps = 0;

                var infoCardButtonInfo = AccessTools.Method(typeof(Widgets), nameof(Widgets.InfoCardButton),
                    new[] {typeof(float), typeof(float), typeof(Thing)});

                var doRenameFloatMenuButtonInfo =
                    AccessTools.Method(typeof(Patch_DrawThingRow), nameof(DoRenameFloatMenuButton));

                foreach (var codeInstruction in instructionList)
                {
                    var instruction = codeInstruction;

                    // Do our 'renamable gizmos substitute' button after the info card button
                    if (instruction.opcode == OpCodes.Call && (MethodInfo) instruction.operand == infoCardButtonInfo)
                    {
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Ldloca_S, 0);
                        yield return new CodeInstruction(OpCodes.Ldarga_S, 1);
                        yield return new CodeInstruction(OpCodes.Ldarg_3);
                        instruction = new CodeInstruction(OpCodes.Call, doRenameFloatMenuButtonInfo);
                    }

                    // Label recolouring
                    if (instruction.opcode == OpCodes.Call && (MethodInfo) instruction.operand == wordWrapInfo)
                    {
                        wordWraps++;
                        yield return instruction;
                        if (wordWraps % 2 == 0)
                        {
                            instruction = new CodeInstruction(OpCodes.Call,
                                RenameUtility.ChangeGUIColourPostLabelDraw_Info); // ChangeGUIColourPostLabelDraw()
                        }
                        else
                        {
                            yield return new CodeInstruction(OpCodes.Ldarg_3); // thing
                            instruction = new CodeInstruction(OpCodes.Call,
                                RenameUtility
                                    .ChangeGUIColourPreLabelDraw_Thing_Info); // ChangeGUIColourPreLabelDraw(thing)
                        }
                    }

                    yield return instruction;
                }
            }

            private static void DoRenameFloatMenuButton(ref Rect rect, ref float y, Thing thing)
            {
                rect.width -= 24;
                var renamableComp = thing.TryGetComp<CompRenamable>();
                if (renamableComp != null &&
                    Widgets.ButtonImage(new Rect(rect.width - 24, rect.y + y, 24, 24), TexButton.RenameTex))
                {
                    Find.WindowStack.Add(new FloatMenu(RenameUtility
                        .CaravanRenameThingButtonFloatMenuOptions(renamableComp).ToList()));
                }
            }
        }
    }
}