using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe.StatOverride
{
    public abstract class StatOverrideWorker
    {
        public abstract void Apply(ref float statValue, StatDef statDef, Thing thing);
    }
}
