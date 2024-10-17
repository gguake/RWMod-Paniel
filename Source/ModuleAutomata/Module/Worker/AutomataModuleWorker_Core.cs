using Verse;

namespace ModuleAutomata
{
    public class AutomataModuleWorker_Core : AutomataModuleWorker
    {
        public override void OnInstallToPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleSpec spec)
        {
        }

        public override void OnUninstallFromPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleSpec spec)
        {
        }

        public override AutomataModuleSpec TryGetModuleSpecFromPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleDef moduleDef)
        {
            var compCore = pawn.GetComp<CompAutomataCore>();
            if (compCore == null) { return null; }

            return new AutomataModuleSpec_Core()
            {
                moduleDef = moduleDef,
                thing = pawn,
            };
        }
    }
}
