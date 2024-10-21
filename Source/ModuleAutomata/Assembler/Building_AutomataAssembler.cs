using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace ModuleAutomata
{
    public class Building_AutomataAssembler : Building, IThingHolder, INotifyHauledTo
    {
        private ThingOwner _innerContainer;

        private AutomataAssembleBill _bill;

        public AutomataAssembleUIModExtension AutomataAssembleUIExtension => def.GetModExtension<AutomataAssembleUIModExtension>();

        public IEnumerable<(AutomataModuleIngredientInfo info, int count)> RequiredIngredients
        {
            get
            {
                if (_bill == null) { yield break; }

                foreach (var tuple in _bill.plan.TotalIngredients)
                {
                    var ingredientInfo = tuple.info;
                    var requiredCount = tuple.count;

                    var innerThing = _innerContainer.FirstOrDefault(thing => ingredientInfo.Match(thing));
                    if (innerThing != null)
                    {
                        requiredCount -= innerThing.stackCount;
                    }

                    if (requiredCount > 0)
                    {
                        yield return (ingredientInfo, requiredCount);
                    }
                }
            }
        }

        public Building_AutomataAssembler()
        {
            _innerContainer = new ThingOwner<Thing>(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Deep.Look(ref _innerContainer, "innerContainer", this);
            Scribe_Deep.Look(ref _bill, "bill", this);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (Spawned && Faction == Faction.OfPlayer)
            {
                if (_bill == null)
                {
                    var commandAssembleNew = new Command_Action();
                    commandAssembleNew.defaultLabel = PNLocale.PN_CommandAssembleNewLabel.Translate();
                    commandAssembleNew.defaultDesc = PNLocale.PN_CommandAssembleNewDesc.Translate();
                    commandAssembleNew.action = () =>
                    {
                        OpenAssembleDialog(null);
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
                                OpenAssembleDialog(pawn);
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

                        _bill = null;
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
                        OpenAssembleDialog(selPawn);
                    }));
                });
            }
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            EjectContents();
            base.DeSpawn(mode);
        }

        public override string GetInspectString()
        {
            var sb = new StringBuilder(base.GetInspectString());

            if (_bill != null && !_bill.IsStarted)
            {
                foreach (var tuple in RequiredIngredients)
                {
                    var totalCount = _bill.plan.GetIngredientCount(tuple.info);
                    sb.AppendInNewLine($"{tuple.info.Label}: {tuple.count} / {totalCount}");
                }
            }

            return sb.ToString();
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
            if (_bill == null)
            {
                _innerContainer.TryDropAll(Position, Map, ThingPlaceMode.Near);
                return;
            }
        }

        private void OpenAssembleDialog(Pawn targetPawn)
        {
            if (targetPawn == null)
            {
                Find.WindowStack.Add(new Dialog_AutomataAssemble(this, (plan) =>
                {
                    _bill = new AutomataAssembleBill(this)
                    {
                        plan = plan,
                    };
                }));
            }
            else
            {
                Find.WindowStack.Add(new Dialog_AutomataAssemble(this, targetPawn, (plan) =>
                {
                    _bill = new AutomataAssembleBill(this)
                    {
                        pawn = targetPawn,
                        plan = plan,
                    };
                }));
            }
        }
    }
}
