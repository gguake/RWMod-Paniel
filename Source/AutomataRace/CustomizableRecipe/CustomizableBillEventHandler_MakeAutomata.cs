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

            /*

            Bill_CustomizedProductionWithUft bill = new Bill_CustomizedProductionWithUft(this.recipe);
            CustomizableBillParameter_MakeAutomata parameter = new CustomizableBillParameter_MakeAutomata()
            {
                workAmount = Rand.Range(10, 10000),
                materialCount = Rand.Range(1, 100),
            };

            bill.SetParameter(parameter);
            billStack.AddBill(bill);

            */
            return false;
        }
    }
}
