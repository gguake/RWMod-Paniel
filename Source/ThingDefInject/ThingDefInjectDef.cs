using AutomataRace.ThingDefInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
