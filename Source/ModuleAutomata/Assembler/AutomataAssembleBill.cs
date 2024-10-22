using Verse;

namespace ModuleAutomata
{
    public class AutomataAssembleBill : IExposable
    {
        public Building_AutomataAssembler building;

        public AutomataModificationPlan plan;
        public Pawn pawn;
        public float lastWorkAmount = -1;

        public bool IsStarted => lastWorkAmount >= 0;

        public AutomataAssembleBill(Building_AutomataAssembler building)
        {
            this.building = building;
        }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref plan, "plan");
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Values.Look(ref lastWorkAmount, "lastWorkAmount");
        }

        public void Start()
        {
            lastWorkAmount = plan.TotalWorkAmount;
        }
    }
}
