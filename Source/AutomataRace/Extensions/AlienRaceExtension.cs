using AlienRace;
using System;
using System.Collections.Generic;
using Verse;

namespace AutomataRace.Extensions
{
    public static class AlienRaceExtension
    {
        public static int FindFaceBodyAddonIndex(this Pawn pawn)
        {
            var alienThingDef = pawn.def as ThingDef_AlienRace;
            if (alienThingDef == null)
            {
                return -1;
            }

            var bodyAddons = alienThingDef?.alienRace?.generalSettings?.alienPartGenerator?.bodyAddons;
            for (int i = 0; i < bodyAddons?.Count; ++i)
            {
                if (bodyAddons[i].bodyPart == "Head")
                {
                    return i;
                }
            }

            return -1;
        }

        public static int GetFaceBodyAddonVariantCount(this Pawn pawn)
        {
            var alienThingDef = pawn.def as ThingDef_AlienRace;
            if (alienThingDef == null)
            {
                Log.Error($"Tried to alienrace extension for non-alien pawn.");
                return -1;
            }

            int faceBodyAddonIndex = pawn.FindFaceBodyAddonIndex();
            if (faceBodyAddonIndex < 0)
            {
                Log.Error($"Failed to find face body addon.");
                return -1;
            }

            var bodyAddons = alienThingDef?.alienRace?.generalSettings?.alienPartGenerator?.bodyAddons;
            return bodyAddons[faceBodyAddonIndex].variantCount;
        }

        public static int GetFaceBodyAddonVariant(this Pawn pawn)
        {
            var alienComp = pawn.TryGetComp<AlienPartGenerator.AlienComp>();
            if (alienComp == null)
            {
                Log.Error($"Tried to alienrace extension for non-alien pawn.");
                return -1;
            }

            int faceBodyAddonIndex = FindFaceBodyAddonIndex(pawn);
            if (faceBodyAddonIndex < 0)
            {
                Log.Error($"Failed to find face body addon.");
                return -1;
            }

            return alienComp.addonVariants[faceBodyAddonIndex];
        }

        public static void SetFaceBodyAddonVariant(this Pawn pawn, int index)
        {
            var alienComp = pawn.TryGetComp<AlienPartGenerator.AlienComp>();
            if (alienComp == null)
            {
                Log.Error($"Tried to alienrace extension for non-alien pawn.");
                return;
            }

            int faceBodyAddonIndex = pawn.FindFaceBodyAddonIndex();
            if (faceBodyAddonIndex < 0)
            {
                Log.Error($"Failed to find face body addon.");
                return;
            }

            if (alienComp.addonVariants == null)
            {
                alienComp.addonVariants = new List<int>();
                for (int i = 0; i < faceBodyAddonIndex + 1; ++i)
                {
                    alienComp.addonVariants.Add(0);
                }
            }
            alienComp.addonVariants[faceBodyAddonIndex] = index;
        }

        public static int SetFaceBodyAddonRandomly(this Pawn pawn)
        {
            var alienThingDef = pawn.def as ThingDef_AlienRace;
            if (alienThingDef == null)
            {
                Log.Error($"Tried to alienrace extension for non-alien pawn.");
                return -1;
            }

            var alienComp = pawn.TryGetComp<AlienPartGenerator.AlienComp>();
            if (alienComp == null)
            {
                Log.Error($"Tried to alienrace extension for non-alien pawn.");
                return -1;
            }

            int faceBodyAddonIndex = FindFaceBodyAddonIndex(pawn);
            if (faceBodyAddonIndex < 0)
            {
                Log.Error($"Failed to find face body addon.");
                return -1;
            }

            var bodyAddons = alienThingDef?.alienRace?.generalSettings?.alienPartGenerator?.bodyAddons;

            int index = Math.Abs(Rand.Int % bodyAddons[faceBodyAddonIndex].variantCount);
            alienComp.addonVariants[faceBodyAddonIndex] = index;

            return index;
        }
    }
}
