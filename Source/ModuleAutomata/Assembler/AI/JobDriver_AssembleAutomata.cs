using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ModuleAutomata
{
    public class JobDriver_AssembleAutomata : JobDriver
    {
        private const TargetIndex BuildingIndex = TargetIndex.A;
        private Building_AutomataAssembler Building => job.GetTarget(BuildingIndex).Thing as Building_AutomataAssembler;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (!pawn.Reserve(Building, job, errorOnFailed: errorOnFailed))
            {
                return false;
            }

            if (!pawn.ReserveSittableOrSpot(Building.InteractionCell, job, errorOnFailed))
            {
                return false;
            }

            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            AddEndCondition(() =>
            {
                var thing = GetActor().jobs.curJob.GetTarget(BuildingIndex).Thing;
                return (!(thing is Building) || thing.Spawned) ? JobCondition.Ongoing : JobCondition.Incompletable;
            });

            this.FailOnBurningImmobile(BuildingIndex);
            this.FailOn(() => Building.Bill == null);

            yield return Toils_Goto.GotoThing(BuildingIndex, PathEndMode.InteractionCell);
            var toil = ToilMaker.MakeToil(nameof(JobDriver_AssembleAutomata));
            toil.handlingFacing = true;
            toil.defaultCompleteMode = ToilCompleteMode.Never;
            toil.FailOnCannotTouch(BuildingIndex, PathEndMode.Touch);
            toil.WithEffect(EffecterDefOf.ConstructMetal, TargetIndex.A);
            toil.WithProgressBar(BuildingIndex, () => (Building.Bill.plan.TotalWorkAmount - Building.Bill.lastWorkAmount) / Building.Bill.plan.TotalWorkAmount);
            toil.tickAction = () =>
            {
                pawn.rotationTracker.FaceTarget(Building);

                float statValue = pawn.GetStatValue(StatDefOf.GeneralLaborSpeed);
                Building.Bill.lastWorkAmount = Mathf.Max(0f, Building.Bill.lastWorkAmount - statValue);

                if (Building.Bill.lastWorkAmount == 0f)
                {
                    Building.CompleteBill();
                    ReadyForNextToil();
                }
            };

        }
    }
}
