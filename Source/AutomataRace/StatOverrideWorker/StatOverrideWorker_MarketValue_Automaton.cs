using CustomizableRecipe.StatOverride;
using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace AutomataRace
{
    public class StatOverrideWorker_MarketValue_Automaton : StatOverrideWorker
    {
        public float addition = 0f;
        public float multiplier = 1f;

        public StatOverrideWorker_MarketValue_Automaton()
        {
        }

        public override void Apply(ref float statValue, StatDef statDef, Thing thing)
        {
            ThingWithComps thingWithComps = (ThingWithComps)thing;

            CompAutomataDataHolder comp = thingWithComps.TryGetComp<CompAutomataDataHolder>();
            statValue += Mathf.Max((comp?.MarketValue ?? 0f) * multiplier + addition, 0f);
        }
    }
}
