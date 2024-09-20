using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AutomataRace
{
    public class AutomataQualityProperty : Def
    {
        public QualityCategory quality;
        public Dictionary<AutomataSpecializationDef, PawnKindDef> pawnKindDefs;
        public SimpleCurve scoreCurve;

        public static AutomataQualityProperty GetQualityProperty(QualityCategory quality)
        {
            switch (quality)
            {
                case QualityCategory.Awful:
                    return AutomataRaceDefOf.PN_AutomataQualityProperty_Awful;

                case QualityCategory.Poor:
                    return AutomataRaceDefOf.PN_AutomataQualityProperty_Poor;

                case QualityCategory.Normal:
                    return AutomataRaceDefOf.PN_AutomataQualityProperty_Normal;

                case QualityCategory.Good:
                    return AutomataRaceDefOf.PN_AutomataQualityProperty_Good;

                case QualityCategory.Excellent:
                    return AutomataRaceDefOf.PN_AutomataQualityProperty_Excellent;

                case QualityCategory.Masterwork:
                    return AutomataRaceDefOf.PN_AutomataQualityProperty_Masterwork;

                case QualityCategory.Legendary:
                    return AutomataRaceDefOf.PN_AutomataQualityProperty_Legendary;

                default:
                    return null;
            }
        }
    }
}
