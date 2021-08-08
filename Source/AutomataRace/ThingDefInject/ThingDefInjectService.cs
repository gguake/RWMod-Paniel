using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AutomataRace
{
    [StaticConstructorOnStartup]
    public static class ThingDefInjectService
    {
        static ThingDefInjectService()
        {
            var injects = DefDatabase<ThingDefInjectDef>.AllDefs.ToList();
            if (injects.Count == 0)
            {
                return;
            }

            foreach (var thingDef in DefDatabase<ThingDef>.AllDefs)
            {
                foreach (var injectDef in injects)
                {
                    bool conditionCheck = true;
                    foreach (var condition in injectDef.conditions)
                    {
                        if (!condition.Check(thingDef))
                        {
                            conditionCheck = false;
                            break;
                        }
                    }

                    if (!conditionCheck)
                    {
                        continue;
                    }

#if DEBUG
                    Log.Message($"ThingDef injection {injectDef.defName} to {thingDef.defName}");
#endif

                    if (injectDef.recipes?.Count > 0)
                    {
                        if (thingDef.recipes.NullOrEmpty())
                        {
                            thingDef.recipes = new List<RecipeDef>();
                        }

                        thingDef.recipes = thingDef.recipes.Concat(injectDef.recipes).ToList();
                    }

                    if (injectDef.comps?.Count > 0)
                    {
                        if (thingDef.comps.NullOrEmpty())
                        {
                            thingDef.comps = new List<CompProperties>();
                        }

                        thingDef.comps = thingDef.comps.Concat(injectDef.comps).ToList();
                    }
                }
            }
        }
    }
}
