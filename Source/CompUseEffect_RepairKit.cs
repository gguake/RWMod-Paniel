using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AutomataRace
{
    public class CompUseEffect_RepairKit : CompUseEffect
    {
        public CompProperties_UseEffectRepairKit Props => props as CompProperties_UseEffectRepairKit;
        
        public override void DoEffect(Pawn usedBy)
        {
            foreach (Hediff_MissingPart hediff in usedBy.health.hediffSet.GetMissingPartsCommonAncestors())
            {
                if (!usedBy.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(hediff.Part))
                {
                    usedBy.health.RestorePart(hediff.Part);
                }
            }

            List<Hediff> injuries = usedBy.health.hediffSet.hediffs.Where(x => x is Hediff_Injury && x.Visible && x.def.everCurableByItem).ToList();
            foreach (Hediff hediff in injuries)
            {
                HealthUtility.CureHediff(hediff);
            }
        }

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            failReason = null;
            if (!Props.thingDefs.NullOrEmpty() && !Props.thingDefs.Contains(p.def))
            {
                return false;
            }

            if (Props.blockUnnecessaryUse)
            {
                if (p.health.hediffSet.GetMissingPartsCommonAncestors().NullOrEmpty() && !p.health.hediffSet.hediffs.Any(x => x is Hediff_Injury && x.Visible && x.def.everCurableByItem))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
