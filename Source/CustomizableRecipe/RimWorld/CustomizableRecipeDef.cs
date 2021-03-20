using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe
{
    /// <summary>
    /// customizable recipe def.
    /// </summary>
    public class CustomizableRecipeDef : RecipeDef
    {
        public CustomizableBillWorker billWorker;

        public override void ResolveReferences()
        {
            base.ResolveReferences();

            billWorker.ResolveReferences();
        }
    }
}
