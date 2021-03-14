using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe
{
    public static class RecipeDefExtensions
    {
        static FieldInfo _field_RecipeDef_ingredientValueGetterClass = AccessTools.Field(typeof(RecipeDef), "ingredientValueGetterClass");
        static FieldInfo _field_RecipeDef_uiIconThing = AccessTools.Field(typeof(RecipeDef), "uiIconThing");
        static FieldInfo _field_RecipeDef_isSurgeryCached = AccessTools.Field(typeof(bool?), "isSurgeryCached");

        public static RecipeDef MakeClone(this RecipeDef recipe, string defName = null)
        {
            int i = 0;
            RecipeDef ret = new RecipeDef();

            // Def
            ret.defName = defName ?? recipe.defName + $"_{DateTime.Now.Ticks}_{GenTicks.TicksGame}";
            ret.label = recipe.label;
            ret.description = recipe.description;
            ret.descriptionHyperlinks = recipe.descriptionHyperlinks.MakeListClone();
            ret.ignoreConfigErrors = recipe.ignoreConfigErrors;
            ret.modExtensions = recipe.modExtensions.MakeListClone();
            recipe.modContentPack?.AddDef(ret, "ImpliedDefs");

            // RecipeDef
            ret.workerClass = recipe.workerClass;
            ret.workerCounterClass = recipe.workerCounterClass;
            ret.jobString = recipe.jobString;
            ret.requiredGiverWorkType = recipe.requiredGiverWorkType;
            ret.workAmount = recipe.workAmount;
            ret.workSpeedStat = recipe.workSpeedStat;
            ret.efficiencyStat = recipe.efficiencyStat;
            ret.workTableEfficiencyStat = recipe.workTableEfficiencyStat;
            ret.workTableSpeedStat = recipe.workTableSpeedStat;
            ret.ingredients = recipe.ingredients.MakeListClone();
            ret.fixedIngredientFilter = recipe.fixedIngredientFilter;
            ret.defaultIngredientFilter = recipe.defaultIngredientFilter;
            ret.allowMixingIngredients = recipe.allowMixingIngredients;
            ret.ignoreIngredientCountTakeEntireStacks = recipe.ignoreIngredientCountTakeEntireStacks;
            _field_RecipeDef_ingredientValueGetterClass.SetValue(ret, _field_RecipeDef_ingredientValueGetterClass.GetValue(recipe));
            ret.forceHiddenSpecialFilters = recipe.forceHiddenSpecialFilters.MakeListClone();
            ret.autoStripCorpses = recipe.autoStripCorpses;
            ret.interruptIfIngredientIsRotting = recipe.interruptIfIngredientIsRotting;
            ret.products = recipe.products.MakeListClone();
            ret.specialProducts = recipe.specialProducts.MakeListClone();
            ret.productHasIngredientStuff = recipe.productHasIngredientStuff;
            ret.useIngredientsForColor = recipe.useIngredientsForColor;
            ret.targetCountAdjustment = recipe.targetCountAdjustment;
            ret.unfinishedThingDef = recipe.unfinishedThingDef;
            ret.skillRequirements = recipe.skillRequirements.MakeListClone();
            ret.workSkill = recipe.workSkill;
            ret.workSkillLearnFactor = recipe.workSkillLearnFactor;
            ret.effectWorking = recipe.effectWorking;
            ret.soundWorking = recipe.soundWorking;
            _field_RecipeDef_uiIconThing.SetValue(ret, _field_RecipeDef_uiIconThing.GetValue(recipe));
            ret.recipeUsers = recipe.recipeUsers.MakeListClone();
            ret.appliedOnFixedBodyParts = recipe.appliedOnFixedBodyParts.MakeListClone();
            ret.appliedOnFixedBodyPartGroups = recipe.appliedOnFixedBodyPartGroups;
            ret.addsHediff = recipe.addsHediff;
            ret.removesHediff = recipe.removesHediff;
            ret.changesHediffLevel = recipe.changesHediffLevel;
            ret.incompatibleWithHediffTags = recipe.incompatibleWithHediffTags.MakeListClone();
            ret.hediffLevelOffset = recipe.hediffLevelOffset;
            ret.hideBodyPartNames = recipe.hideBodyPartNames;
            ret.isViolation = recipe.isViolation;
            ret.successfullyRemovedHediffMessage = recipe.successfullyRemovedHediffMessage;
            ret.surgerySuccessChanceFactor = recipe.surgerySuccessChanceFactor;
            ret.deathOnFailedSurgeryChance = recipe.deathOnFailedSurgeryChance;
            ret.targetsBodyPart = recipe.targetsBodyPart;
            ret.anesthetize = recipe.anesthetize;
            ret.researchPrerequisite = recipe.researchPrerequisite;
            ret.researchPrerequisites = recipe.researchPrerequisites.MakeListClone();
            ret.factionPrerequisiteTags = recipe.factionPrerequisiteTags.MakeListClone();
            ret.conceptLearned = recipe.conceptLearned;
            ret.dontShowIfAnyIngredientMissing = recipe.dontShowIfAnyIngredientMissing;
            // _field_RecipeDef_isSurgeryCached.SetValue(ret, _field_RecipeDef_isSurgeryCached.GetValue(recipe));

            DefDatabase<RecipeDef>.Add(ret);
            return ret;
        }
    }
}
