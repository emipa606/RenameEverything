using System.Collections.Generic;
using Verse;

namespace RenameEverything
{
    public class Command_RenameFloatMenu : Command_RenamablesFromPawn
    {
        protected override IEnumerable<FloatMenuOption> DoFloatMenuOptions()
        {
            foreach (var pawnRenamablesPair in allPawnRenamables)
            foreach (var renamable in pawnRenamablesPair.Second)
            {
                yield return new FloatMenuOption(FloatMenuOptionLabel(pawnRenamablesPair.First, renamable.parent),
                    () => Find.WindowStack.Add(new Dialog_RenameThings(renamable)));
            }
        }
    }
}