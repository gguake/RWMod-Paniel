using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

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

            harmony.Patch(AccessTools.Method(typeof(PawnGenerator), "GenerateSkills"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(PawnGenerator_GenerateSkills_Postfix)));

            harmony.Patch(AccessTools.Method(typeof(Pawn_NeedsTracker), "ShouldHaveNeed"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Pawn_NeedsTracker_ShouldHaveNeed_Postfix)));

            harmony.Patch(AccessTools.Method(typeof(PawnDiedOrDownedThoughtsUtility), "GetThoughts"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(PawnDiedOrDownedThoughtsUtility_GetThoughts_Postfix)));

            harmony.Patch(AccessTools.Method(typeof(InteractionUtility), "CanInitiateInteraction"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(InteractionUtility_CanInitiateInteraction_Postfix)));

            harmony.Patch(AccessTools.Method(typeof(InteractionUtility), "CanReceiveInteraction"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(InteractionUtility_CanReceiveInteraction_Postfix)));
            
            //harmony.Patch(AccessTools.Method(typeof(Pawn_SkillTracker), "SkillsTick"),
            //    prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(Pawn_SkillTracker_SkillsTick_Prefix)));

            harmony.Patch(AccessTools.Method(typeof(Pawn_SkillTracker), "SkillsTick"),
                transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(Pawn_SkillTracker_SkillsTick_Transpiler)));

            harmony.Patch(AccessTools.Method(typeof(HediffComp_Infecter), "CheckMakeInfection"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HediffComp_Infecter_CheckMakeInfection_Prefix)));

            harmony.Patch(AccessTools.Method(typeof(HealthAIUtility), "FindBestMedicine"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(HealthAIUtility_FindBestMedicine_Prefix)));

            harmony.Patch(AccessTools.Method(typeof(GenRecipe), "PostProcessProduct"),
                transpiler: new HarmonyMethod(typeof(HarmonyPatches), nameof(GenRecipe_PostProcessProduct_Transpiler)));

            harmony.Patch(AccessTools.Method(typeof(TransferableUtility), "CanStack"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(TransferableUtility_CanStack_Postfix)));

            harmony.Patch(AccessTools.Method(typeof(PawnGenerator), "GeneratePawn", parameters: new Type[] { typeof(PawnGenerationRequest) }),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(PawnGenerator_GeneratePawn_Postfix)));

        }

        public static void PawnGenerator_GenerateSkills_Postfix(Pawn pawn)
        {
            var raceSettings = AutomataRaceSettingCache.Get(pawn.def);
            if (raceSettings == null)
            {
                return;
            }

            foreach (var skillDef in raceSettings.conflictingPassions)
            {
                if (pawn.skills == null)
                {
                    continue;
                }

                var skillRecord = pawn.skills.GetSkill(skillDef);
                if (skillRecord == null)
                {
                    continue;
                }

                skillRecord.passion = Passion.None;
            }
        }

        public static void PawnDiedOrDownedThoughtsUtility_GetThoughts_Postfix(Pawn victim, ref DamageInfo? dinfo, PawnDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtToAddToAll> outAllColonistsThoughts)
        {
            var raceSettings = AutomataRaceSettingCache.Get(victim.def);
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
                var raceSettings = AutomataRaceSettingCache.Get(___pawn.def);
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

        public static IEnumerable<CodeInstruction> Pawn_SkillTracker_SkillsTick_Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator ilGenerator)
        {
            var instructions = codeInstructions.ToList();
            var automataRaceSettings = ilGenerator.DeclareLocal(typeof(AutomataRaceSettings));

            var labelStart = ilGenerator.DefineLabel();
            var labelEnd = (Label)instructions[instructions.FindIndex(v => v.opcode == OpCodes.Call && (MethodInfo)v.operand == AccessTools.Method(typeof(GenLocalDate), "HourInteger", parameters: new Type[] { typeof(Thing) })) + 1].operand;

            foreach (var instruction in instructions)
            {
                if (instruction.operand is Label && (Label)instruction.operand == labelEnd)
                {
                    instruction.operand = labelStart;
                }
            }

            var injections = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labelStart),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Pawn_SkillTracker), "pawn")),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Pawn), "def")),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AutomataRaceSettingCache), "Get")),
                new CodeInstruction(OpCodes.Stloc_S, automataRaceSettings.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, automataRaceSettings.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, labelEnd),
                new CodeInstruction(OpCodes.Ldloc_S, automataRaceSettings.LocalIndex),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(AutomataRaceSettings), "skillDecayActivated")),
                new CodeInstruction(OpCodes.Brtrue_S, labelEnd),
                new CodeInstruction(OpCodes.Ret),
            };

            int i = instructions.FindIndex(v => v.opcode == OpCodes.Stfld && (FieldInfo)v.operand == AccessTools.Field(typeof(Pawn_SkillTracker), "lastXpSinceMidnightResetTimestamp")) + 1;
            instructions.InsertRange(i, injections);

            return instructions;
        }

        public static bool HediffComp_Infecter_CheckMakeInfection_Prefix(HediffComp_Infecter __instance)
        {
            Pawn pawn = __instance.Pawn;
            if (pawn != null)
            {
                var raceSettings = AutomataRaceSettingCache.Get(pawn.def);
                if (raceSettings != null)
                {
                    if (!raceSettings.infectionActivated)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool HealthAIUtility_FindBestMedicine_Prefix(Pawn patient, Thing __result)
        {
            var raceSettings = AutomataRaceSettingCache.Get(patient.def);
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

        public static IEnumerable<CodeInstruction> GenRecipe_PostProcessProduct_Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var instList = codeInstructions.ToList();
            int i = codeInstructions.FirstIndexOf(x => x.opcode == OpCodes.Brfalse_S);

            instList.InsertRange(i + 1, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ThingCompUtility), "TryGetComp", generics: new Type[] { typeof(CompAutomataDataHolder) })),
                new CodeInstruction(OpCodes.Brtrue_S, instList[i].operand)
            });

            return instList;
        }

        public static void TransferableUtility_CanStack_Postfix(ref bool __result, Thing thing)
        {
            if (thing.TryGetComp<CompAutomataDataHolder>() != null)
            {
                __result = false;
            }
        }

        public static void PawnGenerator_GeneratePawn_Postfix(ref Pawn __result)
        {
            if (__result.def == AutomataRaceDefOf.Paniel_Race)
            {
                __result.ageTracker.AgeBiologicalTicks = 0;
                __result.ageTracker.AgeChronologicalTicks = 0;
            }
        }
    }
}
