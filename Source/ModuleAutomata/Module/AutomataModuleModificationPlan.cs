using System.Collections.Generic;
using Verse;

namespace ModuleAutomata
{
    public enum AutomataModuleModificationPlanType
    {
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
        public Dictionary<AutomataModulePartDef, AutomataModuleModificationPlan> plans = new Dictionary<AutomataModulePartDef, AutomataModuleModificationPlan>();

        public int hairAddonIndex;
        public HeadTypeDef headType;

        public void ExposeData()
        {
            Scribe_Collections.Look(ref plans, "plans", LookMode.Def, LookMode.Deep);
            Scribe_Values.Look(ref hairAddonIndex, "hairAddonIndex");
            Scribe_Deep.Look(ref headType, "headType");
        }
    }
}
