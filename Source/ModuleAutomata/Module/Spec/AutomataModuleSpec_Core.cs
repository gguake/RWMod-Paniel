using RimWorld;
using Verse;

namespace ModuleAutomata
{
    public class AutomataModuleSpec_Core : AutomataModuleSpec
    {
        public Thing coreThing;

        public override string Label
        {
            get
            {
                var comp = coreThing.TryGetComp<CompAutomataCore>();
                if (comp == null || comp.CoreInfo == null) { return moduleDef.mainIngredientDef.LabelCap; }

                return $"{moduleDef.mainIngredientDef.LabelCap} {comp.CoreInfo.quality.GetLabelShort()} {comp.CoreInfo.sourceName.ToStringShort}";
            }
        }

        public override QualityCategory Quality
        {
            get
            {
                var comp = coreThing.TryGetComp<CompAutomataCore>();
                if (comp == null) { return QualityCategory.Normal; }

                return comp.CoreInfo.quality;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref coreThing, "coreThing");
        }

        public override bool Equals(object obj)
            => obj is AutomataModuleSpec_Core other && moduleDef == other.moduleDef && coreThing == other.coreThing;

        public override int GetHashCode()
        {
            int hashCode = -1377030798;
            hashCode = hashCode * -1521134295 + moduleDef.GetHashCode();
            hashCode = hashCode * -1521134295 + coreThing.GetHashCode();
            return hashCode;
        }
    }
}
