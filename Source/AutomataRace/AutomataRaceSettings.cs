using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AutomataRace
{
    public class DeadThoughtOverride
    {
        public ThoughtDef source = null;
        public ThoughtDef overwrite = null;
    }

    public class AutomataRaceSettings : Def
    {
        public List<NeedDef> needBlacklists = new List<NeedDef>();

        public bool socialActivated = true;
        public bool skillDecayActivated = true;
        public bool infectionActivated = true;
        public bool medicineTendable = true;

        public List<DeadThoughtOverride> deadThoughtOverrides = new List<DeadThoughtOverride>();
        public List<SkillDef> conflictingPassions = new List<SkillDef>();
    }

    public class AutomataRaceSettingCache
    {
        private static Dictionary<ThingDef, AutomataRaceSettings> _cache;

        public static AutomataRaceSettings Get(ThingDef thingDef)
        {
            if (_cache == null)
            {
                _cache = new Dictionary<ThingDef, AutomataRaceSettings>();
                foreach (AutomataRaceSettings automataRaceSettings in DefDatabase<AutomataRaceSettings>.AllDefs)
                {
                    ThingDef td = DefDatabase<ThingDef>.GetNamed(automataRaceSettings.defName, errorOnFail: false);
                    if (td != null)
                    {
                        _cache.Add(td, automataRaceSettings);
                    }
                }
            }

            return _cache.TryGetValue(thingDef, null);
        }
    }
}
