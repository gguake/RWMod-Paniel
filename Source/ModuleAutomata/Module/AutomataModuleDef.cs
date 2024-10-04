using System;
using System.Collections.Generic;
using Verse;

namespace ModuleAutomata
{
    public class AutomataModuleDef : Def
    {
        public List<AutomataModulePartDef> adaptParts;

        public ThingDef ingredientThingDef;
        public Type ingredientWorkerClass;

        private AutomataModuleIngredientWorker _ingredientWorker;
        public AutomataModuleIngredientWorker IngredientWorker
        {
            get
            {
                if (_ingredientWorker == null)
                {
                    _ingredientWorker = (AutomataModuleIngredientWorker)Activator.CreateInstance(ingredientWorkerClass, this);
                }

                return _ingredientWorker;
            }
        }

        public List<AutomataModuleProperty> properties;

        public override void PostLoad()
        {
            base.PostLoad();
            LongEventHandler.ExecuteWhenFinished(() =>
            {
                if (label == null && ingredientThingDef != null)
                {
                    label = ingredientThingDef.label;
                }

                if (description == null && ingredientThingDef != null)
                {
                    description = ingredientThingDef.description;
                }

                cachedLabelCap = null;
            });
        }
    }
}
