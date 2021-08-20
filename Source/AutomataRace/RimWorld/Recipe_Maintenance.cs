using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AutomataRace
{
    public class Recipe_Maintenance : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            var need = pawn?.needs?.AllNeeds?.FirstOrDefault(x => x.def == AutomataRaceDefOf.PN_Need_Maintenance);
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
