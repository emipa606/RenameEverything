using System.Collections.Generic;
using ColourPicker;
using Multiplayer.API;
using UnityEngine;
using Verse;

namespace RenameEverything
{
    public class Command_RecolourLabel : Command_Renamable
    {
        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            Find.WindowStack.Add(new Dialog_ColourPicker(renamables[0].labelColour, c => Callback(c, renamables)));
        }

        [SyncMethod]
        private void Callback(Color c, List<CompRenamable> renamableCompList)
        {
            renamableCompList.ForEach(r => r.labelColour = c);
        }
    }
}