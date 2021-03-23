using CustomizableRecipe.StatOverride;
using RimWorld;
using System;
using Verse;

namespace AutomataRace
{
    public class StatOverrideWorker_MarketValue_Automaton : StatOverrideWorker
    {
        public StatOverrideWorker_MarketValue_Automaton()
        {
        }

        public override void Apply(ref float statValue, StatDef statDef, Thing thing)
        {
            ThingWithComps thingWithComps = (ThingWithComps)thing;

            CompAutomataDataHolder comp = thingWithComps.TryGetComp<CompAutomataDataHolder>();
            statValue += comp?.MarketValue ?? 0;
        }
    }
}
