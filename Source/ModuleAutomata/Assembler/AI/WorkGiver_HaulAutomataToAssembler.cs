using RimWorld;
using Verse;
using Verse.AI;

namespace ModuleAutomata
{
    public class WorkGiver_HaulAutomataToAssembler : WorkGiver_Scanner
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

            var requiredPawn = building.RequiredPawn;
            if (requiredPawn == null)
            {
                return false;
            }

            if (requiredPawn == pawn)
            {
                return true;
            }
            else
            {
                return pawn.CanReserveAndReach(requiredPawn, PathEndMode.ClosestTouch, Danger.Deadly) && requiredPawn.Downed;
            }
        }

        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
        {
            if (!(thing is Building_AutomataAssembler building))
            {
                return null;
            }

            var requiredPawn = building.RequiredPawn;
            if (requiredPawn == null)
            {
                return null;
            }

            if (requiredPawn == pawn)
            {
                return JobMaker.MakeJob(PNJobDefOf.PN_EnterAssembler, building);
            }
            else
            {
                var job = HaulAIUtility.HaulToContainerJob(pawn, requiredPawn, building);
                return job;
            }
        }
    }
}
