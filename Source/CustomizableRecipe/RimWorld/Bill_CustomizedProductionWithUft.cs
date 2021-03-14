using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe
{
    public class Bill_CustomizedProductionWithUft : Bill_ProductionWithUft
    {
        private RecipeDef _originalRecipe;
        private CustomizableBillParameter _billParameter;

        /// <summary>
        /// do not use default constructor without parameters.
        /// </summary>
        public Bill_CustomizedProductionWithUft()
        {
        }

        public Bill_CustomizedProductionWithUft(RecipeDef originalRecipe)
            : base(originalRecipe.MakeClone())
        {
            _originalRecipe = originalRecipe;
        }

        public void SetParameter(CustomizableBillParameter parameter)
        {
            if (_billParameter != null)
            {
                Log.Message($"Tried to set bill parameter over twice.");
            }

            _billParameter = parameter;
            _billParameter.Apply(this);
        }

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref _originalRecipe, "originalRecipe");
            Scribe_Deep.Look(ref _billParameter, "billParameter");

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                string defName = null;
                Scribe_Values.Look<string>(ref defName, "recipe");

                if (!DefDatabase<RecipeDef>.AllDefsListForReading.Any(x => x.defName == defName))
                {
                    var recipeClone = _originalRecipe.MakeClone(defName);
                }
            }

            base.ExposeData();
            _billParameter.Apply(this);
        }
    }
}
