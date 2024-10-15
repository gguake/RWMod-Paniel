using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ModuleAutomata
{
    public class AutomataModuleWorker_StatChange : AutomataModuleWorker
    {
        public List<QualityMultiplier> qualityMultiplier;
        public List<StatModifier> statOffsets;
        public List<StatModifier> statFactors;

        public override void OnApplyPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleSpec spec)
        {
        }
    }
}
