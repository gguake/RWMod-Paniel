using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace AutomataRace.Extensions
{
    public static class PrivateMemberExtension
    {
        private static FieldInfo field_PawnStoryTracker_headGraphicPath = AccessTools.Field(typeof(Pawn_StoryTracker), "headGraphicPath");
        public static void SetHeadGraphicPath(this Pawn_StoryTracker story, string path)
        {
            field_PawnStoryTracker_headGraphicPath.SetValue(story, path);
        }

        private static FieldInfo field_Def_cachedLabelCap = AccessTools.Field(typeof(Def), "cachedLabelCap");
        public static void SetLabelCap(this Def def, TaggedString str)
        {
            field_Def_cachedLabelCap.SetValue(def, str);
        }
    }
}
