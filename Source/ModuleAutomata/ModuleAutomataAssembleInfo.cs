using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ModuleAutomata
{
    public class ModuleAutomataAssembleInfo : IExposable
    {
        public Pawn targetPawn;
        public bool isInvoice;

        public Dictionary<AutomataModulePart, ThingDef> modules = new Dictionary<AutomataModulePart, ThingDef>();
        public List<ThingDef> customModules = new List<ThingDef>();

        public HairDef hair;
        public HeadTypeDef head;

        public void ExposeData()
        {
            Scribe_References.Look(ref targetPawn, "targetPawn");
            Scribe_Values.Look(ref isInvoice, "isInvoice");

            Scribe_Collections.Look(ref modules, "modules", LookMode.Value, LookMode.Def);
            Scribe_Collections.Look(ref customModules, "customModules", LookMode.Def);

            Scribe_Defs.Look(ref hair, "hair");
            Scribe_Defs.Look(ref head, "head");
        }
    }
}
