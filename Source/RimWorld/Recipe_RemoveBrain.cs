using AutomataRace.Extensions;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace AutomataRace
{
    public class Recipe_RemoveBrain : Recipe_Surgery
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			if (!pawn.RaceProps.IsFlesh)
            {
				yield break;
            }

			var brain = pawn.health.hediffSet.GetBrain();
			var spine = pawn.health.hediffSet.GetSpine();

			if (brain == null || spine == null)
            {
				yield break;
            }

			yield return brain;
		}

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

		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			// part = brain
			var spine = pawn.health.hediffSet.GetSpine();

			bool isClean = MedicalRecipesUtility.IsClean(pawn, part);
			bool isViolation = IsViolationOnPawn(pawn, part, Faction.OfPlayer);
			if (billDoer != null)
			{
				if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
				{
					return;
				}

				TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);
				GenSpawn.Spawn(AutomataRaceDefOf.PN_Brain, billDoer.Position, billDoer.Map);
			}

			pawn.TakeDamage(new DamageInfo(DamageDefOf.SurgicalCut, 99999f, 999f, -1f, null, part));
			pawn.health.AddHediff(HediffDefOf.MissingBodyPart, spine, new DamageInfo(DamageDefOf.SurgicalCut, 99999f, 999f, -1f, null, spine));

			if (isClean)
			{
				if (pawn.Dead)
				{
					ThoughtUtility.GiveThoughtsForPawnExecuted(pawn, PawnExecutionKind.OrganHarvesting);
				}
				ThoughtUtility.GiveThoughtsForPawnOrganHarvested(pawn);
			}
			if (isViolation)
			{
				ReportViolation(pawn, billDoer, pawn.FactionOrExtraMiniOrHomeFaction, -70, "GoodwillChangedReason_RemovedBodyPart".Translate(part.LabelShort));
			}
		}

		public override string GetLabelWhenUsedOn(Pawn pawn, BodyPartRecord part)
		{
			if (pawn.RaceProps.IsMechanoid || pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(part))
			{
				return RecipeDefOf.RemoveBodyPart.label;
			}
			switch (HealthUtility.PartRemovalIntent(pawn, part))
			{
				case BodyPartRemovalIntent.Amputate:
					if (part.depth == BodyPartDepth.Inside || part.def.socketed)
					{
						return "RemoveOrgan".Translate();
					}
					return "Amputate".Translate();
				case BodyPartRemovalIntent.Harvest:
					return "HarvestOrgan".Translate();
				default:
					throw new InvalidOperationException();
			}
		}
	}
}
