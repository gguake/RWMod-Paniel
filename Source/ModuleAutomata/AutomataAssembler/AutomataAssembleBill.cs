using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ModuleAutomata
{
    public class AutomataAssembleBill : IExposable
    {
        private Dictionary<AutomataModulePartDef, AutomataModuleBill> _modules = new Dictionary<AutomataModulePartDef, AutomataModuleBill>();

        public bool AllModuleAcquired => DefDatabase<AutomataModulePartDef>.AllDefsListForReading.Where(def => def.required).Count() == _modules.Keys.Count;

        public AutomataModuleBill this[AutomataModulePartDef partDef]
        {
            get
            {
                return _modules.TryGetValue(partDef, out var module) ? module : default;
            }
            set
            {
                _modules[partDef] = value;
            }
        }

        public AutomataAssembleBill()
        {
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref _modules, "modules", LookMode.Def, LookMode.Def);
        }

        public void ApplyPawn(Pawn pawn)
        {
            foreach (var moduleBill in _modules.Values)
            {
                foreach (var prop in moduleBill.moduleDef.properties)
                {
                    prop.OnApplyPawn(pawn, moduleBill);
                }
            }
        }
    }
}
