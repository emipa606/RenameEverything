﻿using HarmonyLib;
using RimWorld;
using Verse;

namespace RenameEverything
{
    public static class Patch_Thing
    {
        [HarmonyPatch(typeof(Thing))]
        [HarmonyPatch(nameof(Thing.DrawGUIOverlay))]
        public static class Patch_DrawGUIOverlay
        {
            public static void Postfix(Thing __instance)
            {
                // If the thing doesn't have a GUI overlay for stack count or quality but is renamable and named, do our GUI overlay
                if (Find.CameraDriver.CurrentZoom == CameraZoomRange.Closest &&
                    RenameUtility.CanDrawThingName(__instance) && __instance.def.stackLimit <= 1 &&
                    !__instance.def.HasComp(typeof(CompQuality)))
                {
                    RenameUtility.DrawThingName(__instance);
                }
            }
        }
    }
}