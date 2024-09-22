using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ModuleAutomata
{
    public class Building_AutomataAssembler : Building, IThingHolder, INotifyHauledTo
    {
        private AutomataAssembleBill _bill;
        private ThingOwner _innerContainer;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Deep.Look(ref _bill, "bill");
            Scribe_Deep.Look(ref _innerContainer, "innerContainer", this);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            yield break;
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            EjectContents();
            base.DeSpawn(mode);
        }

        public override void Tick()
        {
            base.Tick();
            _innerContainer.ThingOwnerTick();

            Tick(1);
        }

        public override void TickRare()
        {
            base.TickRare();
            _innerContainer.ThingOwnerTickRare();

            Tick(GenTicks.TickRareInterval);
        }

        public override void TickLong()
        {
            base.TickLong();
            _innerContainer.ThingOwnerTickLong();

            Tick(GenTicks.TickLongInterval);
        }

        public void Tick(int ticks)
        {

        }

        public virtual void EjectContents()
        {
            _innerContainer.TryDropAll(InteractionCell, base.Map, ThingPlaceMode.Near);
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return _innerContainer;
        }

        public void Notify_HauledTo(Pawn hauler, Thing thing, int count)
        {
        }
    }
}
