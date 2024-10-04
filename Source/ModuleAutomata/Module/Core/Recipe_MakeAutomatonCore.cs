using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ModuleAutomata
{
    public class MakeAutomatonCoreRecipeExtension : DefModExtension
    {
        public ThingDef targetCoreDef;
    }

    public class Recipe_MakeAutomatonCore : Recipe_Surgery
    {
        public override bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
        {
            if ((pawn.Faction == billDoerFaction || pawn.Faction == null) && !pawn.IsQuestLodger())
            {
                return false;
            }

            if (HealthUtility.PartRemovalIntent(pawn, part) == BodyPartRemovalIntent.Harvest)
            {
                return true;
            }

            return false;
        }

        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            yield return pawn.health.hediffSet.GetBodyPartRecord(PNBodyPartDefOf.Brain);
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            var isViolation = IsViolationOnPawn(pawn, part, Faction.OfPlayer);
            if (billDoer != null)
            {
                var billDoerSkillLevel = billDoer.skills.GetSkill(SkillDefOf.Medicine).Level;
                var billDoerInspired = billDoer.InspirationDef == PNInspirationDefOf.Inspired_Surgery;

                if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }

                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);
                pawn.health.hediffSet.GetDirectlyAddedPartFor(part)?.Notify_SurgicallyRemoved(billDoer);

                OnSurgerySuccess(pawn, part, billDoer, ingredients, bill);

                var quality = QualityUtility.GenerateQualityCreatedByPawn(billDoerSkillLevel, billDoerInspired);
                var automatonCore = ThingMaker.MakeThing(recipe.GetModExtension<MakeAutomatonCoreRecipeExtension>().targetCoreDef);
                automatonCore.TryGetComp<CompAutomataCore>().SetPawnInfo(pawn);
                automatonCore.TryGetComp<CompQuality>()?.SetQuality(quality, null);

                GenSpawn.Spawn(automatonCore, billDoer.Position, billDoer.Map);
            }

            pawn.TakeDamage(new DamageInfo(DamageDefOf.SurgicalCut, 99999f, 999f, -1f, null, part));

            pawn.Drawer.renderer.SetAllGraphicsDirty();

            ThoughtUtility.GiveThoughtsForPawnExecuted(pawn, billDoer, PawnExecutionKind.OrganHarvesting);

            if (isViolation)
            {
                ReportViolation(pawn, billDoer, pawn.HomeFaction, -70);
            }
        }

    }
}
