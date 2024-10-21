using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Verse;

namespace ModuleAutomata
{
    public enum AutomataModuleModificationPlanType
    {
        Preserve,
        Replace,
    }

    public struct AutomataModuleModificationPlan : IExposable
    {
        public AutomataModuleSpec spec;
        public AutomataModuleModificationPlanType plan;

        public void ExposeData()
        {
            Scribe_Deep.Look(ref spec, "spec");
            Scribe_Values.Look(ref plan, "plan");
        }
    }

    public struct AutomataModuleIngredientInfo
    {
        public ThingDef thingDef;
        public QualityCategory? quality;
        public ThingDef stuffDef;

        public bool HasAnyCondition => quality != null || stuffDef != null;

        public string Label => PNLocale.MakeModuleLabel(thingDef, quality, stuffDef);

        public AutomataModuleIngredientInfo(ThingDef thingDef, QualityCategory? quality, ThingDef stuff)
        {
            this.thingDef = thingDef;
            this.quality = quality;
            this.stuffDef = stuff;
        }

        public bool Match(Thing thing)
        {
            if (thing.def != thingDef) { return false; }
            if (quality != null && quality.Value != thing.TryGetComp<CompQuality>().Quality) { return false; }
            if (stuffDef != null && stuffDef != thing.Stuff) { return false; }

            return true;
        }
    }

    public class AutomataModificationPlan : IExposable
    {
        public AutomataModuleModificationPlan this[AutomataModulePartDef part]
        {
            get
            {
                if (_plans.TryGetValue(part, out var plan))
                {
                    return plan;
                }

                return default;
            }
            set
            {
                _plans[part] = value;
                RefreshIngredientCache();
            }
        }
        private Dictionary<AutomataModulePartDef, AutomataModuleModificationPlan> _plans = new Dictionary<AutomataModulePartDef, AutomataModuleModificationPlan>();
        public int hairAddonIndex;
        public HeadTypeDef headType;

        public int TotalWorkAmount => _plans
            .Where(kv => kv.Value.plan == AutomataModuleModificationPlanType.Replace && kv.Value.spec != null)
            .Sum(kv => kv.Value.spec.moduleDef.installWorkAmount);

        public int GetIngredientCount(AutomataModuleIngredientInfo info)
        {
            if (_totalIngredientsDictCache == null) { RefreshIngredientCache(); }
            return _totalIngredientsDictCache.TryGetValue(info, out var count) ? count : 0;
        }

        private Dictionary<AutomataModuleIngredientInfo, int> _totalIngredientsDictCache = null;
        public IEnumerable<(AutomataModuleIngredientInfo info, int count)> TotalIngredients
        {
            get
            {
                if (_totalIngredientsDictCache == null) { RefreshIngredientCache(); }
                return _totalIngredientsDictCache.Select(kv => (kv.Key, kv.Value));
            }
        }

        private Dictionary<AutomataModuleIngredientInfo, int> _mainIngredientsDictCache = null;
        public IEnumerable<(AutomataModuleIngredientInfo info, int count)> MainIngredients
        {
            get
            {
                if (_mainIngredientsDictCache == null) { RefreshIngredientCache(); }
                return _mainIngredientsDictCache.Select(kv => (kv.Key, kv.Value));
            }
        }

        private Dictionary<AutomataModuleIngredientInfo, int> _subIngredientsDictCache = null;
        public IEnumerable<(AutomataModuleIngredientInfo info, int count)> SubIngredients
        {
            get
            {
                if (_subIngredientsDictCache == null) { RefreshIngredientCache(); }
                return _subIngredientsDictCache.Select(kv => (kv.Key, kv.Value));
            }
        }

        private void RefreshIngredientCache()
        {
            if (_totalIngredientsDictCache == null)
            {
                _totalIngredientsDictCache = new Dictionary<AutomataModuleIngredientInfo, int>();
            }
            else
            {
                _totalIngredientsDictCache.Clear();
            }

            if (_mainIngredientsDictCache == null)
            {
                _mainIngredientsDictCache = new Dictionary<AutomataModuleIngredientInfo, int>();
            }
            else
            {
                _mainIngredientsDictCache.Clear();
            }

            if (_subIngredientsDictCache == null)
            {
                _subIngredientsDictCache = new Dictionary<AutomataModuleIngredientInfo, int>();
            }
            else
            {
                _subIngredientsDictCache.Clear();
            }

            foreach (var kv in _plans.Where(kv => kv.Value.plan == AutomataModuleModificationPlanType.Replace && kv.Value.spec != null))
            {
                AutomataModuleIngredientInfo mainIngredientInfo;
                if (kv.Value.spec.moduleDef.affectedByQuality)
                {
                    if (kv.Value.spec.moduleDef.affectedByStuff)
                    {
                        mainIngredientInfo = new AutomataModuleIngredientInfo(kv.Value.spec.moduleDef.mainIngredientDef, kv.Value.spec.Quality, kv.Value.spec.Stuff);
                    }
                    else
                    {
                        mainIngredientInfo = new AutomataModuleIngredientInfo(kv.Value.spec.moduleDef.mainIngredientDef, kv.Value.spec.Quality, null);
                    }
                }
                else
                {
                    if (kv.Value.spec.moduleDef.affectedByStuff)
                    {
                        mainIngredientInfo = new AutomataModuleIngredientInfo(kv.Value.spec.moduleDef.mainIngredientDef, null, kv.Value.spec.Stuff);
                    }
                    else
                    {
                        mainIngredientInfo = new AutomataModuleIngredientInfo(kv.Value.spec.moduleDef.mainIngredientDef, null, null);
                    }
                }

                _totalIngredientsDictCache[mainIngredientInfo] = _totalIngredientsDictCache.GetWithFallback(mainIngredientInfo, 0) + 1;
                _mainIngredientsDictCache[mainIngredientInfo] = _mainIngredientsDictCache.GetWithFallback(mainIngredientInfo, 0) + 1;

                if (kv.Value.spec.moduleDef.subIngredients?.Count > 0)
                {
                    foreach (var tdc in kv.Value.spec.moduleDef.subIngredients)
                    {
                        var info = new AutomataModuleIngredientInfo(tdc.thingDef, null, null);
                        _totalIngredientsDictCache[info] = _totalIngredientsDictCache.GetWithFallback(info, 0) + tdc.count;
                        _subIngredientsDictCache[info] = _subIngredientsDictCache.GetWithFallback(info, 0) + tdc.count;
                    }
                }
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref _plans, "plans", LookMode.Def, LookMode.Deep);
            Scribe_Values.Look(ref hairAddonIndex, "hairAddonIndex");
            Scribe_Defs.Look(ref headType, "headType");
        }

        public void ApplyPawn(Pawn pawn)
        {
            foreach (var plan in _plans)
            {
                var part = plan.Key;
                var planType = plan.Value.plan;

                var currentSpec = pawn.TryGetModuleSpec(part);
                var spec = plan.Value.spec;

                if (planType == AutomataModuleModificationPlanType.Replace)
                {
                    if (currentSpec != null)
                    {
                        currentSpec.moduleDef.worker.OnUninstallFromPawn(pawn, part, currentSpec);
                    }

                    if (spec != null)
                    {
                        spec.moduleDef.worker.OnInstallToPawn(pawn, part, spec);
                    }
                }
            }

            pawn.SetBodyAddonIndex(0, hairAddonIndex);
            pawn.story.headType = headType;
        }
    }
}
