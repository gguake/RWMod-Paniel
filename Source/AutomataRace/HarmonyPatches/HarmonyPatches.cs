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

            #region For Debug
            //harmony.Patch(AccessTools.Method(typeof(LifeStageWorker_HumanlikeAdult), "Notify_LifeStageStarted"),
            //    prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(LifeStageWorker_HumanlikeAdult_Notify_LifeStageStarted_Prefix)));
            #endregion

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

            harmony.Patch(AccessTools.Method(typeof(FactionDialogMaker), "FactionDialogFor"),
                postfix: new HarmonyMethod(typeof(HarmonyPatches), nameof(FactionDialogMaker_FactionDialogFor_Postfix)));

        }

        #region For Debug
        public static bool LifeStageWorker_HumanlikeAdult_Notify_LifeStageStarted_Prefix(Pawn pawn, LifeStageDef previousLifeStage)
        {
            Log.Message($"pawn: {pawn}");
            Log.Message($"previousLifeStage: {previousLifeStage}");
            Log.Message($"pawn.Map: {pawn?.Map}");
            Log.Message($"pawn.story: {pawn?.story}");

            return true;
        }
        #endregion

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
                if (GenLocalDate.HourInteger(___pawn) == 0 && (___lastXpSinceMidnightResetTimestamp < 0 || Find.TickManager.TicksGame - ___lastXpSinceMidnightResetTimestamp >= 30000))
                {
                    for (int i = 0; i < __instance.skills.Count; i++)
                    {
                        __instance.skills[i].xpSinceMidnight = 0f;
                    }
                    ___lastXpSinceMidnightResetTimestamp = Find.TickManager.TicksGame;
                }

                var raceSetting = AutomataRaceSettingCache.Get(___pawn.def);
                if (raceSetting?.skillDecayActivated ?? true)
                {
                    for (int j = 0; j < __instance.skills.Count; j++)
                    {
                        __instance.skills[j].Interval();
                    }
                }

                return false;
            }

            return true;
        }

        public static IEnumerable<CodeInstruction> Pawn_SkillTracker_SkillsTick_Transpiler(IEnumerable<CodeInstruction> codeInstructions, ILGenerator ilGenerator)
        {
            var instructions = codeInstructions.ToList();
            var automataRaceSettings = ilGenerator.DeclareLocal(typeof(AutomataRaceSettings));

            var labelToSkillInterval = ilGenerator.DefineLabel();

            // clear preivous labels and add my new label (for jumping in my injections)
            var hookIndex = instructions.FindIndex(v => v.opcode == OpCodes.Stfld && (FieldInfo)v.operand == AccessTools.Field(typeof(Pawn_SkillTracker), "lastXpSinceMidnightResetTimestamp")) + 1;
            List<Label> oldLabels = new List<Label>(instructions[hookIndex].labels);
            instructions[hookIndex].labels.Clear();
            instructions[hookIndex].labels.Add(labelToSkillInterval);
            
            var injections = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(oldLabels),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Pawn_SkillTracker), "pawn")),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(Pawn), "def")),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AutomataRaceSettingCache), "Get")),
                new CodeInstruction(OpCodes.Stloc_S, automataRaceSettings.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, automataRaceSettings.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, labelToSkillInterval),
                new CodeInstruction(OpCodes.Ldloc_S, automataRaceSettings.LocalIndex),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(AutomataRaceSettings), "skillDecayActivated")),
                new CodeInstruction(OpCodes.Brtrue_S, labelToSkillInterval),
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

        public static void FactionDialogMaker_FactionDialogFor_Postfix(ref DiaNode __result, Pawn negotiator, Faction faction)
        {
            var map = negotiator.Map;
            if (faction.def != AutomataRaceDefOf.PN_SubsidiaryFaction)
            {
                return;
            }

            Thing sourceItem = null;
            foreach (Building_OrbitalTradeBeacon item in Building_OrbitalTradeBeacon.AllPowered(map))
            {
                foreach (IntVec3 tradeableCell in item.TradeableCells)
                {
                    List<Thing> thingList = tradeableCell.GetThingList(map);
                    for (int i = 0; i < thingList.Count; i++)
                    {
                        Thing thing = thingList[i];
                        if (thing.def == AutomataRaceDefOf.PN_OTPCard)
                        {
                            sourceItem = thing;
                            break;
                        }
                    }
                }

                if (sourceItem != null) { break; }
            }

            var opt = new DiaOption("PN_REQUEST_ORBITAL_TRADER".Translate(AutomataConsts.OrbitalTraderRelationshipCost));
            if (negotiator.skills.GetSkill(SkillDefOf.Social).TotallyDisabled)
            {
                opt.Disable("WorkTypeDisablesOption".Translate(SkillDefOf.Social.label));
            }
            else if (faction.PlayerRelationKind != FactionRelationKind.Ally)
            {
                opt.Disable("MustBeAlly".Translate());
            }
            else if (sourceItem == null)
            {
                opt.Disable("PN_OTP_CARD_REQUIRED_IN_TRADE_BEACON".Translate());
            }
            else if (map.passingShipManager.passingShips
                    .Select(v => v as TradeShip)
                    .Any(v => v != null && v.def == AutomataRaceDefOf.PN_Orbital_PnLindustry))
            {
                opt.Disable("PN_ORBITAL_TRADER_ALREADY_ARRIVED".Translate());
            }

            if (!opt.disabled)
            {
                var incidentsEnumerator = Find.Storyteller.incidentQueue.GetEnumerator();
                while (incidentsEnumerator.MoveNext())
                {
                    var incident = incidentsEnumerator.Current as QueuedIncident;
                    if (incident?.FiringIncident?.def == IncidentDefOf.OrbitalTraderArrival &&
                        incident?.FiringIncident?.parms?.traderKind == AutomataRaceDefOf.PN_Orbital_PnLindustry &&
                        incident?.FiringIncident?.parms?.faction == faction)
                    {
                        opt.Disable("PN_ORBITAL_TRADER_ALREADY_CALLED".Translate());
                        break;
                    }
                }
            }

            DiaNode okNode = new DiaNode("PN_ORBITAL_TRADER_SENT".Translate(faction.leader).CapitalizeFirst());
            okNode.options.Add(new DiaOption("OK".Translate())
            {
                linkLateBind = () => FactionDialogMaker.FactionDialogFor(negotiator, faction)
            });

            opt.action = () =>
            {
                if (sourceItem.stackCount > 1)
                {
                    sourceItem.stackCount--;
                }
                else
                {
                    sourceItem.Destroy();
                }

                var incidentParams = new IncidentParms
                {
                    target = map,
                    faction = faction,
                    traderKind = AutomataRaceDefOf.PN_Orbital_PnLindustry,
                    forced = true,
                };

                var ticks = new IntRange(AutomataConsts.OrbitalTraderArrivalTicksMin, AutomataConsts.OrbitalTraderArrivalTicksMax).RandomInRange;
                Find.Storyteller.incidentQueue.Add(
                    IncidentDefOf.OrbitalTraderArrival,
                    Find.TickManager.TicksGame + ticks,
                    incidentParams,
                    300000);

                Faction.OfPlayer.TryAffectGoodwillWith(faction, -AutomataConsts.OrbitalTraderRelationshipCost, false, true, HistoryEventDefOf.RequestedTrader);
            };

            opt.link = okNode;

            __result.options.Insert(__result.options.Count - 1, opt);
        }
    }
}
