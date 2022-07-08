using RimWorld;
using System.Collections.Generic;

namespace AutomataRace
{
    public struct GeneratePawnSample
    {
        public int weight;

        public string pawnKindDefName;
    }

    public class CompProperties_UseEffectGeneratePawn : CompProperties_UseEffect
    {
        public string letterLabel;
        public string letterText;

        public List<GeneratePawnSample> samples = new List<GeneratePawnSample>();

        public CompProperties_UseEffectGeneratePawn()
        {
            compClass = typeof(CompUseEffect_GeneratePawn);
        }
    }
}
