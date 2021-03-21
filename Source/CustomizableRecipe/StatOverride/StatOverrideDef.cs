using CustomizableRecipe.StatOverride;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe
{
    public class StatOverrideDef : Def
    {
        public ThingDef thingDef;
        public StatDef statDef;
        public StatOverrideWorker worker;
    }
}
