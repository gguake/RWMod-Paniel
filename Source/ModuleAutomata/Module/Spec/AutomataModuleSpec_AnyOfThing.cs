using RimWorld;
using System.Text;
using Verse;

namespace ModuleAutomata
{
    public class AutomataModuleSpec_AnyOfThing : AutomataModuleSpec
    {
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
                    sb.Append($"({quality.Value.GetLabelShort()})");
                }

                return sb.ToString();
            }
        }

        public override QualityCategory Quality => quality ?? QualityCategory.Normal;

        public override ThingDef Stuff => stuffDef;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref quality, "quality");
            Scribe_Defs.Look(ref stuffDef, "stuffDef");
        }

        public override bool Equals(object obj)
            => obj is AutomataModuleSpec_AnyOfThing other &&
            moduleDef == other.moduleDef &&
            quality == other.quality &&
            stuffDef == other.stuffDef;

        public override int GetHashCode()
        {
            int hashCode = 1303672492;
            hashCode = hashCode * -1521134295 + (moduleDef?.GetHashCode() ?? 0);
            hashCode = hashCode * -1521134295 + (quality?.GetHashCode() ?? 0);
            hashCode = hashCode * -1521134295 + (stuffDef?.GetHashCode() ?? 0);
            return hashCode;
        }
    }
}
