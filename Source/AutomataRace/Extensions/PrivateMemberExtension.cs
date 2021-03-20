using HarmonyLib;
using RimWorld;
using System.Reflection;

namespace AutomataRace.Extensions
{
    public static class PrivateMemberExtension
    {
        private static FieldInfo field_PawnStoryTracker_headGraphicPath = AccessTools.Field(typeof(Pawn_StoryTracker), "headGraphicPath");
        public static void SetHeadGraphicPath(this Pawn_StoryTracker story, string path)
        {
            field_PawnStoryTracker_headGraphicPath.SetValue(story, path);
        }
    }
}
