using AutomataRace.Logic;
using CustomizableRecipe;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AutomataRace
{
    public class CustomizeBillWindow_MakeAutomata : CustomizeBillWindow<CustomizableBillWorker_MakeAutomata>
    {
        static class UIConstants
        {
            [TweakValue("AutomataUI", min = 0f, max = 100f)] public static float groupMargin = 10f;
            [TweakValue("AutomataUI", min = 0f, max = 300f)] public static float baseMaterialY = 38f;
            [TweakValue("AutomataUI", min = 0f, max = 300f)] public static float componentIndustrialY = 88f;
            [TweakValue("AutomataUI", min = 0f, max = 300f)] public static float componentSpacerY = 130f;
            [TweakValue("AutomataUI", min = 0f, max = 300f)] public static float aiPersonaCoreY = 175f;

            public static Rect rtAppearanceTab = new Rect(0f, 0f, 270f, 380f);
            public static Rect rtSpecializationTab = new Rect(280f, 0f, 292f, 140f);
            public static Rect rtMaterialTab = new Rect(280f, 150f, 292f, 230f);
            public static Rect rtExpectedTab = new Rect(0f, 390f, 572f, 125f);
            public static Rect rtButtonTab = new Rect(0f, 550f, 572f, 40f);

            public static readonly Vector2 pawnPortraitSize = new Vector2(184f, 256f);
        }

        Pawn _tempPawn;
        RenderTexture _tempPawnCachedTexture;
        List<ThingDef> _baseMaterialThings = DefDatabase<ThingDef>.AllDefs.Where(x => x.IsMetal).ToList();

        public override Vector2 InitialSize => new Vector2(
            Margin * 2 + UIConstants.rtSpecializationTab.x + UIConstants.rtSpecializationTab.width,
            Margin * 2 + UIConstants.rtButtonTab.y + UIConstants.rtButtonTab.height);

        public CustomizeBillWindow_MakeAutomata(CustomizableBillWorker_MakeAutomata billWorker, CustomizableRecipeDef recipe, BillStack billStack)
        {
            Initialize(billWorker, recipe, billStack);
            RecreateSamplePawn();
        }

        public override void ConfirmBill(Bill bill)
        {
            if (!billStack.billGiver.Map.mapPawns.FreeColonists.Any((Pawn col) => bill.recipe.PawnSatisfiesSkillRequirements(col)))
            {
                Bill.CreateNoPawnsWithSkillDialog(bill.recipe);
            }

            string x = null;

            string y = x ?? "fdf";

            base.ConfirmBill(bill);
        }

        public override void DoWindowContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);
            
            #region Appearance Tab
            Widgets.DrawMenuSection(UIConstants.rtAppearanceTab);
            GUI.BeginGroup(UIConstants.rtAppearanceTab.ContractedBy(UIConstants.groupMargin));
            {
                Text.Font = GameFont.Medium;
                Widgets.Label(new Rect(0f, 0f, 200f, 40f), "Appearance");
                Text.Font = GameFont.Small;

                Rect rect = new Rect(10f, 130f, 230f, 220f);
                GUI.DrawTexture(new Rect(rect.center.x - UIConstants.pawnPortraitSize.x / 2f, rect.yMin - 24f, UIConstants.pawnPortraitSize.x, UIConstants.pawnPortraitSize.y), _tempPawnCachedTexture);

                if (Widgets.ButtonText(new Rect(150f, 324f, 100f, 36f), "Randomize".Translate()))
                {
                    SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                    RecreateSamplePawn();
                }
            }
            GUI.EndGroup();
            #endregion

            #region Specialization Tab
            Widgets.DrawMenuSection(UIConstants.rtSpecializationTab);
            GUI.BeginGroup(UIConstants.rtSpecializationTab.ContractedBy(UIConstants.groupMargin));
            {
                Text.Font = GameFont.Medium;
                Widgets.Label(new Rect(0f, 0f, 200f, 40f), "Specialization");
                Text.Font = GameFont.Small;

                // Combat
                if (Widgets.ButtonImageFitted(new Rect(15f, 45f, 64f, 64f), AutomataRaceDefOf.PN_Specialization_Combat.UIIcon))
                {
                    SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                    billWorker.selectedSpecialization = AutomataRaceDefOf.PN_Specialization_Combat;
                }

                if (billWorker.selectedSpecialization == AutomataRaceDefOf.PN_Specialization_Combat)
                    GUI.DrawTexture(new Rect(31f, 81f, 32f, 32f), Widgets.CheckboxOnTex);

                // Engineer
                if (Widgets.ButtonImageFitted(new Rect(104f, 45f, 64f, 64f), AutomataRaceDefOf.PN_Specialization_Engineer.UIIcon))
                {
                    SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                    billWorker.selectedSpecialization = AutomataRaceDefOf.PN_Specialization_Engineer;
                }

                if (billWorker.selectedSpecialization == AutomataRaceDefOf.PN_Specialization_Engineer)
                    GUI.DrawTexture(new Rect(120f, 81f, 32f, 32f), Widgets.CheckboxOnTex);

                // Domestic
                if (Widgets.ButtonImageFitted(new Rect(193f, 45f, 64f, 64f), AutomataRaceDefOf.PN_Specialization_Domestic.UIIcon))
                {
                    SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                    billWorker.selectedSpecialization = AutomataRaceDefOf.PN_Specialization_Domestic;
                }

                if (billWorker.selectedSpecialization == AutomataRaceDefOf.PN_Specialization_Domestic)
                    GUI.DrawTexture(new Rect(209f, 81f, 32f, 32f), Widgets.CheckboxOnTex);
            }
            GUI.EndGroup();
            #endregion

            #region Material Tab
            Widgets.DrawMenuSection(UIConstants.rtMaterialTab);
            GUI.BeginGroup(UIConstants.rtMaterialTab.ContractedBy(UIConstants.groupMargin));
            {
                Text.Font = GameFont.Medium;
                Widgets.Label(new Rect(0f, 0f, 200f, 40f), "Choose Materials");
                Text.Font = GameFont.Small;

                GUI.DrawTexture(new Rect(5f, UIConstants.baseMaterialY, 32f, 32f), billWorker.baseMaterial.uiIcon);
                Widgets.Label(new Rect(47f, UIConstants.baseMaterialY + 5f, 50f, 30f), $"x{billWorker.baseMaterialCount * (billWorker.baseMaterial.smallVolume ? 10 : 1)}");

                if (Widgets.ButtonText(new Rect(130f, UIConstants.baseMaterialY - 3f, 125f, 35f), billWorker.baseMaterial.LabelCap))
                {
                    Find.WindowStack.Add(new FloatMenu(_baseMaterialThings.Select(x => new FloatMenuOption(x.LabelCap, () =>
                    {
                        billWorker.baseMaterial = x;
                    })).ToList()));
                }

                TooltipHandler.TipRegion(new Rect(0f, UIConstants.componentIndustrialY, UIConstants.rtMaterialTab.width, 40f), ThingDefOf.ComponentIndustrial.LabelCap);
                GUI.DrawTexture(new Rect(5f, UIConstants.componentIndustrialY, 32f, 32f), ThingDefOf.ComponentIndustrial.uiIcon);
                Widgets.Label(new Rect(47f, UIConstants.componentIndustrialY + 3f, 50f, 30f), $"x{billWorker.ComponentIndustrialCount}");
                billWorker.ComponentIndustrialCount = (int)Widgets.HorizontalSlider(new Rect(110f, UIConstants.componentIndustrialY, 150f, 38f), billWorker.ComponentIndustrialCount, 0f, 20f, middleAlignment: true, leftAlignedLabel: "0", rightAlignedLabel: "20", roundTo: 0);


                TooltipHandler.TipRegion(new Rect(0f, UIConstants.componentSpacerY, UIConstants.rtMaterialTab.width, 40f), ThingDefOf.ComponentSpacer.LabelCap);
                GUI.DrawTexture(new Rect(5f, UIConstants.componentSpacerY, 32f, 32f), ThingDefOf.ComponentSpacer.uiIcon);
                Widgets.Label(new Rect(47f, UIConstants.componentSpacerY + 3f, 50f, 30f), $"x{billWorker.ComponentSpacerCount}");
                billWorker.ComponentSpacerCount = (int)Widgets.HorizontalSlider(new Rect(110f, UIConstants.componentSpacerY, 150f, 38f), billWorker.ComponentSpacerCount, 0f, 20f, middleAlignment: true, leftAlignedLabel: "0", rightAlignedLabel: "20", roundTo: 0);

                TooltipHandler.TipRegion(new Rect(0f, UIConstants.aiPersonaCoreY, UIConstants.rtMaterialTab.width, 40f), ThingDefOf.AIPersonaCore.LabelCap);
                GUI.DrawTexture(new Rect(5f, UIConstants.aiPersonaCoreY, 32f, 32f), ThingDefOf.AIPersonaCore.uiIcon);
                Widgets.Label(new Rect(47f, UIConstants.aiPersonaCoreY + 3f, 50f, 30f), $"x{(billWorker.useAIPersonaCore ? 1 : 0)}");
                Widgets.Checkbox(new Vector2(170f, UIConstants.aiPersonaCoreY + 2.5f), ref billWorker.useAIPersonaCore);
            }
            GUI.EndGroup();
            #endregion

            #region Expected Tab
            GUI.BeginGroup(UIConstants.rtExpectedTab);
            {
                Text.Font = GameFont.Medium;
                Widgets.Label(new Rect(10f, 10f, 200f, 40f), "Expected");
                Text.Font = GameFont.Small;

                //Widgets.DrawLineHorizontal(0f, 45f, 572f);

                Rect innerRect = new Rect(5f, 45f, UIConstants.rtExpectedTab.width - 10f, UIConstants.rtExpectedTab.height - 45f);
                GUI.BeginGroup(innerRect);
                {
                    float width = innerRect.width / 7f;
                    Widgets.DrawLineHorizontal(0f, 30f, innerRect.width);

                    Dictionary<QualityCategory, float> probability = AutomataQualityService.GetProductProbability(billWorker);

                    for (byte i = 0; i < 7; ++i)
                    {
                        DrawQualityProbabilitySection(probability, (QualityCategory)i, new Rect(i * width, 0f, width, innerRect.height));
                    }
                }
                GUI.EndGroup();
            }
            GUI.EndGroup();
            #endregion

            #region Buttons Tab
            GUI.BeginGroup(UIConstants.rtButtonTab);

            if (Widgets.ButtonText(new Rect(150f, 0f, 110f, 40f), "Cancel"))
            {
                Close(true);
            }

            Color defaultColor = GUI.color;
            GUI.color = CheckAllRequirements() ? defaultColor : Color.grey;
            if (Widgets.ButtonText(new Rect(312f, 0f, 110f, 40f), "Add Bill"))
            {
                TryAddBill();
            }
            GUI.color = defaultColor;

            GUI.EndGroup();
            #endregion

            GUI.EndGroup();
        }

        private bool CheckAllRequirements()
        {
            if (billWorker.selectedSpecialization == null)
            {
                return false;
            }

            return true;
        }

        private void TryAddBill()
        {
            if (!CheckAllRequirements())
            {
                return;
            }

            Bill_CustomizedProductionWithUft bill = new Bill_CustomizedProductionWithUft(originalRecipeDef);

            var ingredients = new Dictionary<ThingDef, int>();
            ingredients.Add(billWorker.baseMaterial, billWorker.baseMaterialCount);
            if (billWorker.ComponentIndustrialCount > 0)
            {
                ingredients.Add(ThingDefOf.ComponentIndustrial, billWorker.ComponentIndustrialCount);
            }

            if (billWorker.ComponentSpacerCount > 0)
            {
                ingredients.Add(ThingDefOf.ComponentSpacer, billWorker.ComponentSpacerCount);
            }

            if (billWorker.useAIPersonaCore)
            {
                ingredients.Add(ThingDefOf.AIPersonaCore, 1);
            }

            CustomizableBillParameter_MakeAutomata parameter = new CustomizableBillParameter_MakeAutomata()
            {
                workAmount = billWorker.WorkAmount,
                specialization = billWorker.selectedSpecialization,
                ingredients = ingredients,
                craftingSkillLevel = billWorker.SkillLevelRequirement,
            };

            bill.SetParameter(parameter);

            _tempPawn.Destroy();
            ConfirmBill(bill);
        }

        private void RecreateSamplePawn()
        {
            if (_tempPawn != null)
            {
                _tempPawn.Destroy();
                Find.WorldPawns.RemovePawn(_tempPawn);
                Find.WorldPawns.PassToWorld(_tempPawn, RimWorld.Planet.PawnDiscardDecideMode.Discard);
                _tempPawn = null;
            }

            _tempPawn = PawnGenerator.GeneratePawn(AutomataRaceDefOf.Paniel_Randombox_Normal, faction: null);
            _tempPawnCachedTexture = PortraitsCache.Get(_tempPawn, UIConstants.pawnPortraitSize);
        }

        private void DrawQualityProbabilitySection(Dictionary<QualityCategory, float> probability, QualityCategory quality, Rect rect)
        {
            byte qualityInt = (byte)quality;
            GUI.BeginGroup(rect);
            if (qualityInt % 2 == 1)
            {
                Widgets.DrawAltRect(rect.AtZero());
            }

            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(new Rect(0f, 0f, rect.width, 30f), quality.GetLabel());

            string probText = "-";
            if (probability.ContainsKey(quality))
            {
                probText = probability[quality].ToString("P");
            }
            Widgets.Label(new Rect(0f, 30f, rect.width, rect.height - 30f), probText);

            Text.Anchor = TextAnchor.UpperLeft;
            Text.Font = GameFont.Small;

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
