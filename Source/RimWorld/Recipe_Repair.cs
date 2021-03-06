using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomataRace.Logic;
using RimWorld;
using Verse;

namespace AutomataRace
{
    public class Recipe_Repair : Recipe_Surgery
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
            RepairService.Repair(pawn);
        }
    }
}
