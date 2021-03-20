using AutomataRace.Logic;
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
        public int componentScore;
        public AutomataSpecializationDef specialization;
        public Dictionary<ThingDef, int> ingredients;
        public int craftingSkillLevel;

        private PseudoRandom _randomGenerator = new PseudoRandom();

        public override void ExposeData()
        {
            Scribe_Values.Look(ref workAmount, "workAmount");
            Scribe_Values.Look(ref componentScore, "componentScore");
            Scribe_Defs.Look(ref specialization, "specialization");
            Scribe_Collections.Look(ref ingredients, "ingredients");
            Scribe_Values.Look(ref craftingSkillLevel, "craftingSkillLevel");

            Scribe_Deep.Look(ref _randomGenerator, "randomGenerator");
        }

        public override void OnAttachBill(Bill bill)
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

        public override void OnComplete(Thing product, Pawn worker)
        {
            int workerSkill = worker?.skills?.GetSkill(SkillDefOf.Crafting)?.Level ?? 0;
            bool workerInspired = worker?.InspirationDef == InspirationDefOf.Inspired_Creativity;
            if (workerInspired)
            {
                worker.mindState.inspirationHandler.EndInspiration(InspirationDefOf.Inspired_Creativity);
            }

            int finalScore = componentScore + (workerInspired ? 100 : 0);

            var weights = AutomataQualityService.GetProductProbabilityWeights(finalScore);
            int weightSum = weights.Sum(x => x.Value);
            int randomValue = (_randomGenerator.Next % weightSum);

            QualityCategory quality = QualityCategory.Awful;
            int weightStack = 0;
            foreach (var kv in weights)
            {
                if (randomValue < weightStack + kv.Value)
                {
                    quality = kv.Key;
                    break;
                }

                weightStack += kv.Value;
            }

            var thing = product as ThingWithComps;
            if (thing != null)
            {
                CompQuality compQuality = new CompQuality();
                compQuality.parent = thing;
                compQuality.SetQuality(quality, ArtGenerationContext.Colony);

                thing.AllComps.Add(compQuality);
            }
        }
    }

}
