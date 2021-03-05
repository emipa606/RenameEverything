using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RenameEverything
{
    public abstract class Command_Renamable : Command
    {
        public CompRenamable renamable;
        protected List<CompRenamable> renamables;

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            if (renamables == null)
            {
                renamables = new List<CompRenamable>();
            }

            renamables.Add(renamable);
        }

        public override bool InheritInteractionsFrom(Gizmo other)
        {
            if (renamables == null)
            {
                renamables = new List<CompRenamable>();
            }

            renamables.Add(((Command_Renamable) other).renamable);
            return false;
        }
    }
}