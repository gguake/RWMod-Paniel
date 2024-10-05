using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ModuleAutomata
{
    public class AutomataAssembleBill : IExposable
    {
        private Dictionary<AutomataModulePartDef, AutomataModuleBill> _modules = new Dictionary<AutomataModulePartDef, AutomataModuleBill>();

        public int HairAddonIndex
        {
            get => _hairAddonIndex;
            set
            {
                _hairAddonIndex = value;
            }
        }
        private int _hairAddonIndex;

        public HeadTypeDef HeadTypeDef
        {
            get => _headDef;
            set
            {
                _headDef = value;
            }
        }
        private HeadTypeDef _headDef;

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
            _headDef = PNThingDefOf.Paniel_Race.GetAvailableAlienHeadTypes().RandomElement();
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref _modules, "modules", LookMode.Def, LookMode.Def);
            Scribe_Values.Look(ref _hairAddonIndex, "hairAddonIndex");
            Scribe_Defs.Look(ref _headDef, "headDef");
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

            pawn.story.headType = _headDef;
            pawn.SetBodyAddonIndex(0, _hairAddonIndex);
        }
    }
}
