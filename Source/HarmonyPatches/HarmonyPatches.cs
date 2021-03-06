using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using RimWorld;
using HarmonyLib;

namespace AutomataRace.HarmonyPatches
{
    [DefOf]
    public static partial class HarmonyPatchDefOf
    {
    }

    [StaticConstructorOnStartup]
    public static partial class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new Harmony("gguake.automatarace");

            harmony.Patch(AccessTools.Method(typeof(Pawn_NeedsTracker), "ShouldHaveNeed"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Pawn_NeedsTracker_ShouldHaveNeed_Postfix)));

            harmony.Patch(AccessTools.Method(typeof(PawnDiedOrDownedThoughtsUtility), "GetThoughts"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(PawnDiedOrDownedThoughtsUtility_GetThoughts_Postfix)));

            harmony.Patch(AccessTools.Method(typeof(InteractionUtility), "CanInitiateInteraction"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(InteractionUtility_CanInitiateInteraction_Postfix)));

            harmony.Patch(AccessTools.Method(typeof(InteractionUtility), "CanReceiveInteraction"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(InteractionUtility_CanReceiveInteraction_Postfix)));
            
            harmony.Patch(AccessTools.Method(typeof(Pawn_SkillTracker), "SkillsTick"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Pawn_SkillTracker_SkillsTick_Prefix)));

            harmony.Patch(AccessTools.Method(typeof(HediffComp_Infecter), "CheckMakeInfection"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HediffComp_Infecter_CheckMakeInfection_Prefix)));

            harmony.Patch(AccessTools.Method(typeof(HealthAIUtility), "FindBestMedicine"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HealthAIUtility_FindBestMedicine_Prefix)));
        }

        public static void PawnDiedOrDownedThoughtsUtility_GetThoughts_Postfix(Pawn victim, ref DamageInfo? dinfo, PawnDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtToAddToAll> outAllColonistsThoughts)
        {
            var raceSettings = DefDatabase<AutomataRaceSettings>.GetNamed(victim.def.defName, errorOnFail: false);
            if (!(raceSettings?.deadThoughtOverrides?.NullOrEmpty() ?? true))
            {
                var oldIndividualThoughts = new List<IndividualThoughtToAdd>(outIndividualThoughts);
                var oldAllColonistsThoughts = new List<ThoughtToAddToAll>(outAllColonistsThoughts);

                outIndividualThoughts.Clear();
                foreach (var individualThought in oldIndividualThoughts)
                {
                    var overrideData = raceSettings.deadThoughtOverrides.FirstOrDefault(x => x.source == individualThought.thought.def);
                    if (overrideData != null)
                    {
                        if (overrideData.overwrite != null)
                        {
                            var oldThought = individualThought.thought;
                            var newThought = new IndividualThoughtToAdd(
                                overrideData.overwrite,
                                individualThought.addTo,
                                otherPawn: oldThought.otherPawn,
                                moodPowerFactor: oldThought.moodPowerFactor);

                            var newThoughtSocial = newThought.thought as Thought_MemorySocial;
                            if (newThoughtSocial != null)
                            {
                                newThoughtSocial.opinionOffset = (oldThought as Thought_MemorySocial)?.opinionOffset ?? newThoughtSocial.opinionOffset;
                            }

                            outIndividualThoughts.Add(newThought);
                        }
                    }
                    else
                    {
                        outIndividualThoughts.Add(individualThought);
                    }
                }

                outAllColonistsThoughts.Clear();
                foreach (var allColonistThought in oldAllColonistsThoughts)
                {
                    var overrideData = raceSettings.deadThoughtOverrides.FirstOrDefault(x => x.source == allColonistThought.thoughtDef);
                    if (overrideData != null)
                    {
                        if (overrideData.overwrite != null)
                        {
                            outAllColonistsThoughts.Add(new ThoughtToAddToAll(overrideData.overwrite, allColonistThought.otherPawn));
                        }
                    }
                    else
                    {
                        outAllColonistsThoughts.Add(allColonistThought);
                    }
                }
            }
        }

        public static bool Pawn_SkillTracker_SkillsTick_Prefix(Pawn ___pawn, Pawn_SkillTracker __instance, ref int ___lastXpSinceMidnightResetTimestamp)
        {
            if (___pawn.IsHashIntervalTick(200))
            {
                var raceSettings = DefDatabase<AutomataRaceSettings>.GetNamed(___pawn.def.defName, errorOnFail: false);
                if (raceSettings != null)
                {
                    if (!raceSettings.skillDecayActivated)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool HediffComp_Infecter_CheckMakeInfection_Prefix(HediffComp_Infecter __instance)
        {
            Pawn pawn = __instance.Pawn;
            if (pawn != null)
            {
                AutomataRaceSettings automataRaceSettings = DefDatabase<AutomataRaceSettings>.GetNamed(pawn.def.defName, errorOnFail: false);
                if (automataRaceSettings != null)
                {
                    if (!automataRaceSettings.infectionActivated)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool HealthAIUtility_FindBestMedicine_Prefix(Pawn patient, Thing __result)
        {
            var raceSettings = DefDatabase<AutomataRaceSettings>.GetNamed(patient.def.defName, errorOnFail: false);
            if (raceSettings != null)
            {
                if (!raceSettings.medicineTendable)
                {
                    __result = null;
                    return false;
                }
            }

            return true;
        }
    }
}
