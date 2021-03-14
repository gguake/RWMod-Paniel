using CustomizableRecipe;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AutomataRace
{
    public class CustomizeBillWindow_MakeAutomata : CustomizeBillWindow
    {
        public override Vector2 InitialSize => new Vector2(600f, 600f);

        public CustomizeBillWindow_MakeAutomata(CustomizableRecipeDef recipe, BillStack billStack)
        {
            Initialize(recipe, billStack);
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (Widgets.ButtonText(new Rect(0, 0, 100, 100), "test"))
            {
                Close(true);
            }
        }
    }
}
