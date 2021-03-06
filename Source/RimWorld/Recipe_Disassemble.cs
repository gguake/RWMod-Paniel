using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AutomataRace
{
    public class Recipe_Disassemble : Recipe_Surgery
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
            List<Thing> things = new List<Thing>();
            var productDefs = pawn.def.butcherProducts;
            if (!productDefs.NullOrEmpty())
            {
                foreach (var thingDefCountClass in productDefs)
                {
                    Thing thing = ThingMaker.MakeThing(thingDefCountClass.thingDef);
                    thing.stackCount = thingDefCountClass.count;
                    
                    if (thing.def.Minifiable)
                    {
                        thing = thing.MakeMinified();
                    }

                    things.Add(thing);
                }
            }

            var position = pawn.Position;
            var map = pawn.Map;

            pawn.Destroy(DestroyMode.KillFinalize);

            foreach (Thing thing in things)
            {
                if (!GenPlace.TryPlaceThing(thing, position, map, ThingPlaceMode.Near))
                {
                    Log.Error($"Error on generate disassemble products near {billDoer.Position}.");
                    return;
                }
            }
        }
    }
}
