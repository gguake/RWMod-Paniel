using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ModuleAutomata
{
    public class AutomataAssembleBill : IExposable
    {
        public Pawn targetPawn;

        public Dictionary<AutomataModulePart, ThingDef> modules = new Dictionary<AutomataModulePart, ThingDef>();
        public List<ThingDef> customModules = new List<ThingDef>();

        public HairDef hair;
        public HeadTypeDef head;

        public AutomataAssembleBill()
        {
        }

        public AutomataAssembleBill(Pawn sourcePawn)
        {
            targetPawn = sourcePawn;

        }

        public void ExposeData()
        {
            Scribe_References.Look(ref targetPawn, "targetPawn");

            Scribe_Collections.Look(ref modules, "modules", LookMode.Value, LookMode.Def);
            Scribe_Collections.Look(ref customModules, "customModules", LookMode.Def);

            Scribe_Defs.Look(ref hair, "hair");
            Scribe_Defs.Look(ref head, "head");
        }
    }
}
