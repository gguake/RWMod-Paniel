using Verse;
using RimWorld;

namespace AutomataRace
{
    public class CompSelfResurrect : ThingComp
    {
        private CompProperties_SelfResurrect Props => (CompProperties_SelfResurrect)props;

        private Corpse Corpse => parent as Corpse;
        private Pawn InnerPawn => Corpse?.InnerPawn;

        private bool _activated = false;
        private int _progress = 1000;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            
            if (InnerPawn == null)
            {
                return;
            }

            if (Props.hediffCondition != null)
            {
                if (InnerPawn.health?.hediffSet?.GetFirstHediffOfDef(Props.hediffCondition) == null)
                {
                    return;
                }
            }

            _progress = Props.resurrectDelayTick;
            _activated = true;
        }

        public override void CompTickRare()
        {
            base.CompTickRare();

            if (!_activated || !parent.Spawned)
            {
                return;
            }

            _progress -= 250;
            if (_progress < 0 && InnerPawn != null)
            {
                if (Props.hediffCondition != null && Props.removeHediffAfterResurrect)
                {
                    var hediff = InnerPawn.health?.hediffSet?.GetFirstHediffOfDef(Props.hediffCondition);
                    InnerPawn.health.RemoveHediff(hediff);
                }

                ResurrectionUtility.ResurrectWithSideEffects(InnerPawn);
                _activated = false;
            }
        }
    }
}
