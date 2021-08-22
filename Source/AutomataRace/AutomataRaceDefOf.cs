using CustomizableRecipe;
using RimWorld;
using Verse;

namespace AutomataRace
{
    [DefOf]
    public static class AutomataRaceDefOf
    {
        public static PawnKindDef Paniel_Randombox_Normal;
        public static NeedDef PN_Need_Maintenance;
        public static HediffDef PN_OilLoss;
        public static HediffDef PN_Maintenance;
        public static ThingDef PN_Brain;

        public static ThingDef Paniel_Race;

        public static CustomizableRecipeDef PN_Make_Automaton;

        public static AutomataSpecializationDef PN_Specialization_Combat;
        public static AutomataSpecializationDef PN_Specialization_Engineer;
        public static AutomataSpecializationDef PN_Specialization_Domestic;

        public static AutomataQualityProperty PN_AutomataQualityProperty_Awful;
        public static AutomataQualityProperty PN_AutomataQualityProperty_Poor;
        public static AutomataQualityProperty PN_AutomataQualityProperty_Normal;
        public static AutomataQualityProperty PN_AutomataQualityProperty_Good;
        public static AutomataQualityProperty PN_AutomataQualityProperty_Excellent;
        public static AutomataQualityProperty PN_AutomataQualityProperty_Masterwork;
        public static AutomataQualityProperty PN_AutomataQualityProperty_Legendary;
    }
}
