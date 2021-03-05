﻿using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RenameEverything
{
    public static class Patch_MinifiedThing
    {
        [HarmonyPatch(typeof(MinifiedThing))]
        [HarmonyPatch(nameof(MinifiedThing.GetGizmos))]
        public static class Patch_GetGizmos
        {
            public static void Postfix(MinifiedThing __instance, ref IEnumerable<Gizmo> __result)
            {
                var innerThing = __instance.InnerThing;
                if (innerThing != null && innerThing.TryGetComp<CompRenamable>() is CompRenamable renamableComp)
                {
                    __result = __result.Concat(RenameUtility.GetRenamableCompGizmos(renamableComp));
                }
            }
        }
    }
}