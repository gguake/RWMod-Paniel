using CustomizableRecipe;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;
using AutomataRace.Logic;

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

        public int Score => AutomataBillService.CalcComponentScore(recipe, componentIndustrialCount, componentSpacerCount, useAIPersonaCore);

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

            selectedSpecialization = other.selectedSpecialization;

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
