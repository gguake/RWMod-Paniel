using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ModuleAutomata
{
    public static class HARExtension
    {
        public static int GetBodyAddonIndex(this Pawn pawn, int addonIndex)
        {
            if (pawn == null) { return -1; }

            var alienComp = pawn.AllComps.FirstOrDefault(comp => comp.GetType().Name == "AlienComp");
            if (alienComp == null) { return -1; }

            var fieldInfo = alienComp.GetType().GetField("addonVariants");

            var variants = (List<int>)fieldInfo.GetValue(alienComp);
            if (variants == null || addonIndex >= variants.Count) { return -1; }

            return variants[addonIndex];
        }

        public static void SetBodyAddonIndex(this Pawn pawn, int addonIndex, int variantIndex)
        {
            if (pawn == null) { return; }

            var alienComp = pawn.AllComps.FirstOrDefault(comp => comp.GetType().Name == "AlienComp");
            if (alienComp == null) { return; }

            var fieldInfo = alienComp.GetType().GetField("addonVariants");

            var variants = (List<int>)fieldInfo.GetValue(alienComp);
            if (variants == null)
            {
                variants = new List<int>();
                fieldInfo.SetValue(alienComp, variants);
            }

            while (addonIndex >= variants.Count)
            {
                variants.Add(0);
            }

            variants[addonIndex] = variantIndex;
        }

        private static Dictionary<(ThingDef, int), int> _cachedBodyAddonCounts = new Dictionary<(ThingDef, int), int>();
        public static int GetBodyAddonVariantCount(this ThingDef pawnThingDef, int addonIndex)
        {
            if (pawnThingDef == null) { return 0; }

            if (_cachedBodyAddonCounts.TryGetValue((pawnThingDef, addonIndex), out var cached))
            {
                return cached;
            }
            else
            {
                var alienRaceFieldInfo = pawnThingDef.GetType().GetField("alienRace");
                var alienRace = alienRaceFieldInfo.GetValue(pawnThingDef);

                var generalSettingsFieldInfo = alienRace.GetType().GetField("generalSettings");
                var generalSettings = generalSettingsFieldInfo.GetValue(alienRace);

                var alienPartGeneratorFieldInfo = generalSettings.GetType().GetField("alienPartGenerator");
                var alienPartGenerator = alienPartGeneratorFieldInfo.GetValue(generalSettings);

                var bodyAddonsFieldInfo = alienPartGenerator.GetType().GetField("bodyAddons");
                var bodyAddons = bodyAddonsFieldInfo.GetValue(alienPartGenerator);

                var indexOfMethodInfo = bodyAddons.GetType().GetMethod("get_Item");
                var bodyAddon = indexOfMethodInfo.Invoke(bodyAddons, new object[] { addonIndex });

                var variantCountMaxPropertyGetter = bodyAddon.GetType().GetProperty("VariantCountMax").GetGetMethod();
                var variantCountMax = (int)variantCountMaxPropertyGetter.Invoke(bodyAddon, new object[] { });

                _cachedBodyAddonCounts.Add((pawnThingDef, addonIndex), variantCountMax);
                return variantCountMax;
            }
        }

        private static Dictionary<ThingDef, List<HeadTypeDef>> _cachedHeadTypeDefs = new Dictionary<ThingDef, List<HeadTypeDef>>();
        public static List<HeadTypeDef> GetAvailableAlienHeadTypes(this ThingDef pawnThingDef)
        {
            if (pawnThingDef == null) { return null; }

            if (_cachedHeadTypeDefs.TryGetValue(pawnThingDef, out var cached))
            {
                return cached;
            }
            else
            {
                var alienRaceFieldInfo = pawnThingDef.GetType().GetField("alienRace");
                var alienRace = alienRaceFieldInfo.GetValue(pawnThingDef);

                var generalSettingsFieldInfo = alienRace.GetType().GetField("generalSettings");
                var generalSettings = generalSettingsFieldInfo.GetValue(alienRace);

                var alienPartGeneratorFieldInfo = generalSettings.GetType().GetField("alienPartGenerator");
                var alienPartGenerator = alienPartGeneratorFieldInfo.GetValue(generalSettings);

                var headTypesFieldInfo = alienPartGenerator.GetType().GetField("headTypes");
                var headTypes = headTypesFieldInfo.GetValue(alienPartGenerator) as List<HeadTypeDef>;

                _cachedHeadTypeDefs.Add(pawnThingDef, headTypes.OrderBy(v => v.label).ToList());
                return headTypes;
            }
        }
    }
}
