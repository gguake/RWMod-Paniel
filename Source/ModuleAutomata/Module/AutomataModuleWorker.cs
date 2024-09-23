using RimWorld;
using System;
using Verse;

namespace ModuleAutomata
{
    public abstract class AutomataModuleWorker
    {
        public abstract void OnApplyPawn(Pawn pawn, QualityCategory sourceQuality);
    }

    public class AutomataModuleWorker_Core : AutomataModuleWorker
    {
        public override void OnApplyPawn(Pawn pawn, QualityCategory sourceQuality)
        {
            throw new NotImplementedException();
        }
    }

    public class AutomataModuleWorker_ImplantHediff : AutomataModuleWorker
    {
        public Hediff hediff;

        public override void OnApplyPawn(Pawn pawn, QualityCategory sourceQuality)
        {
            throw new NotImplementedException();
        }
    }

    public class AutomataModuleWorker_BodyPartHediff : AutomataModuleWorker
    {
        public Hediff hediff;

        public override void OnApplyPawn(Pawn pawn, QualityCategory sourceQuality)
        {
            throw new NotImplementedException();
        }
    }

    public class AutomataModuleWorker_Appearance : AutomataModuleWorker
    {
        public HairDef hairDef;
        public HeadTypeDef headTypeDef;

        public override void OnApplyPawn(Pawn pawn, QualityCategory sourceQuality)
        {
            throw new NotImplementedException();
        }
    }

}
