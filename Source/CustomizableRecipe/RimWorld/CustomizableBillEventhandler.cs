using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableRecipe
{
    /// <summary>
    /// abstract class for handling event of customizable bill.
    /// </summary>
    public abstract class CustomizableBillEventhandler
    {
        public CustomizableRecipeDef recipe;
        public BillStack billStack;

        /// <summary>
        /// Called before bill is added to bill stack.
        /// </summary>
        /// <returns>whether bill should added to bill stack.</returns>
        public abstract bool OnAddBill();

        protected void ConfirmBill(Bill bill)
        {
            billStack.AddBill(bill);
        }
    }
}
