using RimWorld;
using System;
using Verse;

namespace ModuleAutomata
{
    public class AutomataModuleWorker_Core : AutomataModuleWorker
    {
        public override void OnInstallToPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleSpec spec)
        {
            if (!(spec is AutomataModuleSpec_Core specCore))
            {
                throw new NotImplementedException();
            }

            var originalCompCore = specCore.thing.TryGetComp<CompAutomataCore>();
            if (originalCompCore == null) { throw new NotImplementedException(); }

            var coreInfo = originalCompCore.CoreInfo;

            var compCore = pawn.GetComp<CompAutomataCore>();
            if (compCore == null) { throw new NotImplementedException(); }

            compCore.InitializeFrom(coreInfo);

            foreach (var skillDef in DefDatabase<SkillDef>.AllDefsListForReading)
            {
                var skill = pawn.skills.GetSkill(skillDef);
                if (coreInfo.sourceSkill.TryGetValue(skillDef, out var skillLevel))
                {
                    skill.Level = skillLevel;
                }
            }

            pawn.story.Childhood = coreInfo.coreModuleDef.GetModExtension<AutomataCoreModExtension>().childhoodBackstory;
            pawn.story.Adulthood = coreInfo.coreModuleDef.GetModExtension<AutomataCoreModExtension>().adulthoodBackstory;
        }

        public override void OnUninstallFromPawn(Pawn pawn, AutomataModulePartDef partDef, AutomataModuleSpec spec)
        {
            throw new NotImplementedException();
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
