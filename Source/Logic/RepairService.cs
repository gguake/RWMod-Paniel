using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

using AutomataRace;
namespace AutomataRace.Logic
{
    internal static class RepairService
    {
        public static void Repair(Pawn pawn)
        {
            foreach (Hediff_MissingPart hediff in pawn.health.hediffSet.GetMissingPartsCommonAncestors())
            {
                if (!pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(hediff.Part))
                {
                    pawn.health.RestorePart(hediff.Part);
                }
            }

            List<Hediff> injuries = pawn.health.hediffSet.hediffs.Where(x => IsCurableHediff(x)).ToList();
            foreach (Hediff hediff in injuries)
            {
                HealthUtility.CureHediff(hediff);
            }
        }

        internal static bool IsCurableHediff(Hediff hediff)
        {
            if (hediff.def == HediffDefOf.BloodLoss)
            {
                return true;
            }

            if (hediff.def == AutomataRaceDefOf.PN_OilLoss)
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
