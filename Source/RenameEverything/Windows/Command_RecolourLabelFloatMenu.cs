using System.Collections.Generic;
using ColourPicker;
using Multiplayer.API;
using UnityEngine;
using Verse;

namespace RenameEverything
{
    public class Command_RecolourLabelFloatMenu : Command_RenamablesFromPawn
    {
        protected override IEnumerable<FloatMenuOption> DoFloatMenuOptions()
        {
            foreach (var pawnRenamablesPair in allPawnRenamables)
            foreach (var renamable in pawnRenamablesPair.Second)
            {
                yield return new FloatMenuOption(FloatMenuOptionLabel(pawnRenamablesPair.First, renamable.parent),
                    () => Find.WindowStack.Add(new Dialog_ColourPicker(renamable.labelColour,
                        c => Callback(c, renamable))));
            }
        }

        [SyncMethod]
        private void Callback(Color c, CompRenamable renamableComp)
        {
            renamableComp.labelColour = c;
        }
    }
}