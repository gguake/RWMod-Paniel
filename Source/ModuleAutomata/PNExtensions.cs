using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ModuleAutomata
{
    internal static class PNExtensions
    {
        public static bool IsAutomata(this Pawn pawn)
        {
            return pawn.def == PNThingDefOf.Paniel_Race;
        }
    }
}
