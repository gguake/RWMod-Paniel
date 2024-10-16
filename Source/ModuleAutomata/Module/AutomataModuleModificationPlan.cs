using System.Collections.Generic;
using Verse;

namespace ModuleAutomata
{
    public enum AutomataModuleModificationPlanType
    {
        None,
        Preserve,
        Replace,
    }

    public struct AutomataModuleModificationPlan : IExposable
    {
        public AutomataModuleSpec spec;
        public AutomataModuleModificationPlanType plan;

        public void ExposeData()
        {
            Scribe_Deep.Look(ref spec, "spec");
            Scribe_Values.Look(ref plan, "plan");
        }
    }

    public class AutomataModificationPlan : IExposable
    {
        public AutomataModuleModificationPlan this[AutomataModulePartDef part]
        {
            get
            {
                if (_plans.TryGetValue(part, out var plan))
                {
                    return plan;
                }

                return default;
            }
            set
            {
                _plans[part] = value;
            }
        }
        private Dictionary<AutomataModulePartDef, AutomataModuleModificationPlan> _plans = new Dictionary<AutomataModulePartDef, AutomataModuleModificationPlan>();

        public int hairAddonIndex;
        public HeadTypeDef headType;

        public void ExposeData()
        {
            Scribe_Collections.Look(ref _plans, "plans", LookMode.Def, LookMode.Deep);
            Scribe_Values.Look(ref hairAddonIndex, "hairAddonIndex");
            Scribe_Deep.Look(ref headType, "headType");
        }

        public void ApplyPawn(Pawn pawn)
        {
        }
    }
}
