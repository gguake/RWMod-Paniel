//using AlienRace;
//using System;
//using System.Collections.Generic;
//using Verse;

//namespace AutomataRace.Extensions
//{
//    public static class AlienRaceExtension
//    {
//        public static int GetBodyAddonCount(this Pawn pawn)
//        {
//            var alienThingDef = pawn.def as ThingDef_AlienRace;
//            if (alienThingDef == null)
//            {
//                return -1;
//            }

//            var bodyAddons = alienThingDef?.alienRace?.generalSettings?.alienPartGenerator?.bodyAddons;
//            return bodyAddons.Count;
//        }

//        public static IList<int> GetBodyAddonVariant(this Pawn pawn)
//        {
//            var alienThingDef = pawn.def as ThingDef_AlienRace;
//            if (alienThingDef == null)
//            {
//                Log.Error($"Tried to alienrace extension for non-alien pawn.");
//                return null;
//            }

//            var alienComp = pawn.TryGetComp<AlienPartGenerator.AlienComp>();
//            if (alienComp == null)
//            {
//                Log.Error($"Tried to alienrace extension for non-alien pawn.");
//                return null;
//            }


//            return alienComp.addonVariants;
//        }

//        public static void SetBodyAddonVariant(this Pawn pawn, List<int> variant)
//        {
//            if (variant == null)
//            {
//                return;
//            }

//            var alienComp = pawn.TryGetComp<AlienPartGenerator.AlienComp>();
//            if (alienComp == null)
//            {
//                Log.Error($"Tried to alienrace extension for non-alien pawn.");
//                return;
//            }

//            alienComp.addonVariants = new List<int>(variant);
//        }

//        public static IList<int> SetBodyAddonRandomly(this Pawn pawn)
//        {
//            var alienThingDef = pawn.def as ThingDef_AlienRace;
//            if (alienThingDef == null)
//            {
//                Log.Error($"Tried to alienrace extension for non-alien pawn.");
//                return null;
//            }

//            var alienComp = pawn.TryGetComp<AlienPartGenerator.AlienComp>();
//            if (alienComp == null)
//            {
//                Log.Error($"Tried to alienrace extension for non-alien pawn.");
//                return null;
//            }

//            var bodyAddons = alienThingDef?.alienRace?.generalSettings?.alienPartGenerator?.bodyAddons;
//            int bodyAddonCount = bodyAddons.Count;
//            for (int i = 0; i < bodyAddonCount; ++i)
//            {
//                int index = Math.Abs(Rand.Int % bodyAddons[i].variantCount);
//                alienComp.addonVariants[i] = index;
//            }

//            return alienComp.addonVariants;
//        }
//    }
//}
