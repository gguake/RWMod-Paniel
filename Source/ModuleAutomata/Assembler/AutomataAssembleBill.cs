using Verse;

namespace ModuleAutomata
{
    public class AutomataAssembleBill : IExposable
    {
        public AutomataModificationPlan plan;
        public Pawn pawn;

        public int workAmountDone;

        public void ExposeData()
        {
            Scribe_Deep.Look(ref plan, "plan");
            Scribe_References.Look(ref pawn, "pawn");

            Scribe_Values.Look(ref workAmountDone, "workAmountDone");
        }
    }
}
