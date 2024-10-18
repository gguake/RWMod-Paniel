using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public AutomataModuleIngredientInfo(ThingDef thingDef, QualityCategory? quality, ThingDef stuff)
        {
            this.thingDef = thingDef;
            this.quality = quality;
            this.stuffDef = stuff;
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

        public int WorkAmount => _plans
            .Where(kv => kv.Value.plan == AutomataModuleModificationPlanType.Replace && kv.Value.spec != null)
            .Sum(kv => kv.Value.spec.moduleDef.installWorkAmount);

        public IEnumerable<(AutomataModuleIngredientInfo, int count)> IngredientsWithAnyCondition
        {
            get
            {
                if (_ingredientsDictCache == null) { RefreshIngredientCache(); }
                return _ingredientsDictCache.Where(kv => kv.Key.HasAnyCondition).Select(kv => (kv.Key, kv.Value));
            }
        }

        public IEnumerable<(AutomataModuleIngredientInfo, int count)> IngredientsWithNoCondition
        {
            get
            {
                if (_ingredientsDictCache == null) { RefreshIngredientCache(); }
                return _ingredientsDictCache.Where(kv => !kv.Key.HasAnyCondition).Select(kv => (kv.Key, kv.Value));
            }
        }

        private Dictionary<AutomataModuleIngredientInfo, int> _ingredientsDictCache = null;
        private void RefreshIngredientCache()
        {
            if (_ingredientsDictCache == null)
            {
                _ingredientsDictCache = new Dictionary<AutomataModuleIngredientInfo, int>();
            }
            else
            {
                _ingredientsDictCache.Clear();
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

                _ingredientsDictCache[mainIngredientInfo] = _ingredientsDictCache.GetWithFallback(mainIngredientInfo, 0) + 1;

                if (kv.Value.spec.moduleDef.subIngredients?.Count > 0)
                {
                    foreach (var tdc in kv.Value.spec.moduleDef.subIngredients)
                    {
                        var info = new AutomataModuleIngredientInfo(tdc.thingDef, null, null);
                        _ingredientsDictCache[info] = _ingredientsDictCache.GetWithFallback(info, 0) + tdc.count;
                    }
                }
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref _plans, "plans", LookMode.Def, LookMode.Deep);
            Scribe_Values.Look(ref hairAddonIndex, "hairAddonIndex");
            Scribe_Deep.Look(ref headType, "headType");
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
