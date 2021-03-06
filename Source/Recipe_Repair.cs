using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace AutomataRace
{
    public class Recipe_Repair : Recipe_Surgery
    {
        public override bool AvailableOnNow(Thing thing)
        {
            Pawn pawn = thing as Pawn;
            if (pawn == null)
            {
                return false;
            }

            return true;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            foreach (Hediff_MissingPart hediff in pawn.health.hediffSet.GetMissingPartsCommonAncestors())
            {
                if (!pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(hediff.Part))
                {
                    pawn.health.RestorePart(hediff.Part);
                }
            }

            var hediffList = pawn.health.hediffSet.hediffs.Where(x => IsCurableHediff(x)).ToList();
            foreach (var hediff in hediffList)
            {
                HealthUtility.CureHediff(hediff);
            }
        }

        internal bool IsCurableHediff(Hediff hediff)
        {
            if (hediff.def == HediffDefOf.BloodLoss)
            {
                return true;
            }

            if (hediff is Hediff_Injury && hediff.Visible && hediff.def.everCurableByItem)
            {
                return true;
            }

            return false;
        }
    }
}
