using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace RenameEverything
{
    public static class Patch_Pawn
    {
        [HarmonyPatch(typeof(Pawn))]
        [HarmonyPatch(nameof(Pawn.GetInspectString))]
        public static class Patch_GetInspectString
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var instructionList = instructions.ToList();

                var thingLabelInfo = AccessTools.Property(typeof(Entity), nameof(Entity.Label)).GetGetMethod();
                var adjustedEquippedInspectStringInfo = AccessTools.Method(typeof(Patch_GetInspectString),
                    nameof(AdjustedEquippedInspectString));

                foreach (var codeInstruction in instructionList)
                {
                    var instruction = codeInstruction;

                    if (instruction.opcode == OpCodes.Callvirt && (MethodInfo) instruction.operand == thingLabelInfo)
                    {
                        yield return instruction; // this.equipment.Primary.Label
                        yield return new CodeInstruction(OpCodes.Ldarg_0); // this
                        instruction =
                            new CodeInstruction(OpCodes.Call,
                                adjustedEquippedInspectStringInfo); // AdjustedEquippedInspectString(this.equipment.Primary.Label, this)
                    }

                    yield return instruction;
                }
            }

            public static string AdjustedEquippedInspectString(string original, Pawn instance)
            {
                // Integration with Dual Wield
                if (!ModCompatibilityCheck.DualWield || !RenameEverythingSettings.dualWieldInspectString)
                {
                    return original;
                }

                var equipmentTracker = instance.equipment;
                if (equipmentTracker == null)
                {
                    return original;
                }

                var primary = equipmentTracker.Primary;
                if (primary != null && ReflectedMethods.TryGetOffHandEquipment(equipmentTracker, out var secondary))
                {
                    return $"{original} {"AndLower".Translate()} {secondary.LabelCap}";
                }

                return original;
            }
        }
    }
}