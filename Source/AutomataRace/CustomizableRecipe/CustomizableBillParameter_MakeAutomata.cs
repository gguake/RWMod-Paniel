using AutomataRace.Logic;
using CustomizableRecipe;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AutomataRace
{
    public class CustomizableBillParameter_MakeAutomata : CustomizableBillParameter
    {
        public List<AutomataAppearanceParameter> appearanceChoices = new List<AutomataAppearanceParameter>();
        public AutomataSpecializationDef specialization;
        public Dictionary<ThingDef, int> ingredients;

        private PseudoRandom _randomGenerator = new PseudoRandom();

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref appearanceChoices, "appearanceChoices");
            Scribe_Defs.Look(ref specialization, "specialization");
            Scribe_Collections.Look(ref ingredients, "ingredients");

            Scribe_Deep.Look(ref _randomGenerator, "randomGenerator");
        }

        public override void OnAttachBill(Bill bill)
        {
            var customizedBill = bill as Bill_CustomizedProductionWithUft;
            var customizableRecipe = customizedBill?.OriginalRecipe as CustomizableRecipeDef;
            var billWorker = customizableRecipe?.billWorker as CustomizableBillWorker_MakeAutomata;

            if (customizedBill == null || customizableRecipe == null || billWorker == null)
            {
                Log.Error("Tried to call OnAttachBill with non-supported bill.");
                return;
            }

            bill.recipe.workAmount = AutomataBillService.CalcWorkAmount(customizableRecipe, billWorker.baseMaterial);
            bill.recipe.ingredients = MakeIngredientCountList(ingredients);

            if (!billWorker.fixedIngredients.NullOrEmpty())
            {
                bill.recipe.ingredients.AddRange(billWorker.fixedIngredients);
            }

            int score = AutomataBillService.CalcComponentScore(customizableRecipe, ingredients);
            int craftingSkillRequire = AutomataBillService.CalcCraftingSkillRequirement(customizableRecipe, score);

            bill.recipe.skillRequirements = new List<SkillRequirement>();
            if (craftingSkillRequire > 0)
            {
                bill.recipe.skillRequirements.Add(new SkillRequirement()
                {
                    skill = SkillDefOf.Crafting,
                    minLevel = craftingSkillRequire
                });
            }
        }

        public override void OnComplete(Bill bill, Thing product, Pawn worker)
        {
            var customizedBill = bill as Bill_CustomizedProductionWithUft;
            var customizableRecipe = customizedBill?.OriginalRecipe as CustomizableRecipeDef;

            if (customizedBill == null || customizableRecipe == null)
            {
                Log.Error("Tried to call OnAttachBill with non-supported bill.");
                return;
            }

            int workerSkill = worker?.skills?.GetSkill(SkillDefOf.Crafting)?.Level ?? 0;
            bool workerInspired = worker?.InspirationDef == InspirationDefOf.Inspired_Creativity;
            if (workerInspired)
            {
                worker.mindState.inspirationHandler.EndInspiration(InspirationDefOf.Inspired_Creativity);
            }

            int score = AutomataBillService.CalcComponentScore(customizableRecipe, ingredients);
            int finalScore = Mathf.FloorToInt(score * (workerInspired ? 1.2f : 1f));

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
