using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ModuleAutomata
{
    public enum AutomataAssembleSummaryTab
    {
        PawnSkill,
        PawnCapacity,
        PawnAppearance,
    }

    [StaticConstructorOnStartup]
    public class Dialog_AutomataAssemble : Window
    {
        public struct ModuleElementInfo
        {
            public string selectButtonLabel;
            public Action onSelectButtonClicked;

            public string nameLabel;
            public bool descButtonVisible;
            public Action onDescButtonClicked;
        }
        private ModuleElementInfo[] _systemModuleElementInfos = new ModuleElementInfo[4];
        private ModuleElementInfo[] _armModuleElementInfos = new ModuleElementInfo[2];
        private ModuleElementInfo[] _legModuleElementInfos = new ModuleElementInfo[2];

        public static readonly Texture2D OpenStatsReportTex = ContentFinder<Texture2D>.Get("UI/Buttons/OpenStatsReport");
        public static readonly Texture2D PNTitleIconTex = ContentFinder<Texture2D>.Get("UI/Icons/PNCT_PnL");

        public override Vector2 InitialSize => new Vector2(1000f, 520f);

        private AutomataAssembleSummaryTab _currentTab = AutomataAssembleSummaryTab.PawnSkill;
        private Pawn _samplePawn;

        private Building_AutomataAssembler _building;
        private Action<AutomataAssembleBill> _callback;
        private Pawn _targetPawn;

        public AutomataAssembleBill Bill { get; private set; } = new AutomataAssembleBill();

        public Dialog_AutomataAssemble(Building_AutomataAssembler building, Pawn pawn, Action<AutomataAssembleBill> callback)
        {
            _building = building;
            _callback = callback;
            _targetPawn = pawn;

            doCloseX = true;
            forcePause = true;
            absorbInputAroundWindow = true;

            InitializeBillFromDefaultSetting();
            RefreshModuleUI();
        }

        public Dialog_AutomataAssemble(Building_AutomataAssembler building, Action<AutomataAssembleBill> callback)
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
            {
                _ = rtLeftSection.NewRow(24f);

                GUI.color = Color.white;
                Widgets.DrawMenuSection(rtLeftSection.Rect);

                var tabs = new List<TabRecord>()
                {
                    new TabRecord(PNLocale.PN_DialogTabPawnSkillLabel.Translate(), () =>
                    {
                        _currentTab = AutomataAssembleSummaryTab.PawnSkill;
                    }, _currentTab == AutomataAssembleSummaryTab.PawnSkill),

                    new TabRecord(PNLocale.PN_DialogTabPawnCapacityLabel.Translate(), () =>
                    {
                        _currentTab = AutomataAssembleSummaryTab.PawnCapacity;
                    }, _currentTab == AutomataAssembleSummaryTab.PawnCapacity),

                    new TabRecord(PNLocale.PN_DialogTabPawnAppearanceLabel.Translate(), () =>
                    {
                        _currentTab = AutomataAssembleSummaryTab.PawnAppearance;
                    }, _currentTab == AutomataAssembleSummaryTab.PawnAppearance)
                };

                TabDrawer.DrawTabs(rtLeftSection, tabs);

                try
                {
                    var rtLeftTabBody = rtLeftSection.Rect.ContractedBy(9f);
                    Widgets.BeginGroup(rtLeftTabBody);

                    var rtLeftTabGroup = new Rect(0, 0, rtLeftTabBody.width, rtLeftTabBody.height);
                    switch (_currentTab)
                    {
                        case AutomataAssembleSummaryTab.PawnSkill:
                            #region PawnSkill
                            {
                                var coreModule = Bill[PNAutomataModulePartDefOf.PN_Core];
                                if (coreModule.IsInvalid || coreModule.thing == null)
                                {
                                    Text.Anchor = TextAnchor.MiddleCenter;
                                    Widgets.Label(rtLeftTabGroup, PNLocale.PN_DialogTabNoSelectedCoreLabel.Translate());
                                    Text.Anchor = TextAnchor.UpperLeft;
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

                                        Text.Anchor = TextAnchor.MiddleLeft;
                                        Widgets.Label(rtTitle.Rect, coreModule.thing.LabelCap);

                                        Text.Anchor = TextAnchor.UpperLeft;
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

                                        Text.Anchor = TextAnchor.MiddleLeft;
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

                                            Text.Anchor = TextAnchor.MiddleLeft;
                                            Widgets.Label(rtRow, skillLevel.ToStringCached());
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

                                        Text.Anchor = TextAnchor.MiddleLeft;
                                        Widgets.Label(rtRow, capacityLabel);

                                        Text.Anchor = TextAnchor.MiddleRight;
                                        GUI.color = efficiencyColor;
                                        Widgets.Label(rtRow, efficiencyLabel);

                                        Text.Anchor = TextAnchor.UpperLeft;
                                        GUI.color = Color.white;
                                    }
                                }

                                var rtDivider = rtListDivider.NewRow(2f);
                                Widgets.DrawLineHorizontal(rtListDivider.Rect.xMin, rtDivider.Rect.yMin, rtDivider.Rect.width, new Color(0.3f, 0.3f, 0.3f, 1f));

                                foreach (var statDef in _building.AutomataAssembleUIExtension.statDefs)
                                {
                                    var rtRow = rtListDivider.NewRow(24f);
                                    var statLabel = statDef.LabelCap;
                                    var valueLabel = _samplePawn.GetStatValue(statDef).ToStringByStyle(statDef.toStringStyle);

                                    Text.Anchor = TextAnchor.MiddleLeft;
                                    Widgets.Label(rtRow, statLabel);

                                    Text.Anchor = TextAnchor.MiddleRight;
                                    Widgets.Label(rtRow, valueLabel);

                                    Text.Anchor = TextAnchor.UpperLeft;
                                }
                            }
                            #endregion
                            break;

                        case AutomataAssembleSummaryTab.PawnAppearance:
                            #region PawnAppareance
                            {

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

            var rtLabelNameSection = rtMain.NewRow(80f);
            {
                var rtIconRect = rtLabelNameSection.NewCol(80f);
                Widgets.DrawTextureFitted(rtIconRect, PNTitleIconTex, 1f);

                try
                {
                    Text.Anchor = TextAnchor.MiddleLeft;
                    Text.Font = GameFont.Medium;

                    Widgets.Label(rtLabelNameSection, PNLocale.PN_DialogAssembleTitleLabel.Translate());
                }
                finally
                {
                    Text.Anchor = TextAnchor.UpperLeft;
                    Text.Font = GameFont.Small;
                }
            }

            var rtSummarySection = rtMain.NewRow(120f, VerticalJustification.Bottom);
            {
                var centerRect = new Rect(0f, 0f, 280f, 100f);
                centerRect.center = rtSummarySection.Rect.center;
                Widgets.DrawMenuSection(centerRect);
            }

            var rtSystemModuleSection = rtMain.NewCol(260f);
            {
                var centerRect = rtSystemModuleSection.Rect.ContractedBy(0f, 4f);
                DrawModuleUI(centerRect, _systemModuleElementInfos);
            }

            var rtArmLegModuleSection = rtMain.NewCol(260f, HorizontalJustification.Right);
            {
                var rtArmModuleSection = rtArmLegModuleSection.NewRow(rtArmLegModuleSection.Rect.height / 2f, marginOverride: 0f);
                {
                    var centerRect = rtArmModuleSection.Rect.ContractedBy(4f);
                    DrawModuleUI(centerRect, _armModuleElementInfos);

                }

                var rtLegModuleSection = rtArmLegModuleSection.Rect;
                {
                    var centerRect = rtLegModuleSection.ContractedBy(4f);
                    DrawModuleUI(centerRect, _legModuleElementInfos);
                }
            }

            var rtPortraitSection = rtMain;
            {
                var rtTemp = new Rect(0f, 0f, 140f, 200f);
                rtTemp.center = rtPortraitSection.Rect.center;
                GUI.color = Color.white;
                GUI.DrawTexture(rtTemp, PortraitsCache.Get(_samplePawn, new Vector2(210f, 300f), Rot4.South, cameraZoom: 1.5f));
            }
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
                Bill[setting.modulePartDef] = new AutomataModuleBill()
                {
                    modulePartDef = setting.modulePartDef,
                    moduleDef = setting.moduleDef,
                    thingDef = setting.moduleDef.ingredientThingDef,
                };
            }
        }

        private void RefreshSamplePawn()
        {
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
        }

        private void RefreshModuleUI()
        {
            // System
            _systemModuleElementInfos[0] = GenerateModuleElementInfo(PNAutomataModulePartDefOf.PN_Core);
            _systemModuleElementInfos[1] = GenerateModuleElementInfo(PNAutomataModulePartDefOf.PN_Chassi);
            _systemModuleElementInfos[2] = GenerateModuleElementInfo(PNAutomataModulePartDefOf.PN_Shell);
            _systemModuleElementInfos[3] = GenerateModuleElementInfo(PNAutomataModulePartDefOf.PN_CustomModule);

            // Arm
            _armModuleElementInfos[0] = GenerateModuleElementInfo(PNAutomataModulePartDefOf.PN_LeftArm);
            _armModuleElementInfos[1] = GenerateModuleElementInfo(PNAutomataModulePartDefOf.PN_RightArm);

            // Leg
            _legModuleElementInfos[0] = GenerateModuleElementInfo(PNAutomataModulePartDefOf.PN_LeftLeg);
            _legModuleElementInfos[1] = GenerateModuleElementInfo(PNAutomataModulePartDefOf.PN_RightLeg);

            RefreshSamplePawn();
        }

        private List<FloatMenuOption> _tmpFloatMenuOptions = new List<FloatMenuOption>();
        private ModuleElementInfo GenerateModuleElementInfo(AutomataModulePartDef modulePartDef)
        {
            var moduleBill = Bill[modulePartDef];

            return new ModuleElementInfo()
            {
                selectButtonLabel = modulePartDef.LabelCap,
                nameLabel = moduleBill.IsInvalid ? PNLocale.PN_DialogEmptyModuleElementLabel.Translate().ToString() : moduleBill.Label,
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
                descButtonVisible = moduleBill.moduleDef != null,
                onDescButtonClicked = () =>
                {

                }
            };
        }

        private void DrawModuleUI(Rect rect, ModuleElementInfo[] elementInfos)
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
                    Text.Anchor = TextAnchor.MiddleCenter;
                    var label = element.nameLabel;
                    if (Text.CalcSize(label).x > rtRow.Rect.width - 13f)
                    {
                        Text.Font = GameFont.Tiny;
                    }

                    TooltipHandler.TipRegion(rtRow, label);
                    Widgets.Label(rtRow.Rect, label);
                }
                finally
                {
                    Text.Anchor = TextAnchor.UpperLeft;
                    Text.Font = GameFont.Small;
                }
            }
        }
    }
}
