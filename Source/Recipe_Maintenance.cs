using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace AutomataRace
{
    public class Recipe_Maintenance : Recipe_Surgery
    {
        public override bool AvailableOnNow(Thing thing)
        {
            Pawn pawn = thing as Pawn;
            if (pawn == null)
            {
                return false;
            }

            return true;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            var need = pawn.needs.AllNeeds.FirstOrDefault(x => x.def == AutomataRaceDefOf.PN_Need_Maintenance);
            if (need != null)
            {
                need.CurLevel = 1.0f;
            }
            else
            {
                Log.Error("Tried to maintenance without Need_Maintenance.");
            }
        }
    }
}
