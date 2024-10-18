using RimWorld;
using Verse;

namespace ModuleAutomata
{
    public abstract class AutomataModuleSpec : IExposable
    {
        public AutomataModuleDef moduleDef;

        public abstract string Label { get; }
        public abstract QualityCategory Quality { get; }
        public abstract ThingDef Stuff { get; }

        public virtual void ExposeData()
        {
            Scribe_Defs.Look(ref moduleDef, "moduleDef");
        }
    }
}
