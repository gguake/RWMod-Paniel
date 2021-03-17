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
        public override Vector2 InitialSize => new Vector2(607f, 630f);

        List<ThingDef> _baseMaterialThings;


        public CustomizeBillWindow_MakeAutomata(CustomizableBillWorker_MakeAutomata billWorker, CustomizableRecipeDef recipe, BillStack billStack)
        {
            Initialize(billWorker, recipe, billStack);

            _baseMaterialThings = DefDatabase<ThingDef>.AllDefs.Where(x => x.IsMetal).ToList();
        }

        public override void ConfirmBill(Bill bill)
        {
            if (!billStack.billGiver.Map.mapPawns.FreeColonists.Any((Pawn col) => bill.recipe.PawnSatisfiesSkillRequirements(col)))
            {
                Bill.CreateNoPawnsWithSkillDialog(bill.recipe);
            }

            base.ConfirmBill(bill);
        }

        public override void DoWindowContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);

            // Appearance Tab
            Rect rtAppearanceTab = new Rect(0f, 0f, 270f, 380f);
            GUI.BeginGroup(rtAppearanceTab);
            GUI.DrawTexture(rtAppearanceTab.AtZero(), TexUI.TitleBGTex);

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(10f, 10f, 200f, 40f), "Appearance");
            Text.Font = GameFont.Small;

            Widgets.DrawBox(new Rect(15f, 50f, 240f, 30f));
            Widgets.DrawBox(new Rect(15f, 90f, 240f, 30f));
            Widgets.DrawBox(new Rect(20f, 140f, 230f, 220f));

            GUI.EndGroup();

            // Specialization
            Rect rtSpecializationTab = new Rect(280f, 0f, 292f, 140f);
            GUI.BeginGroup(rtSpecializationTab);
            GUI.DrawTexture(rtSpecializationTab.AtZero(), TexUI.TitleBGTex);

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(10f, 10f, 200f, 40f), "Specialization");
            Text.Font = GameFont.Small;

            Widgets.DrawBox(new Rect(25f, 55f, 64f, 64f));
            Widgets.DrawBox(new Rect(114f, 55f, 64f, 64f));
            Widgets.DrawBox(new Rect(203f, 55f, 64f, 64f));

            GUI.EndGroup();

            // Material
            Rect rtMaterialTab = new Rect(280f, 150f, 292f, 230f);
            GUI.BeginGroup(rtMaterialTab);
            GUI.DrawTexture(rtMaterialTab.AtZero(), TexUI.TitleBGTex);

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(10f, 10f, 200f, 40f), "Choose Materials");
            Text.Font = GameFont.Small;


            //Widgets.DrawBox(new Rect(15f, 50f, 262f, 30f));
            GUI.DrawTexture(new Rect(20f, 50f, 32f, 32f), billWorker.baseMaterial.uiIcon);
            Widgets.Label(new Rect(62f, 55f, 50f, 30f), $"x{billWorker.baseMaterialCount * (billWorker.baseMaterial.smallVolume ? 10 : 1)}");

            if (Widgets.ButtonText(new Rect(145f, 47.5f, 125f, 35f), billWorker.baseMaterial.LabelCap))
            {
                Find.WindowStack.Add(new FloatMenu(_baseMaterialThings.Select(x => new FloatMenuOption(x.LabelCap, () =>
                {
                    billWorker.baseMaterial = x;
                })).ToList()));
            }   

            Widgets.DrawBox(new Rect(15f, 105f, 262f, 30f));
            Widgets.DrawBox(new Rect(15f, 145f, 262f, 30f));
            Widgets.DrawBox(new Rect(15f, 185f, 262f, 30f));

            GUI.EndGroup();


            // Expected
            Rect rtExpectedTab = new Rect(0f, 390f, 572f, 125f);
            GUI.BeginGroup(rtExpectedTab);
            //GUI.DrawTexture(rtExpectedTab.AtZero(), TexUI.TitleBGTex);

            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(10f, 10f, 200f, 40f), "Expected");
            Text.Font = GameFont.Small;

            Widgets.DrawLineHorizontal(0f, 45f, 572f);

            GUI.EndGroup();



            // Buttons
            Rect rtButtonTab = new Rect(0f, 550f, 572f, 80f);
            GUI.BeginGroup(rtButtonTab);
            //GUI.DrawTexture(rtExpectedTab.AtZero(), TexUI.TitleBGTex);

            if (Widgets.ButtonText(new Rect(150f, 0f, 110f, 40f), "Cancel"))
            {
                Close(true);
            }
            
            if (Widgets.ButtonText(new Rect(312f, 0f, 110f, 40f), "Add Bill"))
            {
                Bill_CustomizedProductionWithUft bill = new Bill_CustomizedProductionWithUft(originalRecipeDef);

                var ingredients = new Dictionary<ThingDef, int>();
                ingredients.Add(billWorker.baseMaterial, billWorker.baseMaterialCount);
                if (billWorker.componentIndustrialCount > 0)
                {
                    ingredients.Add(ThingDefOf.ComponentIndustrial, billWorker.componentIndustrialCount);
                }

                if (billWorker.componentSpacerCount > 0)
                {
                    ingredients.Add(ThingDefOf.ComponentSpacer, billWorker.componentSpacerCount);
                }

                if (billWorker.useAIPersonaCore)
                {
                    ingredients.Add(ThingDefOf.AIPersonaCore, 1);
                }

                CustomizableBillParameter_MakeAutomata parameter = new CustomizableBillParameter_MakeAutomata()
                {
                    workAmount = billWorker.WorkAmount,
                    ingredients = ingredients,
                    craftingSkillLevel = billWorker.SkillLevelRequirement,
                };

                bill.SetParameter(parameter);
                ConfirmBill(bill);
            }

            GUI.EndGroup();
            /*
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
            }

            if (Widgets.ButtonText(new Rect(120, 450, 100, 40), "Cancel"))
            {
                Close(true);
            }
            */

            GUI.EndGroup();
        }

        private IEnumerable<Widgets.DropdownMenuElement<ThingDef>> Button_MaterialMenu()
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
