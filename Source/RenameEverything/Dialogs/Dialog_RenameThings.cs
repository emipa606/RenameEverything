using System.Collections.Generic;
using Multiplayer.API;
using Verse;

namespace RenameEverything
{
    public class Dialog_RenameThings : Dialog_Rename
    {
        private readonly List<CompRenamable> renamableComps;

        public Dialog_RenameThings(List<CompRenamable> renamableComps)
        {
            this.renamableComps = renamableComps;

            if (renamableComps.Count == 1)
            {
                var renamable = renamableComps[0];
                curName = renamable.Named ? renamable.Name : renamable.parent.LabelCapNoCount;
            }
            else
            {
                curName = string.Empty;
            }
        }

        public Dialog_RenameThings(CompRenamable renamableComp)
        {
            renamableComps = new List<CompRenamable> {renamableComp};
            curName = renamableComp.Named ? renamableComp.Name : renamableComp.parent.LabelCapNoCount;
        }

        protected override AcceptanceReport NameIsValid(string name)
        {
            return true;
        }

        [SyncMethod]
        protected override void SetName(string name)
        {
            foreach (var renamableComp in renamableComps)
            {
                renamableComp.Name = name;
            }
        }
    }
}