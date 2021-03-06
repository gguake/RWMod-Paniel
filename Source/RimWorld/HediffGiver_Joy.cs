using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AutomataRace
{
    public class HediffGiver_Joy : HediffGiver
    {
        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            var needJoy = pawn.needs.joy;
            if (needJoy == null || needJoy.CurLevel < float.Epsilon)
            {
                return;
            }

            var hediff = pawn.health.hediffSet.GetFirstHediffOfDef(this.hediff);
            if (hediff == null)
            {
                hediff = pawn.health.AddHediff(this.hediff);
            }
            
            hediff.Severity = needJoy.CurLevel;

        }
    }
}
