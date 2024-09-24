using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ModuleAutomata
{
    public enum AutomataAssembleSummaryTab
    {
        PawnCapacity,
        PawnSkill,
    }

    public struct AutomataModuleDialogElement
    {
        public AutomataModuleDef def;
        public QualityCategory? quality;

        public override string ToString()
        {
            if (quality == null)
            {
                return def.LabelCap;
            }
            else
            {
                return $"{def.LabelCap} ({quality.Value.GetLabelShort()})";
            }
        }
    }

    public class Dialog_AutomataAssemble : Window
    {
        public override Vector2 InitialSize => new Vector2(860f, 440f);

        private AutomataAssembleSummaryTab _currentTab = AutomataAssembleSummaryTab.PawnCapacity;
        private Pawn _samplePawn;

        private Dictionary<AutomataModulePartDef, List<AutomataModuleDialogElement>> _elementsByPartDict;

        public Dialog_AutomataAssemble()
        {
            doCloseX = true;
            forcePause = true;
            absorbInputAroundWindow = true;

            _elementsByPartDict = new Dictionary<AutomataModulePartDef, List<AutomataModuleDialogElement>>();
            foreach (var def in DefDatabase<AutomataModulePartDef>.AllDefsListForReading)
            {
                _elementsByPartDict.Add(def, new List<AutomataModuleDialogElement>());
            }

            RefreshSamplePawn();
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
                    new TabRecord(PNLocale.PN_TabPawnCapacityLabel.Translate(), () =>
                    {
                        _currentTab = AutomataAssembleSummaryTab.PawnCapacity;
                    }, _currentTab == AutomataAssembleSummaryTab.PawnCapacity),

                    new TabRecord(PNLocale.PN_TabPawnSkillLabel.Translate(), () =>
                    {
                        _currentTab = AutomataAssembleSummaryTab.PawnSkill;
                    }, _currentTab == AutomataAssembleSummaryTab.PawnSkill)
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

            var rtCenterSection = rtMain.NewCol(400f);
            try
            {
                Text.Anchor = TextAnchor.MiddleLeft;

                var rtCenterElementLabelSection = rtCenterSection.NewCol(rtCenterSection.Rect.width * 0.375f);
                var rtCenterElementButtonSection = rtCenterSection.NewCol(44f, HorizontalJustification.Right);
                var rtCenterElementValueSection = rtCenterSection;

                var modulePartDefs = AutomataModulePartDef.AllModulePartDefOrdered;
                bool highlight = false;
                foreach (var partDef in modulePartDefs)
                {
                    var elements = _elementsByPartDict[partDef];

                    var partLabelHeight = Text.CalcHeight(partDef.LabelCap, rtCenterElementLabelSection.Rect.width);
                    var partDetailHeightSum = elements.Sum(option => Text.CalcHeight(option.ToString(), rtCenterElementValueSection.Rect.width));

                    var rowHeight = Mathf.Max(partLabelHeight, partDetailHeightSum);
                    var rtElementLabelSection = rtCenterElementLabelSection.NewRow(rowHeight);
                    var rtElementValueSection = rtCenterElementValueSection.NewRow(rowHeight);
                    var rtElementButtonSection = rtCenterElementValueSection.NewRow(rowHeight);

                    var elementGroupRect = new Rect(
                        rtElementLabelSection.Rect.xMin,
                        rtElementLabelSection.Rect.yMin, 
                        rtElementButtonSection.Rect.xMax, 
                        rtElementButtonSection.Rect.yMax);

                    // 배경 하이라이팅
                    if (highlight)
                    {
                        GUI.color = new Color(0.75f, 0.75f, 0.85f, 1f);
                        GUI.DrawTexture(elementGroupRect, TexUI.HighlightTex);
                    }
                    highlight = !highlight;

                    Widgets.DrawHighlightIfMouseover(elementGroupRect);

                    var rtElementLabel = rtElementLabelSection.NewRow(partLabelHeight);
                    Widgets.Label(rtElementLabel, partDef.LabelCap);

                    if (elements.Count == 0)
                    {
                        var rtElementValue = rtElementValueSection.Rect;
                        Widgets.Label(rtElementValue, PNLocale.PN_DialogEmptyModuleElementLabel.Translate());
                    }
                    else
                    {
                        for (int i = 0; i < elements.Count; ++i)
                        {
                            var partElementText = elements[i].ToString();
                            var rtElementValue = rtElementValueSection.NewRow(Text.CalcHeight(partElementText, rtElementValueSection.Rect.width), marginOverride: 0f);
                            Widgets.Label(rtElementValue, partElementText);

                            var buttons = new List<GenUI.AnonymousStackElement>
                            {
                                new GenUI.AnonymousStackElement
                                {
                                    drawer = (rect) =>
                                    {
                                        MouseoverSounds.DoRegion(rect);
                                        if (Widgets.ButtonImage(rect, TexButton.Info, GUI.color))
                                        {
                                            Log.Message(elements[i].ToString());
                                        }
                                        UIHighlighter.HighlightOpportunity(rect, "PN_AutomataModuleOption");
                                    },
                                    width = 20f
                                }
                            };

                            GenUI.DrawElementStack(rtElementButtonSection, rtElementButtonSection.Rect.height, buttons, (r, obj) => obj.drawer(r), obj => obj.width);
                        }
                    }
                }
            }
            finally
            {
                Text.Anchor = TextAnchor.UpperLeft;
            }

            var rtRightSection = rtMain;
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
                forceNoGear: true));

        }
    }
}
