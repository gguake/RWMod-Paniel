using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe
{
    public class Bill_CustomizedProductionImpl : IExposable
    {
        public RecipeDef originalRecipe;
        public CustomizableBillParameter billParameter;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref originalRecipe, "originalRecipe");
            Scribe_Deep.Look(ref billParameter, "billParameter");
        }
    }
}
