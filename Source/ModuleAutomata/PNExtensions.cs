using System;
using Verse;

namespace ModuleAutomata
{
    internal static class PNExtensions
    {
        private static AutomataModExtension _automataExtension;
        public static AutomataModExtension GetAutomataExtension()
        {
            if (_automataExtension == null)
            {
                _automataExtension = PNThingDefOf.Paniel_Race.GetModExtension<AutomataModExtension>();
            }
            return _automataExtension;
        }

        public static bool IsAutomata(this Pawn pawn)
        {
            return pawn.def == PNThingDefOf.Paniel_Race;
        }

        public static AutomataModuleSpec GetModuleSpec(this Pawn pawn, AutomataModulePartDef partDef)
        {
            if (!pawn.IsAutomata()) { return null; }

            throw new NotImplementedException();
        }
    }
}
