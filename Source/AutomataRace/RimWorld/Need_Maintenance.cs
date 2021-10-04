using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AutomataRace
{
    public class Need_Maintenance : Need
    {
        private float FallPerTick => def.fallPerDay / 60000f;

        public Need_Maintenance(Pawn pawn) : 
            base(pawn)
        {
            this.threshPercents = new List<float>();
            this.threshPercents.Add(0.1f);
            this.threshPercents.Add(0.2f);
            this.threshPercents.Add(0.8f);
        }

        public override void NeedInterval()
        {
            if (!IsFrozen)
            {
                CurLevel -= FallPerTick * 150f;
            }
        }

        public override void SetInitialLevel()
        {
            CurLevel = 1f;
        }
    }
}
