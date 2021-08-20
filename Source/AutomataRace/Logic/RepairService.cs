using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
namespace AutomataRace.Logic
{
    internal static class RepairService
    {
        public static void Repair(Pawn pawn)
        {
            List<Hediff_MissingPart> missings = pawn.health.hediffSet.GetMissingPartsCommonAncestors().Where(x => !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(x.Part)).ToList();
            foreach (var hediff in missings)
            {
                pawn.health.RestorePart(hediff.Part);
            }

            List<Hediff> injuries = pawn.health.hediffSet.hediffs.Where(x => IsCurableHediff(x)).ToList();
            foreach (var hediff in injuries)
            {
                HealthUtility.Cure(hediff);
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

        public static void RecoverMaintenance(Pawn pawn)
        {
            var need = pawn?.needs?.AllNeeds?.FirstOrDefault(x => x.def == AutomataRaceDefOf.PN_Need_Maintenance);
            if (need != null)
            {
                need.CurLevel = 1.0f;
            }
        }
    }
}
