using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ModuleAutomata
{
    internal static class PNExtensions
    {
        private static Dictionary<ThingDef, HediffDef> _moduleThingToHediffCache;
        public static Dictionary<ThingDef, HediffDef> ModuleThingToHediffCache
        {
            get
            {
                if (_moduleThingToHediffCache == null)
                {
                    _moduleThingToHediffCache = new Dictionary<ThingDef, HediffDef>();
                    
                    foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
                    {
                        var moduleExt = thingDef.GetModExtension<AutomataModuleExtension>();
                        if (moduleExt != null && moduleExt.hediff != null)
                        {
                            _moduleThingToHediffCache.Add(thingDef, moduleExt.hediff);
                        }
                    }
                }

                return _moduleThingToHediffCache;
            }
        }

        private static Dictionary<HediffDef, ThingDef> _moduleHediffToThingCache;
        public static Dictionary<HediffDef, ThingDef> ModuleHediffToThingCache
        {
            get
            {
                if (_moduleHediffToThingCache == null)
                {
                    _moduleHediffToThingCache = new Dictionary<HediffDef, ThingDef>();

                    foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
                    {
                        var moduleExt = thingDef.GetModExtension<AutomataModuleExtension>();
                        if (moduleExt != null && moduleExt.hediff != null)
                        {
                            _moduleHediffToThingCache.Add(moduleExt.hediff, thingDef);
                        }
                    }
                }

                return _moduleHediffToThingCache;
            }
        }

        private static Dictionary<AutomataModulePart, HashSet<HediffDef>> _moduleHediffCache;
        public static Dictionary<AutomataModulePart, HashSet<HediffDef>> ModuleHediffCache
        {
            get
            {
                if (_moduleHediffCache == null)
                {
                    _moduleHediffCache = new Dictionary<AutomataModulePart, HashSet<HediffDef>>();

                    foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
                    {
                        var extension = thingDef.GetModExtension<AutomataModuleExtension>();
                        foreach (var part in extension.targetParts)
                        {
                            if (!_moduleHediffCache.TryGetValue(part, out var set))
                            {
                                set = new HashSet<HediffDef>();
                                _moduleHediffCache.Add(part, set);
                            }

                            set.Add(extension.hediff);
                        }
                    }
                }

                return _moduleHediffCache;
            }
        }

        private static Dictionary<AutomataModulePart, HashSet<ThingDef>> _moduleThingCache;
        public static Dictionary<AutomataModulePart, HashSet<ThingDef>> ModuleThingCache
        {
            get
            {
                if (_moduleThingCache == null)
                {
                    _moduleThingCache = new Dictionary<AutomataModulePart, HashSet<ThingDef>>();

                    foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
                    {
                        var extension = thingDef.GetModExtension<AutomataModuleExtension>();
                        foreach (var part in extension.targetParts)
                        {
                            if (!_moduleThingCache.TryGetValue(part, out var set))
                            {
                                set = new HashSet<ThingDef>();
                                _moduleThingCache.Add(part, set);
                            }

                            set.Add(thingDef);
                        }
                    }
                }

                return _moduleThingCache;
            }
        }

        public static bool IsAutomata(this Pawn pawn)
        {
            return pawn.def == PNThingDefOf.Paniel_Race;
        }

        public static IEnumerable<ThingDef> GetAutomataModule(this Pawn pawn, AutomataModulePart part)
        {
            var hediffSet = ModuleHediffCache[part];
            var hediffs = pawn.health.hediffSet.hediffs.Where(v => hediffSet.Contains(v.def));

            var moduleHediffToThingCache = ModuleHediffToThingCache;
            foreach (var hediff in hediffs)
            {
                yield return moduleHediffToThingCache[hediff.def];
            }
        }

        public static ThingDef GetDefaultAutomataModule(this AutomataModulePart part)
        {
            return ModuleThingCache[part].FirstOrDefault(v => v.GetModExtension<AutomataModuleExtension>().isDefault);
        }
    }
}
