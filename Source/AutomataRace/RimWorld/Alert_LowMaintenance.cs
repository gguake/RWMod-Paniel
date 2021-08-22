using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AutomataRace
{
    public class Alert_LowMaintenance : Alert
    {
        public IEnumerable<Pawn> LowMaintenancePawns => PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists_NoCryptosleep.Where(pawn => {
            var hediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(AutomataRaceDefOf.PN_Maintenance);
            return hediff?.CurStageIndex <= 1;
        });

        public IEnumerable<Pawn> UrgentMaintenancePawns => PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists_NoCryptosleep.Where(pawn => {
            var hediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(AutomataRaceDefOf.PN_Maintenance);
            return hediff?.CurStageIndex == 0;
        });

        public IEnumerable<Pawn> RequiredMaintenancePawns => PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonists_NoCryptosleep.Where(pawn => {
            var hediff = pawn.health?.hediffSet?.GetFirstHediffOfDef(AutomataRaceDefOf.PN_Maintenance);
            return hediff?.CurStageIndex == 1;
        });

        public bool IsCritical => UrgentMaintenancePawns.Any();

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

        public Alert_LowMaintenance()
        {
            this.defaultLabel = "PN_LowMaintenance".Translate();
            this.defaultPriority = AlertPriority.High;
        }

        public override AlertPriority Priority => UrgentMaintenancePawns.Any() ? AlertPriority.Critical : AlertPriority.High;

        public override TaggedString GetExplanation()
        {
            StringBuilder pawnsString = new StringBuilder();
            foreach (var pawn in LowMaintenancePawns)
            {
                pawnsString.AppendLine($"  - {pawn.NameShortColored.Resolve()}");
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("PN_LowMaintenanceDesc".Translate(pawnsString).Resolve());

            return sb.ToString();
        }

        public override AlertReport GetReport()
        {
            if (!LowMaintenancePawns.Any())
            {
                return false;
            }

            AlertReport report = default(AlertReport);
            report.culpritsPawns = LowMaintenancePawns.ToList();
            report.active = report.AnyCulpritValid;
            return report;
        }

    }
}
