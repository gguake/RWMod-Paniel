﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace ModuleAutomata
{
    public enum AutomataAssembleSummaryTab
    {
        PawnSkill,
        PawnCapacity,
    }

    [StaticConstructorOnStartup]
    public class Dialog_AutomataAssemble : Window
    {
        public struct ModuleElementUIInfo
        {
            public string selectButtonLabel;
            public Action onSelectButtonClicked;

            public string nameLabel;
            public bool descButtonVisible;
            public Action onDescButtonClicked;
        }
        private ModuleElementUIInfo[] _systemModuleElementInfos = new ModuleElementUIInfo[4];
        private ModuleElementUIInfo[] _armModuleElementInfos = new ModuleElementUIInfo[2];
        private ModuleElementUIInfo[] _legModuleElementInfos = new ModuleElementUIInfo[2];

        public static readonly Texture2D OpenStatsReportTex = ContentFinder<Texture2D>.Get("UI/Buttons/OpenStatsReport");
        public static readonly Texture2D PNTitleIconTex = ContentFinder<Texture2D>.Get("Icon/Paniel_HQ");
        public static readonly Texture2D ArrowLeftTex = ContentFinder<Texture2D>.Get("UI/Widgets/PN_ArrowLeft");
        public static readonly Texture2D ArrowRightTex = ContentFinder<Texture2D>.Get("UI/Widgets/PN_ArrowRight");

        public override Vector2 InitialSize => new Vector2(1000f, 520f);

        private AutomataAssembleSummaryTab _currentTab = AutomataAssembleSummaryTab.PawnSkill;
        private Pawn _samplePawn;

        private Building_AutomataAssembler _building;
        private Action<AutomataInfo> _callback;
        private Pawn _targetPawn;

        public AutomataInfo Bill { get; private set; } = new AutomataInfo();

        public Dialog_AutomataAssemble(Building_AutomataAssembler building, Pawn pawn, Action<AutomataInfo> callback)
        {
            _building = building;
            _callback = callback;
            _targetPawn = pawn;

            doCloseX = true;
            forcePause = true;
            absorbInputAroundWindow = true;

            InitializeBillFromPawn(pawn);
            RefreshModuleUI();
        }

        public Dialog_AutomataAssemble(Building_AutomataAssembler building, Action<AutomataInfo> callback)
        {
            _building = building;
            _callback = callback;

            doCloseX = true;
            forcePause = true;
            absorbInputAroundWindow = true;

            InitializeBillFromDefaultSetting();
            RefreshModuleUI();
        }

        private List<SkillDef> _tmpSkillDefsInListOrder = new List<SkillDef>();
        public override void DoWindowContents(Rect inRect)
        {
            var rtMain = new RectDivider(inRect.ContractedBy(4f), 113779456);

            var rtLeftSection = rtMain.NewCol(240f);
            DrawTabSection(rtLeftSection);

            var rtTitleSection = rtMain.NewRow(80f);
            DrawTitleSection(rtTitleSection);

            var rtSummarySection = rtMain.NewRow(120f, VerticalJustification.Bottom);
            DrawSummarySection(rtSummarySection);

            var rtSystemModuleSection = rtMain.NewCol(260f);
            {
                var centerRect = rtSystemModuleSection.Rect.ContractedBy(0f, 4f);
                DrawModuleSection(centerRect, _systemModuleElementInfos);
            }

            var rtArmLegModuleSection = rtMain.NewCol(260f, HorizontalJustification.Right);
            {
                var rtArmModuleSection = rtArmLegModuleSection.NewRow(rtArmLegModuleSection.Rect.height / 2f, marginOverride: 0f);
                {
                    var centerRect = rtArmModuleSection.Rect.ContractedBy(4f);
                    DrawModuleSection(centerRect, _armModuleElementInfos);

                }

                var rtLegModuleSection = rtArmLegModuleSection.Rect;
                {
                    var centerRect = rtLegModuleSection.ContractedBy(4f);
                    DrawModuleSection(centerRect, _legModuleElementInfos);
                }
            }

            var rtPortraitSection = rtMain;
            DrawPortraitSection(rtPortraitSection);
        }

        public override void Close(bool doCloseSound = true)
        {
            _samplePawn?.Destroy();

            base.Close(doCloseSound);
        }

        private void InitializeBillFromDefaultSetting()
        {
            foreach (var setting in _building.AutomataAssembleUIExtension.defaultModuleSetting)
            {
                Bill[setting.modulePartDef] = new AutomataModuleSpec_ThingDef()
                {
                    modulePartDef = setting.modulePartDef,
                    moduleDef = setting.moduleDef,
                    thingDef = setting.moduleDef.ingredientThingDef,
                };
            }
        }

        private void InitializeBillFromPawn(Pawn pawn)
        {

        }

        private void DrawTabSection(Rect rect)
        {
            var rtBase = new RectDivider(rect, 68345793);
            _ = rtBase.NewRow(24f);

            GUI.color = Color.white;
            Widgets.DrawMenuSection(rtBase.Rect);

            var tabs = new List<TabRecord>()
                {
                    new TabRecord(PNLocale.PN_DialogTabPawnSkillLabel.Translate(), () =>
                    {
                        _currentTab = AutomataAssembleSummaryTab.PawnSkill;
                    }, _currentTab == AutomataAssembleSummaryTab.PawnSkill),

                    new TabRecord(PNLocale.PN_DialogTabPawnCapacityLabel.Translate(), () =>
                    {
                        _currentTab = AutomataAssembleSummaryTab.PawnCapacity;
                    }, _currentTab == AutomataAssembleSummaryTab.PawnCapacity)
                };

            TabDrawer.DrawTabs(rtBase, tabs);

            try
            {
                var rtLeftTabBody = rtBase.Rect.ContractedBy(9f);
                Widgets.BeginGroup(rtLeftTabBody);

                var rtLeftTabGroup = new Rect(0, 0, rtLeftTabBody.width, rtLeftTabBody.height);
                switch (_currentTab)
                {
                    case AutomataAssembleSummaryTab.PawnSkill:
                        #region PawnSkill
                        {
                            var coreModule = Bill[PNAutomataModulePartDefOf.PN_Core] as AutomataModuleSpec_Core;
                            if (coreModule?.thing == null)
                            {
                                using (new TextBlock(TextAnchor.MiddleCenter))
                                {
                                    Widgets.Label(rtLeftTabGroup, PNLocale.PN_DialogTabNoSelectedCoreLabel.Translate());
                                }
                            }
                            else
                            {
                                var comp = coreModule.thing.TryGetComp<CompAutomataCore>();
                                if (comp == null) { throw new NotImplementedException(); }

                                var rtSkillSection = new RectDivider(rtLeftTabGroup, 14797513);

                                var rtTitle = rtSkillSection.NewRow(64f);
                                {
                                    var rtIcon = rtTitle.NewCol(64f, marginOverride: 0f);
                                    Widgets.DrawTextureFitted(rtIcon, comp.Props.SpecializationIcon, 1f);

                                    using (new TextBlock(TextAnchor.MiddleLeft))
                                    {
                                        Widgets.Label(rtTitle.Rect, coreModule.thing.LabelCap);
                                    }
                                }

                                var rtDivider = rtSkillSection.NewRow(2f);
                                {
                                    Widgets.DrawLineHorizontal(rtDivider.Rect.xMin, rtDivider.Rect.yMin, rtDivider.Rect.width, new Color(0.3f, 0.3f, 0.3f, 1f));
                                }

                                _tmpSkillDefsInListOrder.Clear();
                                _tmpSkillDefsInListOrder.AddRange(DefDatabase<SkillDef>.AllDefsListForReading.OrderByDescending(sd => sd.listOrder));
                                var columnWidth = _tmpSkillDefsInListOrder.Max(def => Text.CalcSize(def.skillLabel.CapitalizeFirst()).x);

                                foreach (var skillDef in _tmpSkillDefsInListOrder)
                                {
                                    var skillLevel = comp.GetSkillLevel(skillDef);
                                    var rtRow = rtSkillSection.NewRow(27f, marginOverride: 2f);
                                    if (Mouse.IsOver(rtRow))
                                    {
                                        GUI.DrawTexture(rtRow, TexUI.HighlightTex);
                                    }

                                    using (new TextBlock(TextAnchor.MiddleLeft))
                                    {
                                        if (comp.IsDisabledSkill(skillDef))
                                        {
                                            GUI.color = new Color(1f, 1f, 1f, 0.5f);
                                            Widgets.Label(rtRow.NewCol(columnWidth), skillDef.skillLabel.CapitalizeFirst());
                                            Widgets.Label(rtRow, "-");
                                        }
                                        else
                                        {
                                            GUI.color = Color.white;
                                            Widgets.Label(rtRow.NewCol(columnWidth), skillDef.skillLabel.CapitalizeFirst());

                                            var fillPercent = Mathf.Max(0.01f, skillLevel / 20f);
                                            Widgets.FillableBar(rtRow, fillPercent, ITab_AutomataCoreTexture.SkillBarFillTex, null, doBorder: false);

                                            Widgets.Label(rtRow, skillLevel.ToStringCached());
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        break;

                    case AutomataAssembleSummaryTab.PawnCapacity:
                        #region PawnCapacity
                        {
                            var rtListDivider = new RectDivider(rtLeftTabGroup, 57843750);
                            foreach (var pawnCapacityDef in _building.AutomataAssembleUIExtension.capacityDefs)
                            {
                                if (PawnCapacityUtility.BodyCanEverDoCapacity(_samplePawn.RaceProps.body, pawnCapacityDef))
                                {
                                    var efficiencyLabelPair = HealthCardUtility.GetEfficiencyLabel(_samplePawn, pawnCapacityDef);

                                    var capacityLabel = pawnCapacityDef.GetLabelFor(_samplePawn).CapitalizeFirst();
                                    var efficiencyLabel = efficiencyLabelPair.First;
                                    var efficiencyColor = efficiencyLabelPair.Second;
                                    var rtRow = rtListDivider.NewRow(24f);

                                    if (Mouse.IsOver(rtRow))
                                    {
                                        using (new TextBlock(new Color(0.5f, 0.5f, 0.5f, 1f)))
                                        {
                                            GUI.DrawTexture(rtRow, TexUI.HighlightTex);
                                        }
                                    }

                                    using (new TextBlock(TextAnchor.MiddleLeft))
                                    {
                                        Widgets.Label(rtRow, capacityLabel);
                                    }

                                    using (new TextBlock(TextAnchor.MiddleRight, efficiencyColor))
                                    {
                                        Widgets.Label(rtRow, efficiencyLabel);
                                    }
                                }
                            }

                            var rtDivider = rtListDivider.NewRow(2f);
                            Widgets.DrawLineHorizontal(rtListDivider.Rect.xMin, rtDivider.Rect.yMin, rtDivider.Rect.width, new Color(0.3f, 0.3f, 0.3f, 1f));

                            foreach (var statDef in _building.AutomataAssembleUIExtension.statDefs)
                            {
                                var rtRow = rtListDivider.NewRow(24f);
                                var statLabel = statDef.LabelCap;
                                var valueLabel = _samplePawn.GetStatValue(statDef).ToStringByStyle(statDef.toStringStyle);

                                if (Mouse.IsOver(rtRow))
                                {
                                    using (new TextBlock(new Color(0.5f, 0.5f, 0.5f, 1f)))
                                    {
                                        GUI.DrawTexture(rtRow, TexUI.HighlightTex);
                                    }
                                }

                                using (new TextBlock(TextAnchor.MiddleLeft))
                                {
                                    Widgets.Label(rtRow, statLabel);
                                }

                                using (new TextBlock(TextAnchor.MiddleRight))
                                {
                                    Widgets.Label(rtRow, valueLabel);
                                }
                            }
                        }
                        #endregion
                        break;
                }
            }
            finally
            {
                Text.Font = GameFont.Small;
                GUI.color = Color.white;
                Text.Anchor = TextAnchor.UpperLeft;
                Widgets.EndGroup();
            }
        }

        private void DrawTitleSection(Rect rect)
        {
            var rtBase = new RectDivider(rect, 6868689);
            var rtIconRect = rtBase.NewCol(80f);
            Widgets.DrawTextureFitted(rtIconRect, PNTitleIconTex, 1f);

            using (new TextBlock(GameFont.Medium, TextAnchor.MiddleLeft))
            {
                Widgets.Label(rtBase, PNLocale.PN_DialogAssembleTitleLabel.Translate());
            }
        }

        private void DrawSummarySection(Rect rect)
        {
            var rtBase = new RectDivider(rect, 2239155);
            var rtSummaryLeft = rtBase.NewCol(rtBase.Rect.width / 2f, marginOverride: 0f).Rect;
            var rtSummaryRight = rtBase.Rect;

            var rtIngredientsSection = rtSummaryLeft.RightPartPixels(300f).LeftPartPixels(290f);
            Widgets.DrawMenuSection(rtIngredientsSection);
            {

            }

            var rtButtonSection = rtSummaryRight.LeftPartPixels(300f).RightPartPixels(290f);
            Widgets.DrawMenuSection(rtButtonSection);
            {

            }
        }

        private void DrawPortraitSection(Rect rect)
        {
            var rtBase = new RectDivider(rect, 7896853);
            var rtTemp = new Rect(0f, 0f, 140f, 200f);
            rtTemp.center = rtBase.Rect.center;
            rtTemp.y -= 32f;
            GUI.color = Color.white;
            GUI.DrawTexture(rtTemp, PortraitsCache.Get(_samplePawn, new Vector2(210f, 300f), Rot4.South, cameraZoom: 1.5f));

            rtBase.NewRow(4f, VerticalJustification.Bottom, marginOverride: 0f);
            var rtFaceSelector = rtBase.NewRow(28f, VerticalJustification.Bottom, marginOverride: 4f);
            {
                Widgets.DrawMenuSection(rtFaceSelector);

                var headTypes = _samplePawn.def.GetAvailableAlienHeadTypes();
                var index = headTypes.IndexOf(Bill.HeadTypeDef);

                var rtFaceSelectorArrowLeft = rtFaceSelector.NewCol(28f, marginOverride: 0f);
                if (Widgets.ButtonImageFitted(rtFaceSelectorArrowLeft.Rect.ContractedBy(2f), ArrowLeftTex))
                {
                    if (index == 0) { index = headTypes.Count - 1; }
                    else { index--; }

                    Bill.HeadTypeDef = headTypes[index];
                    RefreshModuleUI();
                }

                var rtFaceSelectorArrowRight = rtFaceSelector.NewCol(28f, HorizontalJustification.Right, marginOverride: 0f);
                if (Widgets.ButtonImageFitted(rtFaceSelectorArrowRight.Rect.ContractedBy(2f), ArrowRightTex))
                {
                    index = (index + 1) % headTypes.Count;

                    Bill.HeadTypeDef = headTypes[index];
                    RefreshModuleUI();
                }

                using (new TextBlock(TextAnchor.MiddleLeft))
                {
                    Widgets.Label(rtFaceSelector, PNLocale.PN_DialogHeadSelectorLabel.Translate());
                }

                using (new TextBlock(TextAnchor.MiddleCenter))
                {
                    Widgets.Label(rtFaceSelector, _samplePawn.story.headType.LabelCap);
                }
            }

            var rtHairSelector = rtBase.NewRow(28f, VerticalJustification.Bottom);
            {
                Widgets.DrawMenuSection(rtHairSelector);

                var maxHairAddonIndex = _samplePawn.def.GetBodyAddonVariantCount(0);
                var index = Bill.HairAddonIndex;

                var rtHairSelectorArrowLeft = rtHairSelector.NewCol(28f, marginOverride: 0f);
                if (Widgets.ButtonImageFitted(rtHairSelectorArrowLeft.Rect.ContractedBy(2f), ArrowLeftTex))
                {
                    if (index == 0) { index = maxHairAddonIndex - 1; }
                    else { index--; }

                    Bill.HairAddonIndex = index;
                    RefreshModuleUI();
                }

                var rtHairSelectorArrowRight = rtHairSelector.NewCol(28f, HorizontalJustification.Right, marginOverride: 0f);
                if (Widgets.ButtonImageFitted(rtHairSelectorArrowRight.Rect.ContractedBy(2f), ArrowRightTex))
                {
                    index = (index + 1) % maxHairAddonIndex;

                    Bill.HairAddonIndex = index;
                    RefreshModuleUI();
                }

                using (new TextBlock(TextAnchor.MiddleLeft))
                {
                    Widgets.Label(rtHairSelector, PNLocale.PN_DialogHairSelectorLabel.Translate());
                }

                using (new TextBlock(TextAnchor.MiddleCenter))
                {
                    Widgets.Label(rtHairSelector, ((char)('A' + Bill.HairAddonIndex)).ToString());
                }
            }
        }

        private List<FloatMenuOption> _tmpFloatMenuOptions = new List<FloatMenuOption>();
        private void RefreshModuleUI()
        {
            ModuleElementUIInfo GenerateModuleElementUIInfo(AutomataModulePartDef modulePartDef)
            {
                var moduleSpec = Bill[modulePartDef];

                return new ModuleElementUIInfo()
                {
                    selectButtonLabel = modulePartDef.LabelCap,
                    nameLabel = moduleSpec != null ? moduleSpec.Label : PNLocale.PN_DialogEmptyModuleElementLabel.Translate().ToString(),
                    onSelectButtonClicked = () =>
                    {
                        _tmpFloatMenuOptions.Clear();
                        foreach (var moduleDef in modulePartDef.ModuleDefs)
                        {
                            _tmpFloatMenuOptions.AddRange(moduleDef.IngredientWorker.GetCandidateFloatMenuOptions(modulePartDef, _building.Map, (selection) =>
                            {
                                Bill[modulePartDef] = selection;
                                RefreshModuleUI();
                            }));
                        }

                        if (_tmpFloatMenuOptions.Count == 0)
                        {
                            Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>()
                        {
                            new FloatMenuOption(PNLocale.PN_DialogFloatMenuOptionNoModuleCandidate.Translate(), null)
                        }));
                        }
                        else
                        {
                            Find.WindowStack.Add(new FloatMenu(_tmpFloatMenuOptions));
                        }
                    },
                    descButtonVisible = moduleSpec?.moduleDef != null,
                    onDescButtonClicked = () =>
                    {

                    }
                };
            }

            // System
            _systemModuleElementInfos[0] = GenerateModuleElementUIInfo(PNAutomataModulePartDefOf.PN_Core);
            _systemModuleElementInfos[1] = GenerateModuleElementUIInfo(PNAutomataModulePartDefOf.PN_Chassi);
            _systemModuleElementInfos[2] = GenerateModuleElementUIInfo(PNAutomataModulePartDefOf.PN_Shell);
            _systemModuleElementInfos[3] = GenerateModuleElementUIInfo(PNAutomataModulePartDefOf.PN_CustomModule);

            // Arm
            _armModuleElementInfos[0] = GenerateModuleElementUIInfo(PNAutomataModulePartDefOf.PN_LeftArm);
            _armModuleElementInfos[1] = GenerateModuleElementUIInfo(PNAutomataModulePartDefOf.PN_RightArm);

            // Leg
            _legModuleElementInfos[0] = GenerateModuleElementUIInfo(PNAutomataModulePartDefOf.PN_LeftLeg);
            _legModuleElementInfos[1] = GenerateModuleElementUIInfo(PNAutomataModulePartDefOf.PN_RightLeg);

            _samplePawn?.Destroy();
            _samplePawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(
                PNPawnKindDefOf.PN_ColonistPawn,
                context: PawnGenerationContext.NonPlayer,
                faction: null,
                forceGenerateNewPawn: true,
                canGeneratePawnRelations: false,
                colonistRelationChanceFactor: 0f,
                allowGay: false,
                allowFood: false,
                allowAddictions: false,
                relationWithExtraPawnChanceFactor: 0f,
                forcedTraits: new List<TraitDef>() { },
                forceNoIdeo: true,
                forceNoBackstory: true,
                forceNoGear: true));

            _samplePawn.inventory.DestroyAll();
            _samplePawn.apparel.DestroyAll();

            var traits = new List<Trait>(_samplePawn.story.traits.allTraits);
            foreach (var trait in traits)
            {
                _samplePawn.story.traits.RemoveTrait(trait);
            }

            Bill.ApplyPawn(_samplePawn);

            _samplePawn.Drawer.renderer.SetAllGraphicsDirty();
        }

        private void DrawModuleSection(Rect rect, ModuleElementUIInfo[] elementInfos)
        {
            Widgets.DrawMenuSection(rect);

            var rtSystemModuleDivider = new RectDivider(rect.ContractedBy(4f), 9817340, new Vector2(0f, 0f));
            var rowHeight = rtSystemModuleDivider.Rect.height / elementInfos.Length;

            foreach (var element in elementInfos)
            {
                var rtRow = rtSystemModuleDivider.NewRow(rowHeight);
                var rtInnerRowRect = new Rect(rtRow);

                var rtSelectButton = new Rect(0f, 0f, 85f, 32f);
                rtSelectButton.center = rtRow.NewCol(85f, marginOverride: 2f).Rect.center;
                if (Widgets.ButtonText(rtSelectButton, element.selectButtonLabel))
                {
                    element.onSelectButtonClicked();
                }

                var rtDetailButton = new Rect(0f, 0f, 20f, 20f);
                rtDetailButton = rtRow.NewCol(20f, HorizontalJustification.Right, marginOverride: 2f);
                if (element.descButtonVisible)
                {
                    if (Widgets.ButtonImage(new Rect(0f, 0f, 20f, 20f).CenteredOnXIn(rtDetailButton).CenteredOnYIn(rtDetailButton), OpenStatsReportTex))
                    {
                        element.onDescButtonClicked();
                    }
                }

                try
                {
                    using (new TextBlock(TextAnchor.MiddleCenter))
                    {
                        var label = element.nameLabel;
                        if (Text.CalcSize(label).x > rtRow.Rect.width - 13f)
                        {
                            Text.Font = GameFont.Tiny;
                        }

                        TooltipHandler.TipRegion(rtRow, label);
                        Widgets.Label(rtRow.Rect, label);
                    }
                }
                finally
                {
                    Text.Font = GameFont.Small;
                }
            }
        }
    }
}
