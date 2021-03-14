using CustomizableRecipe;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AutomataRace
{
    public class CustomizableBillParameter_MakeAutomata : CustomizableBillParameter
    {
        public float workAmount;
        public int materialCount;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref workAmount, "workAmount");
            Scribe_Values.Look(ref materialCount, "materialCount");
        }

        public override void Apply(Bill bill)
        {
            bill.recipe.workAmount = workAmount;

            var newIngredients = new List<IngredientCount>();
            var originalIngredients = bill.recipe.ingredients;
            foreach (var ingredient in originalIngredients)
            {
                var ingredientCount = new IngredientCount();
                ingredientCount.filter = ingredient.filter.MakeClone();
                ingredientCount.SetBaseCount(materialCount);

                newIngredients.Add(ingredientCount);
            }

            bill.recipe.ingredients = newIngredients;
        }
    }

}
