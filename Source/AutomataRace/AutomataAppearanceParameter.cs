using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AutomataRace
{
    public class AutomataAppearanceParameter : IExposable
    {
        public HairDef hairDef;
        public string headGraphicPath;
        public List<int> bodyAddonVariant;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref hairDef, "hairDef");
            Scribe_Values.Look(ref headGraphicPath, "headGraphicPath");
            Scribe_Collections.Look(ref bodyAddonVariant, "bodyAddonVariant");
        }
    }

}
