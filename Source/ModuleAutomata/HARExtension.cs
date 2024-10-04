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

        public static IEnumerable<HeadTypeDef> GetAvailableAlienHeadTypes(this ThingDef pawnThingDef)
        {
            if (pawnThingDef == null) { yield break; }

            var alienRaceFieldInfo = pawnThingDef.GetType().GetField("alienRace");
            var alienRace = alienRaceFieldInfo.GetValue(pawnThingDef);

            var generalSettingsFieldInfo = alienRace.GetType().GetField("generalSettings");
            var generalSettings = generalSettingsFieldInfo.GetValue(alienRace);

            var alienPartGeneratorFieldInfo = generalSettings.GetType().GetField("alienPartGenerator");
            var alienPartGenerator = alienPartGeneratorFieldInfo.GetValue(generalSettings);

            var headTypesFieldInfo = alienPartGenerator.GetType().GetField("headTypes");
            var headTypes = headTypesFieldInfo.GetValue(alienPartGenerator) as List<HeadTypeDef>;

            foreach (var headTypeDef in headTypes)
            {
                yield return headTypeDef;
            }
        }
    }
}
