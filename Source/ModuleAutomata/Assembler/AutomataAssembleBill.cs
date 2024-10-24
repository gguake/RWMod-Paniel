using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ModuleAutomata
{
    public class AutomataAssembleBill : IExposable
    {
        public Building_AutomataAssembler building;

        public AutomataModificationPlan plan;
        public Pawn pawn;
        public float lastWorkAmount = -1;

        public bool IsStarted => lastWorkAmount >= 0;

        public AutomataAssembleBill(Building_AutomataAssembler building)
        {
            this.building = building;
        }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref plan, "plan");
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Values.Look(ref lastWorkAmount, "lastWorkAmount");
        }

        public void Start()
        {
            lastWorkAmount = plan.TotalWorkAmount;
        }

        public void Complete()
        {
            var targetPawn = pawn;
            if (targetPawn == null)
            {
                targetPawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(
                    PNPawnKindDefOf.PN_ColonistPawn,
                    context: PawnGenerationContext.NonPlayer,
                    faction: null,
                    forceGenerateNewPawn: true,
                    canGeneratePawnRelations: false,
                    colonistRelationChanceFactor: 0f,
                    allowGay: false,
                    allowFood: false,
                    allowAddictions: false,
                    relationWithExtraPawnChanceFactor: 0f,
                    forcedTraits: new List<TraitDef>() { },
                    forceNoIdeo: true,
                    forceNoBackstory: true,
                    forceNoGear: true,
                    fixedBiologicalAge: 0,
                    fixedChronologicalAge: 0));

                targetPawn.inventory.DestroyAll();
                targetPawn.apparel.DestroyAll();

                building.GetDirectlyHeldThings().TryAdd(targetPawn);
            }

            plan.ApplyPawn(targetPawn);
            targetPawn.Drawer.renderer.SetAllGraphicsDirty();
        }
    }
}
