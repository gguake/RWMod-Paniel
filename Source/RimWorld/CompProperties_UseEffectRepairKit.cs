using System.Collections.Generic;
using Verse;
using RimWorld;

namespace AutomataRace
{
    public class CompProperties_UseEffectRepairKit : CompProperties_UseEffect
    {
        public List<ThingDef> thingDefs = new List<ThingDef>();
        public bool blockUnnecessaryUse = false;

        public CompProperties_UseEffectRepairKit()
        {
            compClass = typeof(CompUseEffect_RepairKit);
        }
    }
}
