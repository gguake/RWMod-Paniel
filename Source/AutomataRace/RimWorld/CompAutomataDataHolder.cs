using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AutomataRace
{
    public class AutomataData : IExposable
    {
        public ThingDef baseMaterialDef = ThingDefOf.Steel;
        public Dictionary<ThingDef, int> ingredients = new Dictionary<ThingDef, int>();

        public void ExposeData()
        {
            Scribe_Defs.Look(ref baseMaterialDef, "baseMaterial");
            Scribe_Collections.Look(ref ingredients, "ingredients");
        }
    }

    public class CompAutomataDataHolder : ThingComp
    {
        public AutomataData automataData = new AutomataData();

        private CompPropeties_AutomataDataHolder Props => (CompPropeties_AutomataDataHolder)props;

        public CompAutomataDataHolder() : base()
        {
        }

        public override void PostExposeData()
        {
            Scribe_Deep.Look(ref automataData, "automataData");
            base.PostExposeData();
        }
    }
}
