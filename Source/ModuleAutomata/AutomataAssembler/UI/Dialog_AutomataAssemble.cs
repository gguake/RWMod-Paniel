using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
using static RimWorld.BaseGen.SymbolStack;

namespace ModuleAutomata
{
    public enum AutomataAssembleSummaryTab
    {
        PawnCapacity,
        PawnSkill,
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
            public Action onDescButtonClicked;
        }
        private ModuleElementInfo[] _systemModuleElementInfos = new ModuleElementInfo[4];
        private ModuleElementInfo[] _armModuleElementInfos = new ModuleElementInfo[2];
        private ModuleElementInfo[] _legModuleElementInfos = new ModuleElementInfo[2];

        public static readonly Texture2D OpenStatsReportTex = ContentFinder<Texture2D>.Get("UI/Buttons/OpenStatsReport");
        public static readonly Texture2D PNTitleIconTex = ContentFinder<Texture2D>.Get("UI/Icons/PNCT_PnL");

        public override Vector2 InitialSize => new Vector2(1000f, 450f);

        private AutomataAssembleSummaryTab _currentTab = AutomataAssembleSummaryTab.PawnCapacity;
        private Pawn _samplePawn;

        public Dialog_AutomataAssemble()
        {
            doCloseX = true;
            forcePause = true;
            absorbInputAroundWindow = true;

            RefreshSamplePawn();
            RefreshModuleUI();
        }

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
                    new TabRecord(PNLocale.PN_DialogTabPawnCapacityLabel.Translate(), () =>
                    {
                        _currentTab = AutomataAssembleSummaryTab.PawnCapacity;
                    }, _currentTab == AutomataAssembleSummaryTab.PawnCapacity),

                    new TabRecord(PNLocale.PN_DialogTabPawnSkillLabel.Translate(), () =>
                    {
                        _currentTab = AutomataAssembleSummaryTab.PawnSkill;
                    }, _currentTab == AutomataAssembleSummaryTab.PawnSkill),

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

                    switch (_currentTab)
                    {
                        case AutomataAssembleSummaryTab.PawnCapacity:
                            break;

                        case AutomataAssembleSummaryTab.PawnSkill:
                            break;

                        case AutomataAssembleSummaryTab.PawnAppearance:
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

            var rtLabelNameSection = rtMain.NewRow(70f);
            {
                var rtIconRect = rtLabelNameSection.NewCol(70f);
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
                var centerRect = new Rect(0f, 0f, 280f, 120f);
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
                GUI.DrawTexture(rtTemp, PortraitsCache.Get(_samplePawn, new Vector2(140f, 200f), Rot4.South));
            }
        }

        public override void Close(bool doCloseSound = true)
        {
            _samplePawn?.Destroy();

            base.Close(doCloseSound);
        }

        private void RefreshSamplePawn()
        {
            _samplePawn?.Destroy();
            _samplePawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(
                PNPawnKindDefOf.PN_ColonistPawn,
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
                forceNoGear: true,
                developmentalStages: DevelopmentalStage.Newborn,
                validatorPostGear: p => !p.apparel.AnyApparel));

        }

        private void RefreshModuleUI()
        {
            // System
            _systemModuleElementInfos[0] = new ModuleElementInfo()
            {
                selectButtonLabel = PNAutomataModulePartDefOf.PN_Core.LabelCap,
                nameLabel = PNLocale.PN_DialogEmptyModuleElementLabel.Translate(),
                onSelectButtonClicked = () =>
                {

                },
                onDescButtonClicked = () =>
                {

                }
            };

            _systemModuleElementInfos[1] = new ModuleElementInfo()
            {
                selectButtonLabel = PNAutomataModulePartDefOf.PN_Chassi.LabelCap,
                nameLabel = PNLocale.PN_DialogEmptyModuleElementLabel.Translate(),
                onSelectButtonClicked = () =>
                {

                },
                onDescButtonClicked = () =>
                {

                }
            };

            _systemModuleElementInfos[2] = new ModuleElementInfo()
            {
                selectButtonLabel = PNAutomataModulePartDefOf.PN_Shell.LabelCap,
                nameLabel = PNLocale.PN_DialogEmptyModuleElementLabel.Translate(),
                onSelectButtonClicked = () =>
                {

                },
                onDescButtonClicked = () =>
                {

                }
            };

            _systemModuleElementInfos[3] = new ModuleElementInfo()
            {
                selectButtonLabel = PNAutomataModulePartDefOf.PN_CustomModule.LabelCap,
                nameLabel = PNLocale.PN_DialogEmptyModuleElementLabel.Translate(),
                onSelectButtonClicked = () =>
                {

                },
                onDescButtonClicked = () =>
                {

                }
            };

            // Arm
            _armModuleElementInfos[0] = new ModuleElementInfo()
            {
                selectButtonLabel = PNAutomataModulePartDefOf.PN_LeftArm.LabelCap,
                nameLabel = PNLocale.PN_DialogEmptyModuleElementLabel.Translate(),
                onSelectButtonClicked = () =>
                {

                },
                onDescButtonClicked = () =>
                {

                }
            };

            _armModuleElementInfos[1] = new ModuleElementInfo()
            {
                selectButtonLabel = PNAutomataModulePartDefOf.PN_RightArm.LabelCap,
                nameLabel = PNLocale.PN_DialogEmptyModuleElementLabel.Translate(),
                onSelectButtonClicked = () =>
                {

                },
                onDescButtonClicked = () =>
                {

                }
            };

            // Leg
            _legModuleElementInfos[0] = new ModuleElementInfo()
            {
                selectButtonLabel = PNAutomataModulePartDefOf.PN_LeftLeg.LabelCap,
                nameLabel = PNLocale.PN_DialogEmptyModuleElementLabel.Translate(),
                onSelectButtonClicked = () =>
                {

                },
                onDescButtonClicked = () =>
                {

                }
            };

            _legModuleElementInfos[1] = new ModuleElementInfo()
            {
                selectButtonLabel = PNAutomataModulePartDefOf.PN_RightLeg.LabelCap,
                nameLabel = PNLocale.PN_DialogEmptyModuleElementLabel.Translate(),
                onSelectButtonClicked = () =>
                {

                },
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
                if (Widgets.ButtonImage(new Rect(0f, 0f, 20f, 20f).CenteredOnXIn(rtDetailButton).CenteredOnYIn(rtDetailButton), OpenStatsReportTex))
                {
                    element.onDescButtonClicked();
                }

                try
                {
                    Text.Anchor = TextAnchor.MiddleCenter;
                    var label = element.nameLabel;
                    if (Text.CalcSize(label).x > rtRow.Rect.width - 13f)
                    {
                        Text.Font = GameFont.Tiny;
                    }

                    if (Text.CalcSize(label).x > rtRow.Rect.width - 13f)
                    {
                        TooltipHandler.TipRegion(rtRow, label);
                    }

                    Widgets.LabelEllipses(rtRow.Rect, label);
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
