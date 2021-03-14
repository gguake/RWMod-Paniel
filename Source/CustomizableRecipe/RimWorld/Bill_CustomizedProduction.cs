using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe
{
    public class CustomizedBillParameter
    {
    }

    public class Bill_CustomizedProduction : Bill_Production
    {
        private RecipeDef _originalRecipe;

        public Bill_CustomizedProduction()
        {
        }

        public Bill_CustomizedProduction(RecipeDef originalRecipe)
            : base(originalRecipe.MakeClone())
        {
            _originalRecipe = originalRecipe;
        }

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref _originalRecipe, "originalRecipe");

            string recipeDefName = null;
            Scribe_Values.Look<string>(ref recipeDefName, "recipe");
            Log.Message($"{recipeDefName}");

            base.ExposeData();
        }

        public abstract
    }
}
