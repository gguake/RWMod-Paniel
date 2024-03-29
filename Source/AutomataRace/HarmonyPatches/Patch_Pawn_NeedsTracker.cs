﻿using Verse;
using RimWorld;

namespace AutomataRace.HarmonyPatches
{
    public static partial class HarmonyPatchDefOf
    {
        public static NeedDef Mood;
    }

    public static partial class HarmonyPatches
    {
        public static void Pawn_NeedsTracker_ShouldHaveNeed_Postfix(NeedDef nd, Pawn ___pawn, ref bool __result)
        {
            var raceSettings = AutomataRaceSettingCache.Get(___pawn.def);
            if (raceSettings != null)
            {
                if (raceSettings.needBlacklists.Contains(nd))
                {
                    __result = false;
                }
            }
        }
    }
}
