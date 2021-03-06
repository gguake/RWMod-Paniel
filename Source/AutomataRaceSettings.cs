using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AutomataRace
{
    public class DeadThoughtOverride
    {
        public ThoughtDef source = null;
        public ThoughtDef overwrite = null;
    }

    public class AutomataRaceSettings : Def
    {
        public List<NeedDef> needBlacklists = new List<NeedDef>();

        public bool socialActivated = true;
        public bool skillDecayActivated = true;
        public bool infectionActivated = true;
        public bool medicineTendable = true;

        public List<DeadThoughtOverride> deadThoughtOverrides = new List<DeadThoughtOverride>();
    }
}
