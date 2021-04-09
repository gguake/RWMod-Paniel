using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe
{
    public class Bill_CustomizedProductionWithUft : Bill_ProductionWithUft, IBill_Customized
    {
        public RecipeDef OriginalRecipe => _impl.originalRecipe;
        public CustomizableBillParameter BillParameter => _impl.billParameter;

        private Bill_CustomizedProductionImpl _impl;

        /// <summary>
        /// do not use default constructor without parameters.
        /// </summary>
        public Bill_CustomizedProductionWithUft()
        {
        }

        public Bill_CustomizedProductionWithUft(RecipeDef originalRecipe)
            : base(CustomizableRecipeService.CloneRecipeDef(originalRecipe))
        {
            _impl = new Bill_CustomizedProductionImpl();
            _impl.originalRecipe = originalRecipe;
        }

        public void SetParameter(CustomizableBillParameter parameter)
        {
            if (_impl.billParameter != null)
            {
                Log.Message($"Tried to set bill parameter over twice.");
            }

            _impl.billParameter = parameter;
            _impl.billParameter.OnAttachBill(this);
        }

        public override void ExposeData()
        {
            Scribe_Deep.Look(ref _impl, "impl");

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                string recipeDefName = null;
                Scribe_Values.Look<string>(ref recipeDefName, "recipe");

                if (!DefDatabase<RecipeDef>.AllDefsListForReading.Any(x => x.defName == recipeDefName))
                {
                    CustomizableRecipeService.CloneRecipeDef(_impl.originalRecipe, recipeDefName);
                }
            }

            base.ExposeData();

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                _impl.billParameter.OnAttachBill(this);
            }
        }

        public override Bill Clone()
        {
            Bill_CustomizedProductionWithUft bill = (Bill_CustomizedProductionWithUft)base.Clone();

            bill._impl = new Bill_CustomizedProductionImpl();
            bill._impl.originalRecipe = _impl.originalRecipe;
            bill._impl.billParameter = _impl.billParameter.Clone();

            return bill;
        }
    }
}
