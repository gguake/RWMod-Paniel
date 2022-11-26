using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AutomataRace
{
    public class AutomataAppearanceParameter : IExposable
    {
        public HairDef hairDef;
        public HeadTypeDef headTypeDef;
        public List<int> bodyAddonVariant;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref hairDef, "hairDef");
            Scribe_Defs.Look(ref headTypeDef, "headTypeDef");
            Scribe_Collections.Look(ref bodyAddonVariant, "bodyAddonVariant");
        }
    }

}
