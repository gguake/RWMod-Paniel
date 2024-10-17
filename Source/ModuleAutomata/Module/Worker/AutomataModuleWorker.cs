using System.Collections.Generic;
using Verse;

namespace ModuleAutomata
{
    public abstract class AutomataModuleWorker
    {
        public abstract AutomataModuleSpec TryGetModuleSpecFromPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleDef moduleDef);

        public abstract void OnInstallToPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleSpec spec);

        public abstract void OnUninstallFromPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleSpec spec);
    }

    public abstract class AutomataModuleWorkerWithHediff : AutomataModuleWorker
    {
        public List<QualityHediff> hediffs;

    }
}
