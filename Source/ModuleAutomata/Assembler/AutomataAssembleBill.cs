using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ModuleAutomata
{
    public class AutomataAssembleBill : IExposable
    {
        private Building_AutomataAssembler _building;

        public AutomataModificationPlan Plan => _plan;
        private AutomataModificationPlan _plan;

        public Pawn Pawn => _pawn;
        private Pawn _pawn;

        public float lastWorkAmount = -1;

        public AutomataAssembleBill(Building_AutomataAssembler building, AutomataModificationPlan plan, Pawn pawn = null)
        {
            _building = building;
            _plan = plan;
            _pawn = pawn;

            lastWorkAmount = plan.TotalWorkAmount;
        }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref _plan, "plan");
            Scribe_References.Look(ref _pawn, "pawn");
            Scribe_Values.Look(ref lastWorkAmount, "lastWorkAmount");
        }

        public void Complete()
        {
            var targetPawn = _pawn;
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
                    forceNoGear: true,
                    fixedBiologicalAge: 0,
                    fixedChronologicalAge: 0));

                targetPawn.inventory.DestroyAll();
                targetPawn.apparel.DestroyAll();

                targetPawn.SetFaction(Faction.OfPlayer);

                _building.GetDirectlyHeldThings().TryAdd(targetPawn);

                Messages.Message(PNLocale.PN_MessageAssembleNewAutomataComplete.Translate(targetPawn.Name.ToStringShort), targetPawn, MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                Messages.Message(PNLocale.PN_MessageModifyAutomataComplete.Translate(targetPawn.Name.ToStringShort), targetPawn, MessageTypeDefOf.PositiveEvent);
            }

            _plan.ApplyPawn(targetPawn);
            targetPawn.Drawer.renderer.SetAllGraphicsDirty();

        }
    }
}
