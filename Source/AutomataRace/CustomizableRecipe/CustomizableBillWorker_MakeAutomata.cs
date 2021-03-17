using CustomizableRecipe;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace AutomataRace
{
    public class CustomizableBillWorker_MakeAutomata : CustomizableBillWorker
    {
        public int baseMaterialCount;
        public int componentTotalCount;
        public float componentIndustrialScore;
        public float componentSpacerScore;
        public float useAIPersonaCoreScore;
        public int craftingSkillRequirementsMin;
        public int craftingSkillRequirementsMax;

        public ThingDef baseMaterial;

        public int componentIndustrialCount;
        public int componentSpacerCount;
        public bool useAIPersonaCore;

        public List<IngredientCount> fixedIngredients;

        public float WorkAmount
        {
            get
            {
                float baseMaterialWorkToMake = baseMaterial.stuffProps?.statFactors?.FirstOrDefault(x => x.stat == StatDefOf.WorkToMake)?.value ?? 1f;
                float workAmount = recipe.workAmount * baseMaterialWorkToMake;
                return workAmount;
            }
        }

        public float MinScore => componentTotalCount * componentIndustrialScore;
        public float MaxScore => componentTotalCount * componentSpacerScore + useAIPersonaCoreScore;

        public float Score => 
            componentIndustrialScore * componentIndustrialCount + 
            componentSpacerScore * componentSpacerCount + 
            (useAIPersonaCore ? useAIPersonaCoreScore : 0f);

        public int SkillLevelRequirement
        {
            get
            {
                float t = (Score - MinScore) / (MaxScore - MinScore);
                Log.Message($"{t}");
                return Mathf.RoundToInt(Mathf.Lerp(craftingSkillRequirementsMin, craftingSkillRequirementsMax, t));
            }
        }

        public CustomizableBillWorker_MakeAutomata()
        {
        }

        public void SetComponentIndustrialCount(int count)
        {
            if (count >= componentTotalCount)
            {
                componentIndustrialCount = componentTotalCount;
                componentSpacerCount = 0;
            }
            else
            {
                componentIndustrialCount = count;
                componentSpacerCount = componentTotalCount - count;
            }
        }

        public void SetComponentSpacerCount(int count)
        {
            if (count >= componentTotalCount)
            {
                componentSpacerCount = componentTotalCount;
                componentIndustrialCount = 0;
            }
            else
            {
                componentSpacerCount = count;
                componentIndustrialCount = componentTotalCount - count;
            }
        }

        public override void CopyFrom(CustomizableBillWorker worker)
        {
            var other = worker as CustomizableBillWorker_MakeAutomata;
            if (other == null)
            {
                return;
            }

            baseMaterialCount = other.baseMaterialCount;
            componentTotalCount = other.componentTotalCount;
            componentIndustrialScore = other.componentIndustrialScore;
            componentSpacerScore = other.componentSpacerScore;
            useAIPersonaCoreScore = other.useAIPersonaCoreScore;
            craftingSkillRequirementsMin = other.craftingSkillRequirementsMin;
            craftingSkillRequirementsMax = other.craftingSkillRequirementsMax;

            baseMaterial = other.baseMaterial;
            baseMaterialCount = other.baseMaterialCount;
            componentIndustrialCount = other.componentIndustrialCount;
            componentSpacerCount = other.componentSpacerCount;
            useAIPersonaCore = other.useAIPersonaCore;

            fixedIngredients = other.fixedIngredients;
        }

        public override bool OnAddBill()
        {
            Find.WindowStack.Add(new CustomizeBillWindow_MakeAutomata(this, recipe, billStack));
            return false;
        }

    }
}
