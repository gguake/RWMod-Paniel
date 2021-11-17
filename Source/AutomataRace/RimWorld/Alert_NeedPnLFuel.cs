using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AutomataRace
{
    public class Alert_NeedFuel : Alert
    {
        public IEnumerable<Pawn> LowFuelPawns => PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists_NoCryptosleep.Where(pawn => {
            var hediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(AutomataRaceDefOf.PN_AutomatonFuel_Addiction);
            return hediff?.CurStageIndex == 1;
        });

        public IEnumerable<Pawn> NeedFuelPawns => PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists_NoCryptosleep.Where(pawn => {
            var hediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(AutomataRaceDefOf.PN_AutomatonFuel_Addiction);
            return hediff?.CurStageIndex == 1;
        });

        public bool IsCritical => NeedFuelPawns.Any();

        protected override Color BGColor
        {
            get
            {
                if (IsCritical)
                {
                    float num = Pulser.PulseBrightness(0.5f, Pulser.PulseBrightness(0.5f, 0.6f));
                    return new Color(num, num, num) * Color.red;
                }
                else
                {
                    return base.BGColor;
                }
            }
        }

        public Alert_NeedFuel()
        {
            this.defaultLabel = "PN_NeedFuel".Translate();
            this.defaultPriority = AlertPriority.High;
        }

        public override AlertPriority Priority => NeedFuelPawns.Any() ? AlertPriority.Critical : AlertPriority.High;

        public override TaggedString GetExplanation()
        {
            StringBuilder pawnsString = new StringBuilder();
            foreach (var pawn in LowFuelPawns)
            {
                pawnsString.AppendLine($"  - {pawn.NameShortColored.Resolve()}");
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("PN_NeedFuelDesc".Translate(pawnsString).Resolve());

            return sb.ToString();
        }

        public override AlertReport GetReport()
        {
            if (!LowFuelPawns.Any())
            {
                return false;
            }

            AlertReport report = default(AlertReport);
            report.culpritsPawns = LowFuelPawns.ToList();
            report.active = report.AnyCulpritValid;
            return report;
        }

    }
}
