using CustomizableRecipe.StatOverride;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AutomataRace
{
    public class StatOverrideWorker_ArmorRating_Automaton : StatOverrideWorker
    {
        public float addition = 0f;
        public float multiplier = 1f;

        static Dictionary<StatDef, StatDef> _statStuffStatCache;
        static Dictionary<ThingDef, Dictionary<StatDef, float>> _stuffPowerCache;

        public StatOverrideWorker_ArmorRating_Automaton()
        {
        }

        public override void ResolveReferences()
        {
            _statStuffStatCache = new Dictionary<StatDef, StatDef>();
            _statStuffStatCache[StatDefOf.ArmorRating_Sharp] = StatDefOf.StuffPower_Armor_Sharp;
            _statStuffStatCache[StatDefOf.ArmorRating_Blunt] = StatDefOf.StuffPower_Armor_Blunt;
            _statStuffStatCache[StatDefOf.ArmorRating_Heat] = StatDefOf.StuffPower_Armor_Heat;

            _stuffPowerCache = new Dictionary<ThingDef, Dictionary<StatDef, float>>();
        }

        private static float GetMaterialStuffPower(ThingDef materialDef, StatDef statDef)
        {
            if (_statStuffStatCache.TryGetValue(statDef, out var stuffStatDef))
            {
                Dictionary<StatDef, float> statDict;
                if (!_stuffPowerCache.TryGetValue(materialDef, out statDict))
                {
                    statDict = new Dictionary<StatDef, float>();
                    _stuffPowerCache[materialDef] = statDict;

                    foreach (var kv in _statStuffStatCache)
                    {
                        statDict[kv.Key] = materialDef.statBases.FirstOrDefault(x => x.stat == kv.Value)?.value ?? 0f;
                    }
                }

                return statDict.TryGetValue(statDef, 0f);
            }

            return 0f;
        }

        public override void Apply(ref float statValue, StatDef statDef, Thing thing)
        {
            ThingWithComps thingWithComps = (ThingWithComps)thing;

            CompAutomataDataHolder comp = thingWithComps.TryGetComp<CompAutomataDataHolder>();
            if (comp != null)
            {
                float materialStat = GetMaterialStuffPower(comp.automataData.baseMaterialDef, statDef);
                // Log.Message($"{comp.automataData.baseMaterialDef.defName}, {statDef.defName}: {statValue} + {materialStat} = {statValue + materialStat}");
                statValue += Mathf.Max(materialStat * multiplier + addition, 0f);
            }
        }
    }
}
