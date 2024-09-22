using System.Collections.Generic;
using Verse;

namespace ModuleAutomata
{
    public enum AutomataModulePart
    {
        None,
        Core,
        MainFrame,
        LeftArm,
        RightArm,
        LeftLeg,
        RightLeg,
        Shell,
        Custom,
    }

    public class AutomataModuleExtension : DefModExtension
    {
        public List<AutomataModulePart> targetParts;
        public bool isDefault;
        public HediffDef hediff;
    }
}
