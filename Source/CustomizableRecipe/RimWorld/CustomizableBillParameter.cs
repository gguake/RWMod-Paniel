using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe
{
    public abstract class CustomizableBillParameter : IExposable
    {
        public abstract void ExposeData();

        public abstract void OnAttachBill(Bill bill);
        public abstract void OnComplete(Bill bill, Thing product, Pawn worker);

        protected List<IngredientCount> MakeIngredientCountList(Dictionary<ThingDef, int> thingDefDict)
        {
            List<IngredientCount> ingredientCounts = new List<IngredientCount>();

            foreach (var kv in thingDefDict)
            {
                var thingDef = kv.Key;
                int count = kv.Value;

                IngredientCount ingredient = new IngredientCount();
                ingredient.filter = new ThingFilter();
                ingredient.filter.SetThingDefs(thingDef);
                ingredient.SetBaseCount((float)count);

                ingredient.ResolveReferences();
                ingredientCounts.Add(ingredient);
            }

            return ingredientCounts;
        }
    }
}
