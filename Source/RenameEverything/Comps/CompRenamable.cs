using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RenameEverything
{
    public class CompRenamable : ThingComp
    {
        private string _name = string.Empty;
        public bool allowMerge;

        private string cachedLabel = string.Empty;
        public Color labelColour = Color.white;

        public CompProperties_Renamable Props => (CompProperties_Renamable) props;

        public bool Named
        {
            get => !_name.NullOrEmpty() && _name.ToLower() != cachedLabel.ToLower();
            set
            {
                if (!value)
                {
                    _name = string.Empty;
                }
            }
        }

        public string Name
        {
            get => Named ? _name : string.Empty;
            set => _name = value;
        }

        public bool Coloured
        {
            get => !labelColour.IndistinguishableFrom(Color.white);
            set
            {
                if (!value)
                {
                    labelColour = Color.white;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is CompRenamable otherRenamable)
            {
                return Name.Equals(otherRenamable.Name) &&
                       labelColour.IndistinguishableFrom(otherRenamable.labelColour);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool AllowStackWith(Thing other)
        {
            if (!base.AllowStackWith(other))
            {
                return false;
            }

            return allowMerge || Equals(other.TryGetComp<CompRenamable>());
        }

        public override string TransformLabel(string label)
        {
            cachedLabel = label;
            if (!Named)
            {
                return label;
            }

            var shouldAppendCachedLabel = RenameEverythingSettings.appendCachedLabel &&
                                          (!RenameEverythingSettings.onlyAppendInThingHolder ||
                                           ThingOwnerUtility.GetFirstSpawnedParentThing(parent) != parent);
            return Name + (shouldAppendCachedLabel ? $" ({cachedLabel.CapitalizeFirst()})" : string.Empty);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            return RenameUtility.GetRenamableCompGizmos(this);
        }

        public override void PostSplitOff(Thing piece)
        {
            var pieceCompRenamable = piece.TryGetComp<CompRenamable>();

            // Just a paranoid check
            if (pieceCompRenamable != null)
            {
                pieceCompRenamable.Name = Name;
                pieceCompRenamable.labelColour = labelColour;
                pieceCompRenamable.cachedLabel = cachedLabel;
                pieceCompRenamable.allowMerge = allowMerge;
            }
            else
            {
                Log.Warning($"pieceCompRenamable (piece={piece}) is null");
            }
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref cachedLabel, "cachedLabel", string.Empty);
            Scribe_Values.Look(ref _name, "name", string.Empty);
            Scribe_Values.Look(ref labelColour, "labelColour", Color.white);
            Scribe_Values.Look(ref allowMerge, "allowMerge");

            base.PostExposeData();
        }

        public override string CompInspectStringExtra()
        {
            return Named ? $"{Props.inspectStringTranslationKey.Translate()}: {cachedLabel.CapitalizeFirst()}" : null;
        }
    }
}