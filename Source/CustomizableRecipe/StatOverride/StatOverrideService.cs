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
        private static Dictionary<Def, Dictionary<StatDef, StatOverrideWorker>> _cache;

        public static StatOverrideWorker Get(Def def, StatDef statDef)
        {
            if (_cache == null)
            {
                _cache = new Dictionary<Def, Dictionary<StatDef, StatOverrideWorker>>();
                foreach (var statOverrideDef in DefDatabase<StatOverrideDef>.AllDefs)
                {
                    ThingDef td = statOverrideDef.thingDef;
                    StatDef sd = statOverrideDef.statDef;
                    StatOverrideWorker worker = statOverrideDef.worker;

                    Dictionary<StatDef, StatOverrideWorker> dict;
                    if (!_cache.TryGetValue(td, out dict))
                    {
                        dict = new Dictionary<StatDef, StatOverrideWorker>();
                        _cache[td] = dict;
                    }

                    dict[sd] = worker;
                }

                Log.Message($"StatOverrideService Cached: {_cache.Count}");
            }

            Dictionary<StatDef, StatOverrideWorker> dictStat = null;
            if (_cache.TryGetValue(def, out dictStat))
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
