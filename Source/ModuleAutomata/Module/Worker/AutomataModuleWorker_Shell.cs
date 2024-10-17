using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ModuleAutomata
{
    public class AutomataModuleWorker_Shell : AutomataModuleWorker
    {
        public List<QualityMultiplier> qualityMultiplier;
        public List<StatModifier> statOffsets;
        public List<StatModifier> statFactors;

        private Dictionary<StatDef, StatModifier> _statOffsetCache;
        public StatModifier GetStatOffset(StatDef statDef)
        {
            if (_statOffsetCache == null)
            {
                _statOffsetCache = statOffsets.ToDictionary(v => v.stat, v => v);
            }

            return _statOffsetCache.GetWithFallback(statDef, null);
        }

        private Dictionary<StatDef, StatModifier> _statFactorCache;
        public StatModifier GetStatFactor(StatDef statDef)
        {
            if (_statFactorCache == null)
            {
                _statFactorCache = statFactors.ToDictionary(v => v.stat, v => v);
            }

            return _statFactorCache.GetWithFallback(statDef, null);
        }

        public override void OnInstallToPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleSpec spec)
        {
            pawn.GetComp<CompAutomata>().ShellModule = (AutomataModuleSpec_AnyOfThing)spec;
        }

        public override void OnUninstallFromPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleSpec spec)
        {
            pawn.GetComp<CompAutomata>().ShellModule = null;
        }

        public override AutomataModuleSpec TryGetModuleSpecFromPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleDef moduleDef)
        {
            return pawn.TryGetComp<CompAutomata>().ShellModule;
        }
    }
}
