//using RimWorld;
//using Verse;

//namespace AutomataRace
//{
//    public class HediffCompProperties_WithAbility : HediffCompProperties
//    {
//        public AbilityDef abilityDef;
        
//        public HediffCompProperties_WithAbility()
//        {
//            compClass = typeof(HediffComp_WithAbility);
//        }
//    }

//    public class HediffComp_WithAbility : HediffComp
//    {
//        private HediffCompProperties_WithAbility Props => (HediffCompProperties_WithAbility)props;

//        public override void CompPostPostAdd(DamageInfo? dinfo)
//        {
//            Log.Message("post added");
//            parent.pawn.abilities.GainAbility(Props.abilityDef);
//        }

//        public override void CompPostPostRemoved()
//        {
//            Log.Message("post removed");
//            parent.pawn.abilities.RemoveAbility(Props.abilityDef);
//        }
//    }
//}
