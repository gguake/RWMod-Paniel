using CustomizableRecipe.StatOverride;
using RimWorld;
using System;
using Verse;

namespace AutomataRace
{
    public class StatOverrideWorker_MarketValue_PackagedAutomaton : StatOverrideWorker
    {
        public StatOverrideWorker_MarketValue_PackagedAutomaton()
        {
        }

        public override void Apply(ref float statValue, StatDef statDef, Thing thing)
        {
            ThingWithComps thingWithComps = (ThingWithComps)thing;

            CompAutomataDataHolder comp = thingWithComps.TryGetComp<CompAutomataDataHolder>();
            if (comp != null)
            {
                foreach (var kv in comp.automataData.ingredients)
                {
                    statValue += kv.Key.BaseMarketValue * kv.Value;
                }
            }
        }
    }
}
