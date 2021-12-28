using RimWorld;

namespace AutomataRace
{
    public class CompProperties_ExplosiveInstant : CompProperties_Explosive
    {
        public float awfulExplosiveMultiplier = 1.0f;
        public float poorExplosiveMultiplier = 1.0f;

        public CompProperties_ExplosiveInstant()
        {
            compClass = typeof(CompExplosiveInstant);
        }
    }
}
