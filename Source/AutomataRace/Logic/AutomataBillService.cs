using CustomizableRecipe;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AutomataRace.Logic
{
    public static class AutomataBillService
    {
        public static float CalcWorkAmount(RecipeDef recipe, ThingDef baseMaterial)
        {
            float baseMaterialWorkToMake = baseMaterial.stuffProps?.statFactors?.FirstOrDefault(x => x.stat == StatDefOf.WorkToMake)?.value ?? 1f;
            float workAmount = recipe.workAmount * baseMaterialWorkToMake;
            return workAmount;
        }

        public static int CalcComponentScore(RecipeDef recipe, Dictionary<ThingDef, int> ingredients)
        {
            var industrial = ingredients.TryGetValue(ThingDefOf.ComponentIndustrial, 0);
            var spacer = ingredients.TryGetValue(ThingDefOf.ComponentSpacer, 0);
            var personaCore = ingredients.TryGetValue(ThingDefOf.AIPersonaCore, 0);

            return CalcComponentScore(recipe, industrial, spacer, personaCore > 0);
        }

        public static int CalcComponentScore(RecipeDef recipe, int componentIndustrialCount, int componentSpacerCount, bool useAIPersonaCore)
        {
            var customizableRecipe = recipe as CustomizableRecipeDef;
            var billWorker = customizableRecipe?.billWorker as CustomizableBillWorker_MakeAutomata;

            return (int)(billWorker.componentIndustrialScore * componentIndustrialCount + 
                billWorker.componentSpacerScore * componentSpacerCount + 
                (useAIPersonaCore ? billWorker.useAIPersonaCoreScore : 0));
        }

        public static int CalcCraftingSkillRequirement(RecipeDef recipe, int score)
        {
            var customizableRecipe = recipe as CustomizableRecipeDef;
            var billWorker = customizableRecipe?.billWorker as CustomizableBillWorker_MakeAutomata;

            float fScore = score;
            float minScore = CalcComponentScore(recipe, 20, 0, false);
            float maxScore = CalcComponentScore(recipe, 0, 20, true);

            float t = (fScore - minScore) / (maxScore - minScore);
            return Mathf.RoundToInt(Mathf.Lerp(billWorker.craftingSkillRequirementsMin, billWorker.craftingSkillRequirementsMax, t));
        }
    }
}
