using RimWorld;
using System.Linq;
using Verse;

namespace ModuleAutomata
{
    public class AutomataModuleWorker_BodyPartHediff : AutomataModuleWorkerWithHediff
    {
        public override void OnInstallToPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleSpec spec)
        {
            var quality = spec.moduleDef.affectedByQuality ? spec.Quality : QualityCategory.Normal;

            var hediffDef = hediffs.FirstOrDefault(qh => qh.quality == quality)?.hediff;
            if (hediffDef != null)
            {
                pawn.health.AddHediff(hediffDef, partDef.FindBodyPartRecordFromPawn(pawn));
            }
        }

        public override void OnUninstallFromPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleSpec spec)
        {
            throw new System.NotImplementedException();
        }

        public override AutomataModuleSpec TryGetModuleSpecFromPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleDef moduleDef)
        {
            var bodyPart = partDef.FindBodyPartRecordFromPawn(pawn);
            foreach (var hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff.Part == bodyPart)
                {
                    var pair = hediffs.FirstOrDefault(v => v.hediff == hediff.def);
                    if (pair != null)
                    {
                        return new AutomataModuleSpec_AnyOfThing()
                        {
                            moduleDef = moduleDef,
                            quality = pair.quality,
                        };
                    }
                }
            }

            return null;
        }
    }
}
