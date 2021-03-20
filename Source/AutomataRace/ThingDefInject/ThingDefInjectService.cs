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
                        thingDef.recipes = thingDef.recipes.Concat(injectDef.recipes).ToList();
                    }

                    if (injectDef.comps?.Count > 0)
                    {
                        thingDef.comps = thingDef.comps.Concat(injectDef.comps).ToList();
                    }
                }
            }
        }
    }
}
