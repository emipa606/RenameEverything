using System;
using HarmonyLib;
using Verse;

namespace RenameEverything
{
    [StaticConstructorOnStartup]
    public static class ReflectedMethods
    {
        public delegate V FuncOut<T, U, V>(T input, out U output);

        public static FuncOut<Pawn_EquipmentTracker, ThingWithComps, bool> TryGetOffHandEquipment;

        static ReflectedMethods()
        {
            // Convert DualWield.Ext_Pawn_EquipmentTracker.TryGetOffHandEquipment to a delegate
            if (ModCompatibilityCheck.DualWield)
            {
                TryGetOffHandEquipment = (FuncOut<Pawn_EquipmentTracker, ThingWithComps, bool>) Delegate.CreateDelegate(
                    typeof(FuncOut<Pawn_EquipmentTracker, ThingWithComps, bool>),
                    AccessTools.Method(
                        GenTypes.GetTypeInAnyAssembly("DualWield.Ext_Pawn_EquipmentTracker", "DualWield"),
                        "TryGetOffHandEquipment"));
            }
        }
    }
}