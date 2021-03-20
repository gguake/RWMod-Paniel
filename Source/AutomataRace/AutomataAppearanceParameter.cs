using RimWorld;
using Verse;

namespace AutomataRace
{
    public class AutomataAppearanceParameter : IExposable
    {
        public HairDef hairDef;
        public int faceVariantIndex;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref hairDef, "hairDef");
            Scribe_Values.Look(ref faceVariantIndex, "faceVariantIndex");
        }
    }

}
