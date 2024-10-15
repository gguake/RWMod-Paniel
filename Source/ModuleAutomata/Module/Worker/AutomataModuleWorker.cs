using Verse;

namespace ModuleAutomata
{
    public abstract class AutomataModuleWorker
    {
        public abstract void OnApplyPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleSpec spec);
    }
}
