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
        public AutomataSpecializationDef selectedSpecialization;

        public int baseMaterialCount;
        public int componentTotalCount;
        public float componentIndustrialScore;
        public float componentSpacerScore;
        public float useAIPersonaCoreScore;
        public int craftingSkillRequirementsMin;
        public int craftingSkillRequirementsMax;

        public ThingDef baseMaterial;
        public bool useAIPersonaCore;

        public int componentIndustrialCount;
        public int componentSpacerCount;

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

        public int ComponentIndustrialCount
        {
            get => componentIndustrialCount;
            set
            {
                if (value >= componentTotalCount)
                {
                    componentIndustrialCount = componentTotalCount;
                    componentSpacerCount = 0;
                }
                else
                {
                    componentIndustrialCount = value;
                    componentSpacerCount = componentTotalCount - value;
                }
            }
        }

        public int ComponentSpacerCount
        {
            get => componentSpacerCount;
            set
            {
                if (value >= componentTotalCount)
                {
                    componentSpacerCount = componentTotalCount;
                    componentIndustrialCount = 0;
                }
                else
                {
                    componentSpacerCount = value;
                    componentIndustrialCount = componentTotalCount - value;
                }
            }
        }

        public float MinScore => componentTotalCount * componentIndustrialScore;
        public float MaxScore => componentTotalCount * componentSpacerScore + useAIPersonaCoreScore;

        public int Score => 
            (int)(componentIndustrialScore * componentIndustrialCount + 
            componentSpacerScore * componentSpacerCount + 
            (useAIPersonaCore ? useAIPersonaCoreScore : 0));

        public int SkillLevelRequirement
        {
            get
            {
                float t = (Score - MinScore) / (MaxScore - MinScore);
                return Mathf.RoundToInt(Mathf.Lerp(craftingSkillRequirementsMin, craftingSkillRequirementsMax, t));
            }
        }

        public CustomizableBillWorker_MakeAutomata()
        {
        }

        public override void ResolveReferences()
        {
            foreach (var fixedIngredient in fixedIngredients)
            {
                fixedIngredient.ResolveReferences();
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
            useAIPersonaCore = other.useAIPersonaCore;

            componentIndustrialCount = other.componentIndustrialCount;
            componentSpacerCount = other.componentSpacerCount;

            fixedIngredients = other.fixedIngredients;
        }

        public override bool OnAddBill()
        {
            Find.WindowStack.Add(new CustomizeBillWindow_MakeAutomata(this, recipe, billStack));
            return false;
        }
    }
}
