using AutomataRace.ThingDefInject;
using System.Collections.Generic;
using Verse;

namespace AutomataRace
{
    public class ThingDefInjectDef : Def
    {
        public List<ThingDefInjectCondition> conditions;

        public List<RecipeDef> recipes;

        public List<CompProperties> comps;
    }
}
