using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ModuleAutomata
{
    public enum AutomataAssembleSummaryTab
    {
        PawnCapacity,
        PawnSkill,
    }

    public class Dialog_AutomataAssemble : Window
    {
        public override Vector2 InitialSize => new Vector2(860f, 440f);

        private AutomataAssembleBill _bill;
        private AutomataAssembleSummaryTab _currentTab = AutomataAssembleSummaryTab.PawnCapacity;

        public Dialog_AutomataAssemble(Pawn sourcePawn = null)
        {
            doCloseX = true;
            forcePause = true;
            absorbInputAroundWindow = true;

            if (sourcePawn != null )
            {
                _bill = new AutomataAssembleBill(sourcePawn);
            }
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
            {

            }

            var rtRightSection = rtMain;
        }
    }
}
