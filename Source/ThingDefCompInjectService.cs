using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AutomataRace
{
    [StaticConstructorOnStartup]
    public static class ThingDefCompInjectService
    {
        static ThingDefCompInjectService()
        {
            foreach (var injectDef in DefDatabase<ThingDefCompInjectDef>.AllDefs)
            {
                var thingDef = DefDatabase<ThingDef>.GetNamed(injectDef.defName);
                if (thingDef == null)
                {
                    Log.Error($"thingDef '{injectDef.defName}' is not found. comp injection skipped.");
                    continue;
                }

                foreach (var comp in injectDef.comps)
                {
                    thingDef.comps.Add(comp);
                }
            }
        }
    }
}
