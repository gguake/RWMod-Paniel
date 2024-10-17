using System;
using Verse;

namespace ModuleAutomata
{
    internal static class PNExtensions
    {
        private static AutomataModExtension _automataExtension;
        public static AutomataModExtension GetAutomataExtension()
        {
            if (_automataExtension == null)
            {
                _automataExtension = PNThingDefOf.Paniel_Race.GetModExtension<AutomataModExtension>();
            }
            return _automataExtension;
        }

        public static bool IsAutomata(this Pawn pawn)
        {
            return pawn.def == PNThingDefOf.Paniel_Race;
        }

        public static AutomataModuleSpec TryGetModuleSpec(this Pawn pawn, AutomataModulePartDef partDef)
        {
            if (!pawn.IsAutomata()) { return null; }

            if (partDef == PNAutomataModulePartDefOf.PN_Core)
            {
                var coreInfo = pawn.TryGetComp<CompAutomataCore>()?.CoreInfo;
                if (coreInfo == null) { return null; }

                return coreInfo.coreModuleDef.worker.TryGetModuleSpecFromPawn(pawn, partDef, coreInfo.coreModuleDef);
            }
            else if (partDef == PNAutomataModulePartDefOf.PN_Shell)
            {
                var automataInfo = pawn.TryGetComp<CompAutomata>();
                if (automataInfo == null) { return null; }

                return automataInfo.ShellModule;
            }
            else
            {
                var bodyPartRecord = partDef.FindBodyPartRecordFromPawn(pawn);

                if (!pawn.health.hediffSet.HasMissingPartFor(bodyPartRecord))
                {
                    foreach (var hediff in pawn.health.hediffSet.hediffs)
                    {
                        if (hediff.Part == bodyPartRecord)
                        {
                            var spec = partDef.TryGetModuleSpecFromHediff(hediff.def);
                            if (spec != null)
                            {
                                return spec;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
