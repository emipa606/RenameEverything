using Verse;

namespace RenameEverything
{
    public class CompProperties_Renamable : CompProperties
    {
        public string inspectStringTranslationKey = "RenameEverything.Object";

        public string renameTranslationKey = "RenameEverything.RenameObject";

        public CompProperties_Renamable()
        {
            compClass = typeof(CompRenamable);
        }
    }
}