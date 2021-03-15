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
    public class CustomizeBillWindow_MakeAutomata : CustomizeBillWindow<CustomizableBillWorker_MakeAutomata>
    {
        public override Vector2 InitialSize => new Vector2(600f, 600f);

        List<ThingDef> _baseMaterialThings;


        public CustomizeBillWindow_MakeAutomata(CustomizableBillWorker_MakeAutomata billWorker, CustomizableRecipeDef recipe, BillStack billStack)
        {
            Initialize(billWorker, recipe, billStack);

            _baseMaterialThings = DefDatabase<ThingDef>.AllDefs.Where(x => x.IsMetal).ToList();
        }

        public override void DoWindowContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);


            Widgets.Dropdown(new Rect(0f, 50f, 40f, 30f),
                target: billWorker.baseMaterial,
                getPayload: (ThingDef thingDef) => thingDef,
                menuGenerator: Button_MaterialMenu,
                buttonLabel: billWorker.baseMaterial.LabelCap,
                buttonIcon: billWorker.baseMaterial.uiIcon,
                dragLabel: billWorker.baseMaterial.LabelCap,
                dragIcon: billWorker.baseMaterial.uiIcon,
                paintable: true);

            if (Widgets.ButtonText(new Rect(0, 450, 100, 40), "OK"))
            {
                Bill_CustomizedProductionWithUft bill = new Bill_CustomizedProductionWithUft(originalRecipeDef);
                CustomizableBillParameter_MakeAutomata parameter = new CustomizableBillParameter_MakeAutomata()
                {
                    workAmount = billWorker.WorkAmount,
                    ingredients = new Dictionary<ThingDef, int>()
                    {
                        { billWorker.baseMaterial, billWorker.baseMaterialCount },
                    },
                    craftingSkillLevel = billWorker.SkillLevelRequirement,
                };

                bill.SetParameter(parameter);
                ConfirmBill(bill);
            }

            if (Widgets.ButtonText(new Rect(120, 450, 100, 40), "Cancel"))
            {
                Close(true);
            }

            GUI.EndGroup();
        }

        private IEnumerable<Widgets.DropdownMenuElement<ThingDef>> Button_MaterialMenu(ThingDef td)
        {

            foreach (var thingDef in _baseMaterialThings)
            {
                yield return new Widgets.DropdownMenuElement<ThingDef>
                {
                    option = new FloatMenuOption(thingDef.LabelCap, delegate
                    {
                        billWorker.baseMaterial = thingDef;
                    }),
                    payload = thingDef
                };
            }
        }

    }
}
