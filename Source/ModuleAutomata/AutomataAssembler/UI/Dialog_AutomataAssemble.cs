using UnityEngine;
using Verse;

namespace ModuleAutomata
{
    public class Dialog_AutomataAssemble : Window
    {
        public override Vector2 InitialSize => new Vector2(400, 400f);

        private AutomataAssembleBill _bill;

        public Dialog_AutomataAssemble(Pawn sourcePawn = null)
        {
            doCloseX = true;
            forcePause = true;
            absorbInputAroundWindow = true;

            if (sourcePawn != null )
            {
                _bill = new AutomataAssembleBill(sourcePawn);
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
        }
    }
}
