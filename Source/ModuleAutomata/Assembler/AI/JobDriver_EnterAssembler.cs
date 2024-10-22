using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ModuleAutomata
{
    public class JobDriver_EnterAssembler : JobDriver
    {
        public const int EnterDelay = 60;

        private const TargetIndex BuildingIndex = TargetIndex.A;
        private Building_AutomataAssembler Building => job.GetTarget(BuildingIndex).Thing as Building_AutomataAssembler;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(Building, job, errorOnFailed: errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOn(() => Building.Bill == null || Building.RequiredPawn != pawn);

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            yield return Toils_General.WaitWith(TargetIndex.A, EnterDelay, useProgressBar: true);
            yield return Toils_General.Do(() =>
            {
                var innerContainer = Building.TryGetInnerInteractableThingOwner();

                var selected = pawn.DeSpawnOrDeselect();
                innerContainer.TryAddOrTransfer(pawn);

                if (selected)
                {
                    Find.Selector.Select(pawn, playSound: false, forceDesignatorDeselect: false);
                }
            });
        }
    }
}
