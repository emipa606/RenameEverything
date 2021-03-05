using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace RenameEverything
{
    public static class Patch_TransferableUIUtility
    {
        [HarmonyPatch(typeof(TransferableUIUtility))]
        [HarmonyPatch(nameof(TransferableUIUtility.DrawTransferableInfo))]
        public static class Patch_InspectPaneOnGUI
        {
            public static void Prefix(Transferable trad, ref Color labelColor)
            {
                if (trad.HasAnyThing && trad.AnyThing.TryGetComp<CompRenamable>() is CompRenamable renamableComp)
                {
                    labelColor = renamableComp.labelColour;
                }
            }
        }
    }
}