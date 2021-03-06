using RimWorld;
using Verse;

namespace AutomataRace
{
    public class Need_Maintenance : Need
    {
        private float FallPerTick => def.fallPerDay / 60000f;

        public Need_Maintenance(Pawn pawn) : 
            base(pawn)
        {
        }

        public override void NeedInterval()
        {
            if (!IsFrozen)
            {
                CurLevel -= FallPerTick * 150f;
            }
        }
    }
}
