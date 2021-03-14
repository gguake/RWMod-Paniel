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
        public Dictionary<ThingDef, int> ingredients;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref workAmount, "workAmount");
            Scribe_Collections.Look(ref ingredients, "ingredients");
        }

        public override void Apply(Bill bill)
        {
            bill.recipe.workAmount = workAmount;

            var newIngredients = new List<IngredientCount>();
            bill.recipe.ingredients = MakeIngredientCountList(ingredients);
        }
    }

}
