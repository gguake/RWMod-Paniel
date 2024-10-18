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

        public IEnumerable<AutomataModuleSpec> GetCandidateSpecsFromMap(Map map)
        {
            if (isCore)
            {
                foreach (var thing in map.listerThings.ThingsOfDef(mainIngredientDef))
                {
                    yield return new AutomataModuleSpec_Core()
                    {
                        moduleDef = this,
                        thing = thing,
                    };
                }
            }
            else
            {
                if (affectedByQuality)
                {
                    if (affectedByStuff)
                    {
                        var things = map.listerThings.ThingsOfDef(mainIngredientDef);
                        foreach (var tuple in things.Select(v => (v.TryGetComp<CompQuality>().Quality, v.Stuff)).Distinct())
                        {
                            yield return new AutomataModuleSpec_AnyOfThing()
                            {
                                moduleDef = this,
                                quality = tuple.Quality,
                                stuffDef = tuple.Stuff,
                            };
                        }
                    }
                    else
                    {
                        var things = map.listerThings.ThingsOfDef(mainIngredientDef);
                        foreach (var quality in things.Select(v => v.TryGetComp<CompQuality>().Quality).Distinct())
                        {
                            yield return new AutomataModuleSpec_AnyOfThing()
                            {
                                moduleDef = this,
                                quality = quality
                            };
                        }
                    }
                }
                else
                {
                    if (affectedByStuff)
                    {
                        var things = map.listerThings.ThingsOfDef(mainIngredientDef);
                        foreach (var stuff in things.Select(v => v.Stuff).Distinct())
                        {
                            yield return new AutomataModuleSpec_AnyOfThing()
                            {
                                moduleDef = this,
                                stuffDef = stuff
                            };
                        }
                    }
                    else
                    {
                        if (map.listerThings.AnyThingWithDef(mainIngredientDef))
                        {
                            yield return new AutomataModuleSpec_AnyOfThing()
                            {
                                moduleDef = this,
                            };
                        }
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
