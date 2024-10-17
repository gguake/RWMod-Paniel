using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ModuleAutomata
{
    public class CompProperties_Automata : CompProperties
    {
        public CompProperties_Automata()
        {
            compClass = typeof(CompAutomata);
        }
    }

    public class CompAutomata : ThingComp
    {
        public AutomataModuleSpec_AnyOfThing ShellModule
        {
            get => _shellModule;
            set
            {
                _shellModule = value;
                RefreshStatOffsetCache();
            }
        }
        private AutomataModuleSpec_AnyOfThing _shellModule;

        private Dictionary<StatDef, float> _statOffsetCache;
        public float GetStatOffsetCached(StatDef statDef)
        {
            if (_statOffsetCache == null) { RefreshStatOffsetCache(); }

            return _statOffsetCache.GetWithFallback(statDef, 0f);
        }

        static private StatDef[] _marketValueStats = new StatDef[] { StatDefOf.MarketValue, StatDefOf.MarketValueIgnoreHp };
        public void RefreshStatOffsetCache()
        {
            if (_statOffsetCache == null)
            {
                _statOffsetCache = new Dictionary<StatDef, float>();
            }
            else
            {
                _statOffsetCache.Clear();
            }

            if (_shellModule != null && _shellModule.moduleDef.worker is AutomataModuleWorker_Shell shellWorker)
            {
                foreach (var statMod in shellWorker.statOffsets)
                {
                    var offset = statMod.value;

                    var statPartStuff = statMod.stat.GetStatPart<StatPart_Stuff>();
                    if (statPartStuff != null)
                    {
                        var stuffStatMod = shellWorker.GetStatOffset(statPartStuff.multiplierStat);
                        if (stuffStatMod != null && _shellModule.stuffDef != null)
                        {
                            offset += stuffStatMod.value * _shellModule.stuffDef.GetStatValueAbstract(statPartStuff.stuffPowerStat);
                        }
                    }

                    if (_statOffsetCache.TryGetValue(statMod.stat, out var currentValue))
                    {
                        _statOffsetCache[statMod.stat] = currentValue + offset;
                    }
                    else
                    {
                        _statOffsetCache[statMod.stat] = offset;
                    }
                }
            }

            // Calc for marketvalue
            foreach (var marketValueStat in _marketValueStats)
            {
                var offset = 0f;
                foreach (var partDef in DefDatabase<AutomataModulePartDef>.AllDefsListForReading)
                {
                    var spec = ((Pawn)parent).TryGetModuleSpec(partDef);
                    if (spec != null)
                    {
                        switch (spec)
                        {
                            case AutomataModuleSpec_Core core:
                                {
                                    offset += marketValueStat.Worker.GetValue(StatRequest.For(core.moduleDef.mainIngredientDef, null, core.Quality));
                                }
                                break;

                            case AutomataModuleSpec_AnyOfThing anyOfThing:
                                {
                                    offset += marketValueStat.Worker.GetValue(StatRequest.For(anyOfThing.moduleDef.mainIngredientDef, anyOfThing.stuffDef, anyOfThing.Quality));
                                }
                                break;
                        }
                    }
                }


                if (_statOffsetCache.TryGetValue(marketValueStat, out var currentValue))
                {
                    _statOffsetCache[marketValueStat] = currentValue + offset;
                }
                else
                {
                    _statOffsetCache[marketValueStat] = offset;
                }
            }

        }

        public override float GetStatOffset(StatDef stat)
        {
            return GetStatOffsetCached(stat);
        }

        public override void PostExposeData()
        {
            Scribe_Deep.Look(ref _shellModule, "shellModule");
        }

        public override void Notify_DuplicatedFrom(Pawn source)
        {
            var sourceComp = source.GetComp<CompAutomata>();
            _shellModule = sourceComp._shellModule;
        }
    }
}
