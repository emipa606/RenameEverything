﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using Verse;

namespace RenameEverything
{
    public static class Patch_RPGStyleInventory_Sandy_Detailed_RPG_GearTab
    {
        public static class ManualPatch_DrawThingRow1
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var instructionList = instructions.ToList();

                var infoCardButtonInfo = AccessTools.Method(typeof(Widgets), nameof(Widgets.InfoCardButton),
                    new[] {typeof(float), typeof(float), typeof(Thing)});

                var doRenameFloatMenuButtonInfo =
                    AccessTools.Method(typeof(ManualPatch_DrawThingRow1), nameof(DoRenameFloatMenuButton));

                foreach (var codeInstruction in instructionList)
                {
                    var instruction = codeInstruction;

                    // Do our 'renamable gizmos substitute' button below the info card button
                    if (instruction.opcode == OpCodes.Call && (MethodInfo) instruction.operand == infoCardButtonInfo)
                    {
                        yield return instruction;
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Ldarg_2);
                        instruction = new CodeInstruction(OpCodes.Call, doRenameFloatMenuButtonInfo);
                    }

                    yield return instruction;
                }
            }

            private static void DoRenameFloatMenuButton(Rect rect, Thing thing)
            {
                var renamableComp = thing.TryGetComp<CompRenamable>();
                if (renamableComp != null &&
                    Widgets.ButtonImage(new Rect(rect.x, rect.y + 24, 24, 24), TexButton.RenameTex))
                {
                    Find.WindowStack.Add(new FloatMenu(RenameUtility
                        .CaravanRenameThingButtonFloatMenuOptions(renamableComp).ToList()));
                }
            }
        }
    }
}