using RimWorld;
using System.Text;
using Verse;

namespace ModuleAutomata
{
    public struct AutomataModuleBill : IExposable
    {
        public bool IsInvalid => moduleDef == null;

        public string Label
        {
            get
            {
                if (IsInvalid) return "";

                if (thing != null) { return thing.LabelCap; }

                var sb = new StringBuilder();
                if (stuff != null)
                {
                    sb.Append(stuff.LabelAsStuff);
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

        public AutomataModulePartDef modulePartDef;
        public AutomataModuleDef moduleDef;
        public ThingDef thingDef;
        public Thing thing;
        public QualityCategory? quality;
        public ThingDef stuff;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref modulePartDef, "modulePartDef");
            Scribe_Defs.Look(ref moduleDef, "moduleDef");
            Scribe_Defs.Look(ref thingDef, "thingDef");
            Scribe_References.Look(ref thing, "thing");
            Scribe_Values.Look(ref quality, "quality");
            Scribe_Defs.Look(ref stuff, "stuff");
        }
    }
}
