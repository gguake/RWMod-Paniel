using System.Collections.Generic;
using Verse;
using RimWorld;

namespace AutomataRace
{
    public class HediffCondition
    {
        public HediffDef hediff;
        public CompProperties comp;
    }

    public class CompProperties_AddCompHediffSelector : CompProperties
    {
        public List<HediffCondition> hediffConditionalBlock = new List<HediffCondition>();

        public CompProperties_AddCompHediffSelector()
        {
            compClass = typeof(CompAddCompHediffConditional);
        }
    }
}
