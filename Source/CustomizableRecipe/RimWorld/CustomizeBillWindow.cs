using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe
{
    public abstract class CustomizeBillWindow<T> : Window
    {
        protected T billWorker;
        protected CustomizableRecipeDef originalRecipeDef;
        protected BillStack billStack;

        public CustomizeBillWindow()
        {
            forcePause = true;
            absorbInputAroundWindow = true;
            onlyOneOfTypeAllowed = true;
        }

        public virtual void Initialize(T billWorker, CustomizableRecipeDef recipeDef, BillStack billStack)
        {
            this.billWorker = billWorker;
            this.originalRecipeDef = recipeDef;
            this.billStack = billStack;
        }

        public void ConfirmBill(Bill bill)
        {
            billStack.AddBill(bill);
            Close(true);
        }
    }
}
