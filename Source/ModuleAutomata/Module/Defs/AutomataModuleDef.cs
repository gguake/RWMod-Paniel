using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ModuleAutomata
{
    public class AutomataModuleDef : Def
    {
        public int uiOrder;

        public List<AutomataModulePartDef> adaptParts;

        public ThingDef mainIngredientDef;
        public bool affectedByQuality;
        public bool affectedByStuff;
        public bool isCore;

        public List<ThingDefCountClass> subIngredients;

        public int installWorkAmount;

        public AutomataModuleWorker worker;

        private Dictionary<(QualityCategory?, ThingDef), int> _tmpCandidateSet = new Dictionary<(QualityCategory?, ThingDef), int>();
        public IEnumerable<(AutomataModuleSpec spec, int count)> GetCandidateSpecsFromMap(Map map)
        {
            _tmpCandidateSet.Clear();

            if (isCore)
            {
                foreach (var thing in map.listerThings.ThingsOfDef(mainIngredientDef))
                {
                    yield return (new AutomataModuleSpec_Core()
                    {
                        moduleDef = this,
                        thing = thing,

                    }, thing.stackCount);
                }
            }
            else
            {
                if (affectedByQuality)
                {
                    if (affectedByStuff)
                    {
                        var things = map.listerThings.ThingsOfDef(mainIngredientDef);

                        foreach (var group in things.Where(v => v.HasComp<CompQuality>()).GroupBy(v => (v.TryGetComp<CompQuality>().Quality, v.Stuff)))
                        {
                            _tmpCandidateSet.Add(group.Key, group.Count());
                        }

                        foreach (var kv in _tmpCandidateSet)
                        {
                            yield return (new AutomataModuleSpec_AnyOfThing()
                            {
                                moduleDef = this,
                                quality = kv.Key.Item1,
                                stuffDef = kv.Key.Item2,
                            }, kv.Value);
                        }
                    }
                    else
                    {
                        var things = map.listerThings.ThingsOfDef(mainIngredientDef);

                        foreach (var group in things.Where(v => v.HasComp<CompQuality>()).GroupBy(v => (v.TryGetComp<CompQuality>().Quality, (ThingDef)null)))
                        {
                            _tmpCandidateSet.Add(group.Key, group.Count());
                        }

                        foreach (var kv in _tmpCandidateSet)
                        {
                            yield return (new AutomataModuleSpec_AnyOfThing()
                            {
                                moduleDef = this,
                                quality = kv.Key.Item1,
                            }, kv.Value);
                        }
                    }
                }
                else
                {
                    if (affectedByStuff)
                    {
                        var things = map.listerThings.ThingsOfDef(mainIngredientDef);

                        foreach (var group in things.Where(v => v.Stuff != null).GroupBy(v => ((QualityCategory?)null, v.Stuff)))
                        {
                            _tmpCandidateSet.Add(group.Key, group.Count());
                        }

                        foreach (var kv in _tmpCandidateSet)
                        {
                            yield return (new AutomataModuleSpec_AnyOfThing()
                            {
                                moduleDef = this,
                                stuffDef = kv.Key.Item2,

                            }, kv.Value);
                        }
                    }
                    else
                    {
                        var things = map.listerThings.ThingsOfDef(mainIngredientDef);

                        yield return (new AutomataModuleSpec_AnyOfThing()
                        {
                            moduleDef = this,
                        }, things.Sum(v => v.stackCount));
                    }
                }
            }
        }

        public override void PostLoad()
        {
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                if (affectedByQuality && !mainIngredientDef.HasComp<CompQuality>())
                {
                    throw new NotImplementedException("module uses quality but main ingredient has not CompQuality.");
                }

                if (affectedByStuff && !mainIngredientDef.MadeFromStuff)
                {
                    throw new NotImplementedException("module uses stuff but main ingredient has not stuffcategories.");
                }

                if (label == null) { label = mainIngredientDef.label; }
                if (description == null) { description = mainIngredientDef.description; }
            });
        }
    }
}
