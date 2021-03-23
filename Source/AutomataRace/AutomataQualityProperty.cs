using AlienRace;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AutomataRace
{
    public class AutomataQualityProperty : Def
    {
        public QualityCategory quality;
        public Dictionary<AutomataSpecializationDef, PawnKindDef> pawnKindDefs;
    }
}
