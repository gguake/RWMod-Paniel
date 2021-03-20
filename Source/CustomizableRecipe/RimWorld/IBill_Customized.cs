using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe
{
    public interface IBill_Customized
    {
        RecipeDef OriginalRecipe { get; }
        CustomizableBillParameter BillParameter { get; }

        void SetParameter(CustomizableBillParameter parameter);
    }
}
