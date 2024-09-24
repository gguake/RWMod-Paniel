using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ModuleAutomata
{
    public class AutomataModulePartDef : Def
    {
        private static List<AutomataModulePartDef> _allModulePartDefOrdered;
        public static List<AutomataModulePartDef> AllModulePartDefOrdered
        {
            get
            {
                if (_allModulePartDefOrdered == null)
                {
                    _allModulePartDefOrdered = DefDatabase<AutomataModulePartDef>.AllDefsListForReading
                        .OrderBy(v => v.uiOrder)
                        .ToList();
                }

                return _allModulePartDefOrdered;
            }
        }

        public int uiOrder = 0;
        public Color uiColor = Color.white;

        public bool required;
    }
}
