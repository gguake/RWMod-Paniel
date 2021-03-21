using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe.StatOverride
{
    public class StatOverrideService
    {
        private static Dictionary<ThingDef, Dictionary<StatDef, StatOverrideWorker>> _cache;

        public static StatOverrideWorker Get(ThingDef thingDef, StatDef statDef)
        {
            if (_cache == null)
            {
                _cache = new Dictionary<ThingDef, Dictionary<StatDef, StatOverrideWorker>>();
                foreach (var def in DefDatabase<StatOverrideDef>.AllDefs)
                {
                    ThingDef td = def.thingDef;
                    StatDef sd = def.statDef;
                    StatOverrideWorker worker = def.worker;

                    Dictionary<StatDef, StatOverrideWorker> dict;
                    if (!_cache.TryGetValue(td, out dict))
                    {
                        dict = new Dictionary<StatDef, StatOverrideWorker>();
                        _cache[td] = dict;
                    }

                    dict[sd] = worker;
                }
            }

            Dictionary<StatDef, StatOverrideWorker> dictStat = null;
            if (_cache.TryGetValue(thingDef, out dictStat))
            {
                StatOverrideWorker worker;
                if (dictStat.TryGetValue(statDef, out worker))
                {
                    return worker;
                }
            }

            return null;
        }
    }
}
