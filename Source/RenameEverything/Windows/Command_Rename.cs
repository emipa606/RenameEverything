using UnityEngine;
using Verse;

namespace RenameEverything
{
    public class Command_Rename : Command_Renamable
    {
        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            Find.WindowStack.Add(new Dialog_RenameThings(renamables));
        }
    }
}