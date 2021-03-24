using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe
{
    public class Bill_CustomizedProduction : Bill_Production, IBill_Customized
    {
        public RecipeDef OriginalRecipe => _originalRecipe;
        public CustomizableBillParameter BillParameter => _billParameter;

        private RecipeDef _originalRecipe;
        private CustomizableBillParameter _billParameter;

        /// <summary>
        /// do not use default constructor without parameters.
        /// </summary>
        public Bill_CustomizedProduction()
        {
        }

        public Bill_CustomizedProduction(RecipeDef originalRecipe)
            : base(CustomizableRecipeService.CloneRecipeDef(originalRecipe))
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
            _billParameter.OnAttachBill(this);
        }

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref _originalRecipe, "originalRecipe");
            Scribe_Deep.Look(ref _billParameter, "billParameter");

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                string recipeDefName = null;
                Scribe_Values.Look<string>(ref recipeDefName, "recipe");

                if (!DefDatabase<RecipeDef>.AllDefsListForReading.Any(x => x.defName == recipeDefName))
                {
                    CustomizableRecipeService.CloneRecipeDef(_originalRecipe, recipeDefName);
                }
            }

            base.ExposeData();
            _billParameter.OnAttachBill(this);
        }
    }
}
