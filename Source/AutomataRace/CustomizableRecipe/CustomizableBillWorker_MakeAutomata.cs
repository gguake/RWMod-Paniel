using CustomizableRecipe;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AutomataRace
{
    public class CustomizableBillWorker_MakeAutomata : CustomizableBillEventhandler
    {
        public override bool OnAddBill()
        {
            Log.Message($"{this.recipe.defName}");

            Bill bill = new Bill_CustomizedProduction(this.recipe);
            bill.recipe.workAmount = Rand.Range(10, 10000);

            billStack.AddBill(bill);
            return false;
        }
    }
}
