using RimWorld;
using Verse;

namespace ModuleAutomata
{
    public struct AutomataModuleIngredientInfo
    {
        public ThingDef thingDef;
        public QualityCategory? quality;
        public ThingDef stuffDef;

        public bool HasAnyCondition => quality != null || stuffDef != null;

        public string Label => PNLocale.MakeModuleLabel(thingDef, quality, stuffDef);

        public AutomataModuleIngredientInfo(ThingDef thingDef, QualityCategory? quality, ThingDef stuff)
        {
            this.thingDef = thingDef;
            this.quality = quality;
            this.stuffDef = stuff;
        }

        public bool Match(Thing thing)
        {
            if (thing.def != thingDef) { return false; }
            if (quality != null && quality.Value != thing.TryGetComp<CompQuality>().Quality) { return false; }
            if (stuffDef != null && stuffDef != thing.Stuff) { return false; }

            return true;
        }
    }
}
