using CustomizableRecipe;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AutomataRace
{
    public class CustomizableBillEventHandler_MakeAutomata : CustomizableBillEventhandler
    {
        public override bool OnAddBill()
        {
            Find.WindowStack.Add(new CustomizeBillWindow_MakeAutomata(recipe, billStack));
            return false;
        }
    }
}
