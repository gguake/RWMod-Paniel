using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace ModuleAutomata
{
    public enum AutomataModuleOptionGroupRule
    {
        // Thing마다 개별로 전부 표시
        NoGroup,

        // 같은 퀄리티끼리 하나로 표시
        Quality,

        // 퀄리티에 상관없이 모든 아이템을 하나로 표시
        All,
    }

    public class AutomataModuleDef : Def
    {
        public List<AutomataModulePartDef> adaptableParts;

        public ThingDef recipeThingDef;
        public AutomataModuleOptionGroupRule recipeGroupRule;
        public List<ThingDefCountClass> fixedRecipeIngredients;

        public List<AutomataModuleWorker> workers;
    }


}
