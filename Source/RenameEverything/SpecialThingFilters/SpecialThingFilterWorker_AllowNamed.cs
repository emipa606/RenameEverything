using Verse;

namespace RenameEverything
{
    public class SpecialThingFilterWorker_AllowNamed : SpecialThingFilterWorker
    {
        public override bool Matches(Thing t)
        {
            return CanEverMatch(t.def) && t.TryGetComp<CompRenamable>().Named;
        }

        public override bool CanEverMatch(ThingDef def)
        {
            return def.HasComp(typeof(CompRenamable));
        }
    }
}