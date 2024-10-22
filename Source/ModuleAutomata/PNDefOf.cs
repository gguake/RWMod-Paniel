using RimWorld;
using Verse;

namespace ModuleAutomata
{
    [DefOf]
    public static class PNThingDefOf
    {
        public static ThingDef Paniel_Race;

        public static ThingDef PN_AutomatonAssembleBench;
    }

    [DefOf]
    public static class PNBodyPartDefOf
    {
        public static BodyPartDef Brain;
    }

    [DefOf]
    public static class PNInspirationDefOf
    {
        public static InspirationDef Inspired_Surgery;
    }

    [DefOf]
    public static class PNPawnKindDefOf
    {
        public static PawnKindDef PN_ColonistPawn;
    }

    [DefOf]
    public static class PNAutomataModulePartDefOf
    {
        public static AutomataModulePartDef PN_Core;
        public static AutomataModulePartDef PN_Chassi;
        public static AutomataModulePartDef PN_Shell;

        public static AutomataModulePartDef PN_CustomModule;

        public static AutomataModulePartDef PN_LeftArm;
        public static AutomataModulePartDef PN_RightArm;

        public static AutomataModulePartDef PN_LeftLeg;
        public static AutomataModulePartDef PN_RightLeg;
    }

    [DefOf]
    public static class PNJobDefOf
    {
        public static JobDef PN_DoAssembleBill;
        public static JobDef PN_EnterAssembler;
    }
}
