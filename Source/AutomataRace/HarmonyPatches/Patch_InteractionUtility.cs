using Verse;

namespace AutomataRace.HarmonyPatches
{
    public static partial class HarmonyPatches
    {
        public static void InteractionUtility_CanInitiateInteraction_Postfix(Pawn pawn, ref bool __result)
        {
            if (__result)
            {
                var raceSettings = AutomataRaceSettingCache.Get(pawn.def);
                if (raceSettings != null)
                {
                    if (!raceSettings.socialActivated)
                    {
                        __result = false;
                    }
                }
            }
        }

        public static void InteractionUtility_CanReceiveInteraction_Postfix(Pawn pawn, ref bool __result)
        {
            if (__result)
            {
                var raceSettings = AutomataRaceSettingCache.Get(pawn.def);
                if (raceSettings != null)
                {
                    if (!raceSettings.socialActivated)
                    {
                        __result = false;
                    }
                }
            }
        }
    }
}
