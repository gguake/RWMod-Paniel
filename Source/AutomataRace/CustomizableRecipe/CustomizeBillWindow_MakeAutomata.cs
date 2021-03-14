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

        int _workCount = 10000;
        string _workCountBuffer;

        ThingDef _materialThing = ThingDefOf.Steel;
        int _materialCount = 100;
        string _materialCountBuffer;

        ThingDef _componentThing = ThingDefOf.ComponentIndustrial;
        int _componentCount = 20;
        string _componentCountBuffer;

        ThingDef[] _materialThings = new ThingDef[]
        {
            ThingDefOf.Steel,
            ThingDefOf.Uranium,
            ThingDefOf.Plasteel,
        };

        ThingDef[] _componentThings = new ThingDef[]
        {
            ThingDefOf.ComponentIndustrial,
            ThingDefOf.ComponentSpacer,
            ThingDefOf.AIPersonaCore,
        };


        public CustomizeBillWindow_MakeAutomata(CustomizableRecipeDef recipe, BillStack billStack)
        {
            Initialize(recipe, billStack);
        }

        public override void DoWindowContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);

            Widgets.TextFieldNumericLabeled(new Rect(0, 0, 200, 30f), "WorkCount", ref _workCount, ref _workCountBuffer);

            Widgets.Dropdown(new Rect(0f, 50f, 40f, 30f),
                target: _materialThing,
                getPayload: (ThingDef thingDef) => thingDef,
                menuGenerator: Button_MaterialMenu,
                buttonLabel: _materialThing.LabelCap,
                buttonIcon: _materialThing.uiIcon,
                dragLabel: _materialThing.LabelCap,
                dragIcon: _materialThing.uiIcon,
                paintable: true);

            Widgets.TextFieldNumeric(new Rect(0f, 85f, 40f, 30f), ref _materialCount, ref _materialCountBuffer);

            Widgets.Dropdown(new Rect(50f, 50f, 40f, 30f),
                target: _componentThing,
                getPayload: (ThingDef thingDef) => thingDef,
                menuGenerator: Button_ComponentMenu,
                buttonLabel: _componentThing.LabelCap,
                buttonIcon: _componentThing.uiIcon,
                dragLabel: _componentThing.LabelCap,
                dragIcon: _componentThing.uiIcon,
                paintable: true);
            
            Widgets.TextFieldNumeric(new Rect(50f, 85f, 40f, 30f), ref _componentCount, ref _componentCountBuffer);

            if (Widgets.ButtonText(new Rect(0, 450, 100, 40), "OK"))
            {
                Bill_CustomizedProductionWithUft bill = new Bill_CustomizedProductionWithUft(originalRecipeDef);
                CustomizableBillParameter_MakeAutomata parameter = new CustomizableBillParameter_MakeAutomata()
                {
                    workAmount = _workCount,
                    ingredients = new Dictionary<ThingDef, int>()
                    {
                        { _materialThing, _materialCount },
                        { _componentThing, _componentCount },
                    },
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
            foreach (var thingDef in _materialThings)
            {
                yield return new Widgets.DropdownMenuElement<ThingDef>
                {
                    option = new FloatMenuOption(thingDef.LabelCap, delegate
                    {
                        _materialThing = thingDef;
                    }),
                    payload = thingDef
                };
            }
        }

        private IEnumerable<Widgets.DropdownMenuElement<ThingDef>> Button_ComponentMenu(ThingDef td)
        {
            foreach (var thingDef in _componentThings)
            {
                yield return new Widgets.DropdownMenuElement<ThingDef>
                {
                    option = new FloatMenuOption(thingDef.LabelCap, delegate
                    {
                        _componentThing = thingDef;
                    }),
                    payload = thingDef
                };
            }
        }

    }
}
