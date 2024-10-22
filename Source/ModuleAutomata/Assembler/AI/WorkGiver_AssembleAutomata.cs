using RimWorld;
using Verse;
using Verse.AI;

namespace ModuleAutomata
{
    public class WorkGiver_AssembleAutomata : WorkGiver_Scanner
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

            if (building.RequiredIngredients.Count > 0 || building.RequiredPawn != null)
            {
                return false;
            }

            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
        {
            if (!(thing is Building_AutomataAssembler building)) { return null; }

            var bill = building.Bill;
            if (bill == null) { return null; }

            if (building.RequiredIngredients.Count > 0 || building.RequiredPawn != null)
            {
                return null;
            }

            var job = JobMaker.MakeJob(PNJobDefOf.PN_DoAssembleBill, building);
            return job;
        }
    }
}
