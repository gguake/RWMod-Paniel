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

    [StaticConstructorOnStartup]
    public class Dialog_AutomataAssemble : Window
    {
        public static readonly Texture2D OpenStatsReportTex = ContentFinder<Texture2D>.Get("UI/Buttons/OpenStatsReport");
        public static readonly Texture2D DeleteTex = ContentFinder<Texture2D>.Get("UI/Buttons/Delete");

        public override Vector2 InitialSize => new Vector2(900f, 440f);

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

            var rtCenterSection = rtMain.NewCol(440f);
            try
            {
                Text.Anchor = TextAnchor.MiddleLeft;

                var rtCenterElementLabelSection = rtCenterSection.NewCol(rtCenterSection.Rect.width * 0.35f);
                var rtCenterElementButtonSection = rtCenterSection.NewCol(44f, HorizontalJustification.Right);
                var rtCenterElementValueSection = rtCenterSection;

                var modulePartDefs = AutomataModulePartDef.AllModulePartDefOrdered;
                bool highlight = false;
                foreach (var partDef in modulePartDefs)
                {
                    var elements = _elementsByPartDict[partDef];

                    var partLabelHeight = Text.CalcHeight(partDef.LabelCap, rtCenterElementLabelSection.Rect.width);
                    var partDetailHeightSum = elements.Sum(option => Text.CalcHeight(option.ToString(), rtCenterElementValueSection.Rect.width));

                    var rowHeight = Mathf.Max(
                        24f,
                        partLabelHeight, 
                        partDetailHeightSum, 
                        Text.CalcHeight(PNLocale.PN_DialogEmptyModuleElementLabel.Translate(), rtCenterElementValueSection.Rect.width));

                    var rtElementLabelSection = rtCenterElementLabelSection.NewRow(rowHeight, marginOverride: 0f);
                    var rtElementValueSection = rtCenterElementValueSection.NewRow(rowHeight, marginOverride: 0f);
                    var rtElementButtonSection = rtCenterElementButtonSection.NewRow(rowHeight, marginOverride: 0f);

                    var elementGroupRect = Rect.MinMaxRect(
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

                        foreach (var button in GetModuleElementButtons(partDef, null))
                        {
                            var rtButton = rtElementButtonSection.NewCol(20f, HorizontalJustification.Right);
                            button.drawer(rtButton);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < elements.Count; ++i)
                        {
                            var partElementText = elements[i].ToString();
                            var rtElementValue = rtElementValueSection.NewRow(
                                Text.CalcHeight(partElementText, rtElementValueSection.Rect.width), 
                                marginOverride: 0f);

                            Widgets.Label(rtElementValue, partElementText);

                            foreach (var button in GetModuleElementButtons(partDef, null))
                            {
                                var rtButton = rtElementButtonSection.NewCol(20f, HorizontalJustification.Right);
                                button.drawer(rtButton);
                            }
                        }
                    }
                }
            }
            finally
            {
                Text.Anchor = TextAnchor.UpperLeft;
            }

            var rtRightSection = rtMain;
            try
            {
                var rtPortraitSection = rtRightSection.NewRow(200f);
                {
                    var rtTemp = new Rect(0f, 0f, 140f, 200f);
                    rtTemp.center = rtPortraitSection.Rect.center;
                    GUI.color = Color.white;
                    GUI.DrawTexture(rtTemp, PortraitsCache.Get(_samplePawn, new Vector2(140f, 200f), Rot4.South));
                }
            }
            finally
            {
                GUI.color = Color.white;
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
                validatorPostGear: p => !p.apparel.AnyApparel));

        }

        private IEnumerable<GenUI.AnonymousStackElement> GetModuleElementButtons(AutomataModulePartDef modulePartDef, AutomataModuleDialogElement? element)
        {
            if (element == null)
            {
                yield return new GenUI.AnonymousStackElement
                {
                    drawer = (rect) =>
                    {
                        MouseoverSounds.DoRegion(rect);
                        if (Widgets.ButtonImage(rect, OpenStatsReportTex, GUI.color))
                        {
                            Log.Message(modulePartDef.LabelCap);
                        }
                        UIHighlighter.HighlightOpportunity(rect, "PN_AutomataModuleOption");
                    },
                    width = 20f
                };
            }
            else
            {
                yield return new GenUI.AnonymousStackElement
                {
                    drawer = (rect) =>
                    {
                        MouseoverSounds.DoRegion(rect);
                        if (Widgets.ButtonImage(rect, OpenStatsReportTex, GUI.color))
                        {
                            Log.Message(element.ToString());
                        }
                        UIHighlighter.HighlightOpportunity(rect, "PN_AutomataModuleOption");
                    },
                    width = 20f
                };

                yield return new GenUI.AnonymousStackElement
                {
                    drawer = (rect) =>
                    {
                        MouseoverSounds.DoRegion(rect);
                        if (Widgets.ButtonImage(rect, DeleteTex, GUI.color))
                        {
                            Log.Message($"delete");
                        }
                        UIHighlighter.HighlightOpportunity(rect, "PN_AutomataModuleOption");
                    },
                    width = 20f
                };
            }
        }
    }
}
