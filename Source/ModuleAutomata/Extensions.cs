using Verse;

namespace ModuleAutomata
{
    internal static class Extensions
    {
        public static bool IsAutomata(this Pawn pawn)
        {
            return pawn.def == AutomataThingDefOf.Paniel_Race;
        }
    }
}
