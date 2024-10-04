using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ModuleAutomata
{
    public class AutomataModulePartDef : Def
    {
        private HashSet<AutomataModuleDef> _moduleDefCache;
        public HashSet<AutomataModuleDef> ModuleDefs
        {
            get
            {
                if (_moduleDefCache == null)
                {
                    _moduleDefCache = DefDatabase<AutomataModuleDef>.AllDefsListForReading
                        .Where(def => def.adaptParts.Contains(this))
                        .ToHashSet();
                }

                return _moduleDefCache;
            }
        }

        public bool required;
        public string targetBodyCustomLabel;

        public BodyPartRecord FindBodyPartRecordFromPawn(Pawn pawn)
        {
            if (targetBodyCustomLabel == null) { return null; }

            return pawn.RaceProps.body.AllParts.FirstOrDefault(v => v.customLabel == targetBodyCustomLabel);
        }
    }
}
