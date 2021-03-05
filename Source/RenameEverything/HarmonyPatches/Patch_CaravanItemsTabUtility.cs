using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RenameEverything
{
    public static class Patch_CaravanItemsTabUtility
    {
        [HarmonyPatch(typeof(CaravanItemsTabUtility))]
        [HarmonyPatch("DoRow")]
        [HarmonyPatch(new[] {typeof(Rect), typeof(TransferableImmutable), typeof(Caravan)})]
        public static class Patch_DrawThingRow
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var instructionList = instructions.ToList();

                var wordWrapInfo = AccessTools.Property(typeof(Text), nameof(Text.WordWrap)).GetSetMethod();
                var wordWraps = 0;

                var infoCardButtonInfo = AccessTools.Method(typeof(Widgets), nameof(Widgets.InfoCardButton),
                    new[] {typeof(float), typeof(float), typeof(Thing)});
                var anyThingInfo = AccessTools.Property(typeof(Transferable), nameof(Transferable.AnyThing))
                    .GetGetMethod();

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
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        instruction = new CodeInstruction(OpCodes.Call, doRenameFloatMenuButtonInfo);
                    }

                    // Reduce width of the label's rect to reduce risk of text clipping
                    if (instruction.opcode == OpCodes.Ldc_R4 && (float) instruction.operand == 300)
                    {
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Ldc_R4, 24);
                        instruction = new CodeInstruction(OpCodes.Sub);
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
                            yield return new CodeInstruction(OpCodes.Ldarg_1); // thing
                            yield return new CodeInstruction(OpCodes.Callvirt, anyThingInfo); // thing.AnyThing
                            instruction = new CodeInstruction(OpCodes.Call,
                                RenameUtility
                                    .ChangeGUIColourPreLabelDraw_Thing_Info); // ChangeGUIColourPreLabelDraw(thing.AnyThing)
                        }
                    }

                    yield return instruction;
                }
            }

            private static void DoRenameFloatMenuButton(ref Rect rect2, Rect rect, TransferableImmutable thing)
            {
                rect2.width -= 24;
                var renamableComp = thing.AnyThing.TryGetComp<CompRenamable>();
                if (renamableComp != null && Widgets.ButtonImage(new Rect(rect2.width - 24, rect.height - 24, 24, 24),
                    TexButton.RenameTex))
                {
                    Find.WindowStack.Add(new FloatMenu(RenameUtility
                        .CaravanRenameThingButtonFloatMenuOptions(renamableComp).ToList()));
                }
            }
        }
    }
}