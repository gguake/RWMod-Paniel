using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe
{
    public static class CustomizableRecipeService
    {
        private static List<RecipeDef> recipes = new List<RecipeDef>();

        public static RecipeDef CloneRecipeDef(RecipeDef recipe, string reservedDefName = null)
        {
            var recipeClone = recipe.MakeClone(reservedDefName);
            DefDatabase<RecipeDef>.Add(recipeClone);

            recipes.Add(recipeClone);
            return recipeClone;
        }

        private static MethodInfo method_DefDatabase_defsList = AccessTools.Method(typeof(DefDatabase<RecipeDef>), "Remove");
        public static void Reset()
        {
            if (recipes.Count > 0)
            {
                Log.Message($"Cleanup {recipes.Count} customized recipes in previous game.");

                foreach (var recipe in recipes)
                {
                    method_DefDatabase_defsList.Invoke(null, new object[] { recipe });
                }

                recipes.Clear();
            }
        }
    }
}
