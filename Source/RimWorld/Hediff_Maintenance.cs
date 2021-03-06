using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;

namespace AutomataRace
{
    public class Hediff_Maintenance : HediffWithComps
    {
        private Need_Maintenance _need = null;

        public Need_Maintenance Need
        {
            get
            {
                if (pawn.Dead)
                {
                    return null;
                }

                if (_need == null)
                {
                    List<Need> allNeeds = pawn.needs.AllNeeds;
                    for (int i = 0; i < allNeeds.Count; i++)
                    {
                        if (allNeeds[i].def == def.causesNeed)
                        {
                            _need = (Need_Maintenance)allNeeds[i];
                            break;
                        }
                    }
                }

                return _need;
            }
        }
        
        public override int CurStageIndex
        {
            get
            {
                Need_Maintenance need = Need;
                if (need != null)
                {
                    int stage = 0;
                    float currentMinSeverity = float.MaxValue;

                    for (int i = 0; i < def.stages.Count; ++i)
                    {
                        if (def.stages[i].minSeverity < currentMinSeverity && need.CurLevel <= def.stages[i].minSeverity)
                        {
                            currentMinSeverity = def.stages[i].minSeverity;
                            stage = i;
                        }
                    }

                    return stage;
                }

                return def.stages.Count - 1;
            }
        }

        public override float Severity
        {
            get
            {
                return 1f;
            }
            set
            {
            }
        }

        public override string SeverityLabel => null;
    }
}
