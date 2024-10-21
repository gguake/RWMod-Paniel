using RimWorld;
using Verse;

namespace ModuleAutomata
{
    public class AutomataModuleSpec_Core : AutomataModuleSpec
    {
        public Thing thing;

        public override string Label
        {
            get
            {
                var comp = thing.TryGetComp<CompAutomataCore>();
                if (comp == null || comp.CoreInfo == null) { return moduleDef.mainIngredientDef.LabelCap; }

                return $"{PNLocale.MakeModuleLabel(moduleDef, Quality, null)} ({comp.CoreInfo.sourceName.ToStringShort})";
            }
        }

        public override QualityCategory Quality
        {
            get
            {
                var comp = thing.TryGetComp<CompAutomataCore>();
                if (comp == null) { return QualityCategory.Normal; }

                return comp.CoreInfo.quality;
            }
        }

        public override ThingDef Stuff => null;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref thing, "thing");
        }

        public override bool Equals(object obj)
            => obj is AutomataModuleSpec_Core other && 
            moduleDef == other.moduleDef && 
            thing == other.thing;

        public override int GetHashCode()
        {
            int hashCode = -1377030798;
            hashCode = hashCode * -1521134295 + (moduleDef?.GetHashCode() ?? 0);
            hashCode = hashCode * -1521134295 + (thing?.GetHashCode() ?? 0);
            return hashCode;
        }
    }
}
