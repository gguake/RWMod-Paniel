using RimWorld;
using Verse;

namespace AutomataRace
{
    public class CompExplosiveInstant : CompExplosive
    {
        public CompProperties_ExplosiveInstant PropsExplosiveInstant => Props as CompProperties_ExplosiveInstant;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            StartWick();
        }

        public override void CompTickRare()
        {
            base.CompTickRare();

            if (!parent.Spawned)
            {
                return;
            }

            if (wickStarted)
            {
                wickTicksLeft -= 250;
                if (wickTicksLeft <= 0)
                {
                    var corpse = parent as Corpse;
                    var pawn = corpse?.InnerPawn;

                    if (pawn != null)
                    {
                        if (pawn.kindDef == AutomataRaceDefOf.Paniel_Randombox_Awful)
                        {
                            customExplosiveRadius = Props.explosiveRadius * PropsExplosiveInstant.awfulExplosiveMultiplier;
                        }
                        else if (pawn.kindDef == AutomataRaceDefOf.Paniel_Randombox_Poor)
                        {
                            customExplosiveRadius = Props.explosiveRadius * PropsExplosiveInstant.poorExplosiveMultiplier;
                        }
                    }

                    Detonate(parent.MapHeld);
                }
            }
        }
    }
}
