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
}
