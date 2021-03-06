using Verse;

namespace AutomataRace
{
    public class CompProperties_SelfResurrect : CompProperties
    {
        public int resurrectDelayTick = 200;
        public HediffDef hediffCondition = null;
        public bool removeHediffAfterResurrect = true;

        public CompProperties_SelfResurrect()
        {
            compClass = typeof(CompSelfResurrect);
        }
    }
}
