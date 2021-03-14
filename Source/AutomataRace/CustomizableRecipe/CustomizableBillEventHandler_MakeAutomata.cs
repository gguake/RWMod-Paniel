using CustomizableRecipe;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AutomataRace
{
    public class CustomizableBillParameter_MakeAutomata : CustomizableBillParameter
    {
        public float workAmount;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref workAmount, "workAmount");
        }

        public override void Apply(Bill bill)
        {
            bill.recipe.workAmount = workAmount;
        }

    }

    public class CustomizableBillEventHandler_MakeAutomata : CustomizableBillEventhandler
    {
        public override bool OnAddBill()
        {
            Bill_CustomizedProductionWithUft bill = new Bill_CustomizedProductionWithUft(this.recipe);
            CustomizableBillParameter_MakeAutomata parameter = new CustomizableBillParameter_MakeAutomata()
            {
                workAmount = Rand.Range(10, 10000),
            };

            bill.SetParameter(parameter);
            billStack.AddBill(bill);
            return false;
        }
    }
}
