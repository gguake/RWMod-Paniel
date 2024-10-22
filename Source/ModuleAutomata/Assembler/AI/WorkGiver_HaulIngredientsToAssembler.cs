using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ModuleAutomata
{
    public class WorkGiver_HaulIngredientsToAssembler : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(PNThingDefOf.PN_AutomatonAssembleBench);

        public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

        public override bool HasJobOnThing(Pawn pawn, Thing thing, bool forced = false)
        {
            if (!(thing is Building_AutomataAssembler building))
            {
                return false;
            }

            if (building.IsForbidden(pawn) || building.IsBurning())
            {
                return false;
            }

            if (!pawn.CanReserve(building, ignoreOtherReservations: forced) || !pawn.CanReserveSittableOrSpot(building.InteractionCell))
            {
                return false;
            }

            if (pawn.Map.designationManager.DesignationOn(building, DesignationDefOf.Deconstruct) != null)
            {
                return false;
            }

            if (building.Bill == null)
            {
                return false;
            }

            if (building.RequiredIngredients.Count == 0)
            {
                return false;
            }

            var tc = FindIngredients(pawn, building);
            if (tc.Thing == null)
            {
                JobFailReason.Is(PNLocale.PN_JobFailReasonNoIngredients.Translate());
                return false;
            }

            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
        {
            if (!(thing is Building_AutomataAssembler building)) { return null; }

            var bill = building.Bill;
            if (bill == null) { return null; }

            if (building.RequiredIngredients.Count == 0) { return null; }

            var tc = FindIngredients(pawn, building);
            if (tc.Thing == null)
            {
                var job = HaulAIUtility.HaulToContainerJob(pawn, tc.Thing, building);
                job.count = Mathf.Min(job.count, tc.Count);
                return job;
            }

            return null;
        }

        private ThingCount FindIngredients(Pawn pawn, Building_AutomataAssembler building)
        {
            var found = GenClosest.ClosestThingReachable(
                pawn.Position,
                pawn.Map,
                ThingRequest.ForGroup(ThingRequestGroup.HaulableEver),
                PathEndMode.ClosestTouch,
                TraverseParms.For(pawn),
                validator: (thing) =>
                {
                    if (thing.IsForbidden(pawn) || !pawn.CanReserve(thing) || thing.stackCount <= 0) { return false; }

                    foreach (var kv in building.RequiredIngredients)
                    {
                        if (kv.Key.Match(thing))
                        {
                            return true;
                        }
                    }

                    return false;
                });

            if (found == null)
            {
                return default;
            }
            else
            {
                var requiredCount = building.RequiredIngredients.FirstOrDefault(v => v.Key.Match(found)).Value;
                return new ThingCount(found, Mathf.Min(found.stackCount, requiredCount));
            }
        }
    }
}
