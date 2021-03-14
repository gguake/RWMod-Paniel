
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe
{
    /// <summary>
    /// this is intermediate class for make bill with customized recipe.
    /// so this is not actually real bill.
    /// </summary>
    public class Bill_Customizer : Bill_Production
    {
        public CustomizableRecipeDef CustomizableRecipe => (CustomizableRecipeDef)recipe;

        public CustomizableBillEventhandler EventHandler { get; private set; }

        public Bill_Customizer(RecipeDef recipe) : 
            base(recipe)
        {
            EventHandler = Activator.CreateInstance(CustomizableRecipe._billEventHandlerClass) as CustomizableBillEventhandler;
            EventHandler.recipe = CustomizableRecipe;
        }
    }
}
