using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ModuleAutomata
{
    public class AutomataModulePartDef : Def
    {
        private HashSet<AutomataModuleDef> _moduleDefCache;
        public HashSet<AutomataModuleDef> ModuleDefs
        {
            get
            {
                if (_moduleDefCache == null)
                {
                    _moduleDefCache = DefDatabase<AutomataModuleDef>.AllDefsListForReading
                        .Where(def => def.adaptParts.Contains(this))
                        .ToHashSet();
                }

                return _moduleDefCache;
            }
        }

        private Dictionary<HediffDef, AutomataModuleSpec> _hediffModuleDefCache;
        public AutomataModuleSpec TryGetModuleSpecFromHediff(HediffDef hediffDef)
        {
            if (_hediffModuleDefCache == null)
            {
                _hediffModuleDefCache = new Dictionary<HediffDef, AutomataModuleSpec>();

                foreach (var module in ModuleDefs)
                {
                    if (module.worker is AutomataModuleWorkerWithHediff workerWithHediff)
                    {
                        foreach (var v in workerWithHediff.hediffs)
                        {
                            _hediffModuleDefCache[v.hediff] = new AutomataModuleSpec_AnyOfThing()
                            {
                                moduleDef = module,
                                quality = v.quality,
                            };
                        }
                    }
                }
            }

            return _hediffModuleDefCache.TryGetValue(hediffDef, out var result) ? result : null;
        }

        public bool required;
        public string targetBodyCustomLabel;

        public BodyPartRecord FindBodyPartRecordFromPawn(Pawn pawn)
        {
            if (targetBodyCustomLabel == null) { return null; }

            return pawn.RaceProps.body.AllParts.FirstOrDefault(v => v.customLabel == targetBodyCustomLabel);
        }
    }
}
