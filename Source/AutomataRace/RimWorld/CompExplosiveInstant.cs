using RimWorld;

namespace AutomataRace
{
    public class CompExplosiveInstant : CompExplosive
    {
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
                    Detonate(parent.MapHeld);
                }
            }
        }
    }
}
