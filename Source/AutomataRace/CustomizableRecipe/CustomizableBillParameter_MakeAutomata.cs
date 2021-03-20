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
        public AutomataSpecializationDef specialization;
        public Dictionary<ThingDef, int> ingredients;
        public int craftingSkillLevel;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref workAmount, "workAmount");
            Scribe_Defs.Look(ref specialization, "specialization");
            Scribe_Collections.Look(ref ingredients, "ingredients");
            Scribe_Values.Look(ref craftingSkillLevel, "craftingSkillLevel");
        }

        public override void Apply(Bill bill)
        {
            var customizedBill = bill as Bill_CustomizedProductionWithUft;
            var customizableRecipe = customizedBill?.OriginalRecipe as CustomizableRecipeDef;
            var billWorker = customizableRecipe?.billWorker as CustomizableBillWorker_MakeAutomata;

            bill.recipe.workAmount = workAmount;
            bill.recipe.ingredients = MakeIngredientCountList(ingredients);

            if (!billWorker.fixedIngredients.NullOrEmpty())
            {
                bill.recipe.ingredients.AddRange(billWorker.fixedIngredients);
            }

            bill.recipe.skillRequirements = new List<SkillRequirement>();
            if (craftingSkillLevel > 0)
            {
                bill.recipe.skillRequirements.Add(new SkillRequirement()
                {
                    skill = SkillDefOf.Crafting,
                    minLevel = craftingSkillLevel
                });
            }
        }
    }

}
