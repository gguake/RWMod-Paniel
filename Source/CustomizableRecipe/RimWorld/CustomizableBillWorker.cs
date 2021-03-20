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
    /// abstract class for handling event of customizable bill.
    /// </summary>
    public abstract class CustomizableBillWorker
    {
        [Unsaved(false)]
        public CustomizableRecipeDef recipe;

        [Unsaved(false)]
        public BillStack billStack;

        public abstract void ResolveReferences();

        public abstract void CopyFrom(CustomizableBillWorker worker);

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
