using RimWorld;
using System.Text;
using Verse;

namespace ModuleAutomata
{
    public abstract class AutomataModuleSpec : IExposable
    {
        public AutomataModulePartDef modulePartDef;
        public AutomataModuleDef moduleDef;

        public abstract string Label { get; }

        public abstract QualityCategory Quality { get; }

        public virtual void ExposeData()
        {
            Scribe_Defs.Look(ref modulePartDef, "modulePartDef");
            Scribe_Defs.Look(ref moduleDef, "moduleDef");
        }
    }

    public class AutomataModuleSpec_Core : AutomataModuleSpec
    {
        public Thing thing;

        public override string Label
        {
            get
            {
                var comp = thing.TryGetComp<CompAutomataCore>();
                if (comp == null) { return moduleDef.ingredientThingDef.LabelCap; }

                var sb = new StringBuilder();


                return PNLocale.PN_AutomataCoreItemLabel.Translate(moduleDef.ingredientThingDef.LabelCap, comp.CoreInfo.sourceName.ToStringShort).Resolve();
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

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_References.Look(ref thing, "thing");
        }
    }

    public class AutomataModuleSpec_ThingDef : AutomataModuleSpec
    {
        public ThingDef thingDef;
        public QualityCategory? quality;
        public ThingDef stuffDef;

        public override string Label
        {
            get
            {
                var sb = new StringBuilder();
                if (stuffDef != null)
                {
                    sb.Append(stuffDef.LabelAsStuff);
                    sb.Append(" ");
                }

                sb.Append(moduleDef.LabelCap);

                if (quality != null)
                {
                    sb.Append(" ");
                    sb.Append($"({QualityUtility.GetLabelShort(quality.Value)})");
                }

                return sb.ToString();
            }
        }

        public override QualityCategory Quality => quality ?? QualityCategory.Normal;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Defs.Look(ref thingDef, "thingDef");
            Scribe_Values.Look(ref quality, "quality");
            Scribe_Defs.Look(ref stuffDef, "stuffDef");
        }
    }
}
