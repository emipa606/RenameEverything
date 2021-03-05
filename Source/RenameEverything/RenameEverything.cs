using HarmonyLib;
using UnityEngine;
using Verse;

namespace RenameEverything
{
    public class RenameEverything : Mod
    {
        public static Harmony HarmonyInstance;

        public RenameEverythingSettings settings;

        public RenameEverything(ModContentPack content) : base(content)
        {
            GetSettings<RenameEverythingSettings>();
            HarmonyInstance = new Harmony("XeoNovaDan.RenameEverything");
        }

        public override string SettingsCategory()
        {
            return "RenameEverything.SettingsCategory".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            GetSettings<RenameEverythingSettings>().DoWindowContents(inRect);
        }
    }
}