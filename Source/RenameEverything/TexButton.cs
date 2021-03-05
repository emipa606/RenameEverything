using UnityEngine;
using Verse;

namespace RenameEverything
{
    [StaticConstructorOnStartup]
    internal static class TexButton
    {
        public static readonly Texture2D DeleteX = ContentFinder<Texture2D>.Get("UI/Buttons/Delete");
        public static readonly Texture2D RenameTex = ContentFinder<Texture2D>.Get("UI/Buttons/Rename");
        public static readonly Texture2D RecolourTex = ContentFinder<Texture2D>.Get("UI/Buttons/Recolour");
        public static readonly Texture2D AllowMergingTex = ContentFinder<Texture2D>.Get("UI/Buttons/AllowMerging");
    }
}