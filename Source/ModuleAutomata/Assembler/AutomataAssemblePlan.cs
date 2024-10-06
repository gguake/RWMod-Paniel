using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleAutomata
{
    public enum AutomataModuleAssemblePlanState
    {
        // 새로 장착하는 모듈
        NewModule,

        // 이미 장착되어있는 모듈
        AlreadyEmbedded,
    }

    public struct AutomataModuleAssemblePlan
    {
        public AutomataModuleAssemblePlanState state;
        public AutomataModuleSpec spec;
    }

    public class AutomataAssemblePlan
    {
        public Dictionary<AutomataModulePartDef, AutomataModuleAssemblePlan> modulePlans;
    }
}
