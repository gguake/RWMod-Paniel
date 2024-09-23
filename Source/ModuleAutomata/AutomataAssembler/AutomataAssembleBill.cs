using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ModuleAutomata
{
    public struct AutomataModuleSelectOption : IExposable
    {
        public List<AutomataModuleDef> modules;
        public QualityCategory? quality;

        public void ExposeData()
        {
            Scribe_Collections.Look(ref modules, "modules", LookMode.Def);
            Scribe_Values.Look(ref quality, "quality");
        }
    }

    public class AutomataAssembleBill : IExposable
    {
        public Pawn targetPawn;

        public Dictionary<AutomataModulePartDef, AutomataModuleSelectOption> modules;

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

            Scribe_Collections.Look(ref modules, "modules", LookMode.Def, LookMode.Deep);
        }
    }
}
