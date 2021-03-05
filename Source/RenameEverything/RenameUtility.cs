using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColourPicker;
using HarmonyLib;
using Multiplayer.API;
using RimWorld;
using UnityEngine;
using Verse;

namespace RenameEverything
{
    public static class RenameUtility
    {
        private const int MaxTextWidth = 65;

        private static Color cachedGUIColour;

        public static MethodInfo ChangeGUIColourPreLabelDraw_IEnumerableThing_Info => AccessTools.Method(
            typeof(RenameUtility), nameof(ChangeGUIColourPreLabelDraw), new[] {typeof(IEnumerable<Thing>)});

        public static MethodInfo ChangeGUIColourPreLabelDraw_Thing_Info => AccessTools.Method(typeof(RenameUtility),
            nameof(ChangeGUIColourPreLabelDraw), new[] {typeof(Thing)});

        public static MethodInfo ChangeGUIColourPostLabelDraw_Info =>
            AccessTools.Method(typeof(RenameUtility), nameof(ChangeGUIColourPostLabelDraw));

        public static IEnumerable<Gizmo> GetRenamableCompGizmos(CompRenamable renamableComp)
        {
            var filler = renamableComp.Props.inspectStringTranslationKey.TranslateSimple().UncapitalizeFirst();
            // Rename
            yield return new Command_Rename
            {
                renamable = renamableComp,
                defaultLabel = renamableComp.Props.renameTranslationKey.Translate(),
                defaultDesc = "RenameEverything.RenameGizmo_Description".Translate(filler),
                icon = TexButton.RenameTex,
                hotKey = KeyBindingDefOf.Misc1
            };

            // Recolour label
            yield return new Command_RecolourLabel
            {
                renamable = renamableComp,
                defaultLabel = "RenameEverything.RecolourLabel".Translate(),
                defaultDesc = "RenameEverything.RecolourLabel_Description".Translate(filler),
                icon = TexButton.RecolourTex
            };

            if (!renamableComp.Named && !renamableComp.Coloured)
            {
                yield break;
            }

            // Allow merging
            if (renamableComp.parent.def.stackLimit > 1)
            {
                yield return new Command_Toggle
                {
                    defaultLabel = "RenameEverything.AllowMerging".Translate(),
                    defaultDesc = "RenameEverything.AllowMerging_Description".Translate(),
                    icon = TexButton.AllowMergingTex,
                    isActive = () => renamableComp.allowMerge,
                    toggleAction = () => AllowMergeGizmoToggleAction(renamableComp)
                };
            }

            // Remove name
            if (renamableComp.Named)
            {
                yield return new Command_Action
                {
                    defaultLabel = "RenameEverything.RemoveName".Translate(),
                    defaultDesc = "RenameEverything.RemoveName_Description".Translate(filler),
                    icon = TexButton.DeleteX,
                    action = () => RemoveNameGizmoAction(renamableComp)
                };
            }
        }

        [SyncMethod]
        private static void AllowMergeGizmoToggleAction(CompRenamable renamableComp)
        {
            renamableComp.allowMerge = !renamableComp.allowMerge;
        }

        [SyncMethod]
        private static void RemoveNameGizmoAction(CompRenamable renamableComp)
        {
            renamableComp.Named = false;
        }

        public static IEnumerable<CompRenamable> GetRenamableEquipmentComps(Pawn pawn)
        {
            // Equipment
            if (pawn.equipment != null)
            {
                foreach (var eq in pawn.equipment.AllEquipmentListForReading)
                {
                    if (eq.GetComp<CompRenamable>() is CompRenamable renamableComp)
                    {
                        yield return renamableComp;
                    }
                }
            }

            // Apparel
            if (pawn.apparel == null)
            {
                yield break;
            }

            {
                foreach (var ap in pawn.apparel.WornApparel)
                {
                    if (ap.GetComp<CompRenamable>() is CompRenamable renamableComp)
                    {
                        yield return renamableComp;
                    }
                }
            }
        }

        public static void ChangeGUIColourPreLabelDraw(IEnumerable<Thing> things)
        {
            if (things.Count() == 1)
            {
                ChangeGUIColourPreLabelDraw(things.First());
            }
            else
            {
                cachedGUIColour = GUI.color;
            }
        }

        public static void ChangeGUIColourPreLabelDraw(Thing thing)
        {
            // Store the current GUI labelColour and change the GUI labelColour to what's defined in renamableComp
            cachedGUIColour = GUI.color;
            if (thing.GetInnerIfMinified().TryGetComp<CompRenamable>() is CompRenamable renamableComp)
            {
                GUI.color = renamableComp.labelColour;
            }
        }

        public static void ChangeGUIColourPostLabelDraw()
        {
            // After the label has been drawn, change the labelColour back to the previous one
            GUI.color = cachedGUIColour;
        }

        public static IEnumerable<FloatMenuOption> CaravanRenameThingButtonFloatMenuOptions(CompRenamable renamableComp)
        {
            // Rename
            yield return new FloatMenuOption(renamableComp.Props.renameTranslationKey.Translate(),
                () => Find.WindowStack.Add(new Dialog_RenameThings(renamableComp)));

            // Recolour
            yield return new FloatMenuOption("RenameEverything.RecolourLabel".Translate(),
                () => Find.WindowStack.Add(new Dialog_ColourPicker(renamableComp.labelColour,
                    c => renamableComp.labelColour = c)));

            // Remove name
            if (renamableComp.Named)
            {
                yield return new FloatMenuOption("RenameEverything.RemoveName".Translate(),
                    () => renamableComp.Named = false);
            }
        }

        public static void DrawThingName(Thing thing)
        {
            if (!CanDrawThingName(thing, out var renamableComp))
            {
                return;
            }

            // Do background
            Text.Font = GameFont.Tiny;
            var screenPos = GenMapUI.LabelDrawPosFor(thing, -0.4f);
            var text = Text.CalcSize(renamableComp.Name).x <= MaxTextWidth
                ? renamableComp.Name
                : renamableComp.Name.Shorten().Truncate(MaxTextWidth);
            var x = Text.CalcSize(text).x;
            var backgroundRect = new Rect(screenPos.x - (x / 2) - 4, screenPos.y, x + 8, 12);
            GUI.DrawTexture(backgroundRect, TexUI.GrayTextBG);

            // Do label
            Text.Anchor = TextAnchor.UpperCenter;
            ChangeGUIColourPreLabelDraw(thing);
            var textRect = new Rect(screenPos.x - (x / 2), screenPos.y - 3, x, 999);
            Widgets.Label(textRect, text);
            ChangeGUIColourPostLabelDraw();

            // Finish off
            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;
        }

        private static bool CanDrawThingName(Thing t, out CompRenamable renamableComp)
        {
            renamableComp = t.TryGetComp<CompRenamable>();
            return renamableComp != null && renamableComp.Named && RenameEverythingSettings.showNameOnGround &&
                   !(t is Building);
        }

        public static bool CanDrawThingName(Thing t)
        {
            return CanDrawThingName(t, out _);
        }
    }
}