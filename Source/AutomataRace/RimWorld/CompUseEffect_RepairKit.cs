using AutomataRace.Logic;
using RimWorld;
using System.Linq;
using Verse;

namespace AutomataRace
{
    public class CompUseEffect_RepairKit : CompUseEffect
    {
        public CompProperties_UseEffectRepairKit Props => props as CompProperties_UseEffectRepairKit;
        
        public override void DoEffect(Pawn usedBy)
        {
            RepairService.Repair(usedBy);
            RepairService.RecoverMaintenance(usedBy);
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
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
