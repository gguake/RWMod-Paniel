using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ModuleAutomata
{
    public class Building_AutomataAssembler : Building, IThingHolder, INotifyHauledTo
    {
        private ThingOwner _innerContainer;

        public AutomataAssembleUIExtension AutomataAssembleUIExtension => def.GetModExtension<AutomataAssembleUIExtension>();

        public Building_AutomataAssembler()
        {
            _innerContainer = new ThingOwner<Thing>(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Deep.Look(ref _innerContainer, "innerContainer", this);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (Spawned && Faction == Faction.OfPlayer)
            {
                if (true)
                {
                    var commandAssembleNew = new Command_Action();
                    commandAssembleNew.defaultLabel = PNLocale.PN_CommandAssembleNewLabel.Translate();
                    commandAssembleNew.defaultDesc = PNLocale.PN_CommandAssembleNewDesc.Translate();
                    commandAssembleNew.action = () =>
                    {
                        Find.WindowStack.Add(new Dialog_AutomataAssemble(this, (bill) =>
                        {

                        }));
                    };
                    yield return commandAssembleNew;

                    var commandReassemble = new Command_Action();
                    commandReassemble.defaultLabel = PNLocale.PN_CommandReassembleLabel.Translate();
                    commandReassemble.defaultDesc = PNLocale.PN_CommandReassembleDesc.Translate();
                    commandReassemble.action = () =>
                    {
                        var candidates = Map.mapPawns.FreeColonistsAndPrisonersSpawned.Where(p => p.IsAutomata());

                        Find.WindowStack.Add(new FloatMenu(candidates.Select(pawn => new FloatMenuOption(
                            pawn.Name.ToStringShort,
                            () =>
                            {
                                Find.WindowStack.Add(new Dialog_AutomataAssemble(this, pawn, (bill) =>
                                {

                                }));
                            },
                            pawn,
                            Color.white)).ToList()));
                    };
                    yield return commandReassemble;
                }
                else
                {
                    var commandCancel = new Command_Action();
                    commandCancel.defaultLabel = PNLocale.PN_CommandCancelAssembleLabel.Translate();
                    commandCancel.defaultDesc = PNLocale.PN_CommandCancelAssembleDesc.Translate();
                    commandCancel.action = () =>
                    {
                        EjectContents();
                    };
                    yield return commandCancel;
                }
            }
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            if (selPawn.IsAutomata())
            {
                yield return new FloatMenuOption(PNLocale.PN_FloatMenuReassembleLabel.Translate(), () =>
                {
                    Find.WindowStack.Add(new Dialog_AutomataAssemble(this, selPawn, (bill) =>
                    {

                    }));
                });
            }
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
