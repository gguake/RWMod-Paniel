using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ModuleAutomata
{

    public abstract class AutomataModuleIngredientWorker
    {
        protected AutomataModuleDef _moduleDef;
        public bool HasQuality => _moduleDef.ingredientThingDef.HasComp<CompQuality>();
        public bool HasStuff => _moduleDef.ingredientThingDef.MadeFromStuff;

        public AutomataModuleIngredientWorker(AutomataModuleDef moduleDef)
        {
            _moduleDef = moduleDef;
        }

        public abstract IEnumerable<FloatMenuOption> GetCandidateFloatMenuOptions(AutomataModulePartDef partDef, Map map, Action<AutomataModuleSpec> callback);

    }

    public class AutomataModuleIngredientWorker_ThingDef : AutomataModuleIngredientWorker
    {
        public AutomataModuleIngredientWorker_ThingDef(AutomataModuleDef moduleDef) : base(moduleDef)
        {
        }

        public override IEnumerable<FloatMenuOption> GetCandidateFloatMenuOptions(AutomataModulePartDef partDef, Map map, Action<AutomataModuleSpec> callback)
        {
            if (HasQuality)
            {
                if (HasStuff)
                {
                    foreach (var group in map.listerThings
                        .ThingsOfDef(_moduleDef.ingredientThingDef)
                        .GroupBy(thing => (thing.TryGetQuality(out var quality) ? quality : QualityCategory.Normal, thing.Stuff))
                        .OrderBy(v => v.Key.Item1))
                    {
                        var bill = new AutomataModuleSpec()
                        {
                            modulePartDef = partDef,
                            moduleDef = _moduleDef,
                            thingDef = _moduleDef.ingredientThingDef,
                            quality = group.Key.Item1,
                            stuff = group.Key.Item2,
                        };

                        yield return new FloatMenuOption(bill.Label, () =>
                        {
                            callback(bill);
                        });
                    }
                }
                else
                {
                    foreach (var group in map.listerThings
                        .ThingsOfDef(_moduleDef.ingredientThingDef)
                        .GroupBy(thing => thing.TryGetQuality(out var quality) ? quality : QualityCategory.Normal)
                        .OrderBy(v => v.Key))
                    {
                        var bill = new AutomataModuleSpec()
                        {
                            modulePartDef = partDef,
                            moduleDef = _moduleDef,
                            thingDef = _moduleDef.ingredientThingDef,
                            quality = group.Key,
                        };

                        yield return new FloatMenuOption(bill.Label, () =>
                        {
                            callback(bill);
                        });
                    }
                }
            }
            else
            {
                if (HasStuff)
                {
                    foreach (var group in map.listerThings
                        .ThingsOfDef(_moduleDef.ingredientThingDef)
                        .GroupBy(thing => thing.Stuff))
                    {
                        var bill = new AutomataModuleSpec()
                        {
                            modulePartDef = partDef,
                            moduleDef = _moduleDef,
                            thingDef = _moduleDef.ingredientThingDef,
                            stuff = group.Key,
                        };

                        yield return new FloatMenuOption(bill.Label, () =>
                        {
                            callback(bill);
                        });
                    }
                }
                else
                {
                    if (map.listerThings.AnyThingWithDef(_moduleDef.ingredientThingDef))
                    {
                        var bill = new AutomataModuleSpec()
                        {
                            modulePartDef = partDef,
                            moduleDef = _moduleDef,
                            thingDef = _moduleDef.ingredientThingDef,
                        };

                        yield return new FloatMenuOption(bill.Label, () =>
                        {
                            callback(bill);
                        });
                    }
                }
            }
        }
    }

    public class AutomataModuleIngredientWorker_Thing : AutomataModuleIngredientWorker
    {
        public AutomataModuleIngredientWorker_Thing(AutomataModuleDef moduleDef) : base(moduleDef)
        {
        }

        public override IEnumerable<FloatMenuOption> GetCandidateFloatMenuOptions(AutomataModulePartDef partDef, Map map, Action<AutomataModuleSpec> callback)
        {
            foreach (var thing in map.listerThings
                .ThingsOfDef(_moduleDef.ingredientThingDef)
                .OrderBy(v => v.TryGetQuality(out var quality) ? quality : QualityCategory.Normal))
            {
                yield return new FloatMenuOption(thing.LabelCap, () =>
                {
                    callback(new AutomataModuleSpec()
                    {
                        modulePartDef = partDef,
                        moduleDef = _moduleDef,
                        thing = thing,
                    });
                });
            }
        }
    }
}
