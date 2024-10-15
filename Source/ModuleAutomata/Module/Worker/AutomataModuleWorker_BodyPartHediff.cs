using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ModuleAutomata
{
    public class AutomataModuleWorker_BodyPartHediff : AutomataModuleWorker
    {
        public List<QualityHediff> hediffs;

        public override void OnApplyPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleSpec spec)
        {
            var quality = spec.moduleDef.affectedByQuality ? spec.Quality : QualityCategory.Normal;

            var hediffDef = hediffs.FirstOrDefault(qh => qh.quality == quality)?.hediff;
            if (hediffDef != null)
            {
                pawn.health.AddHediff(hediffDef, partDef.FindBodyPartRecordFromPawn(pawn));
            }
        }
    }
}
