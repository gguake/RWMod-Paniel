using AutomataRace.Logic;
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
        public AutomataSpecializationDef specializationDef = AutomataRaceDefOf.PN_Specialization_Combat;
        public ThingDef baseMaterialDef = AutomataBillService.GetBaseMaterialThingDefs().RandomElement();
        public Dictionary<ThingDef, int> ingredients = new Dictionary<ThingDef, int>();

        public AutomataAppearanceParameter appearance = new AutomataAppearanceParameter();

        public AutomataData()
        {
            ingredients = new Dictionary<ThingDef, int>();

            var billWorker = AutomataRaceDefOf.PN_Make_Automaton.billWorker as CustomizableBillWorker_MakeAutomata;
            foreach (var ingredientCount in billWorker.fixedIngredients)
            {
                var thingDef = ingredientCount.filter.AllowedThingDefs.FirstOrDefault();
                if (thingDef != null)
                {
                    ingredients.Add(thingDef, (int)ingredientCount.GetBaseCount());
                }
            }

            int componentIndustrialCount = Rand.Range(0, billWorker.componentTotalCount);
            int componentSpacerCount = billWorker.componentTotalCount - componentIndustrialCount;
            ingredients.Add(ThingDefOf.ComponentIndustrial, componentIndustrialCount);
            ingredients.Add(ThingDefOf.ComponentSpacer, componentSpacerCount);
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref specializationDef, "specialization");
            Scribe_Defs.Look(ref baseMaterialDef, "baseMaterial");
            Scribe_Collections.Look(ref ingredients, "ingredients");
            Scribe_Deep.Look(ref appearance, "appearance");
        }
    }

    public class CompAutomataDataHolder : ThingComp
    {
        public AutomataData automataData = new AutomataData();
        private int _cachedMarketValue = -1;

        public int MarketValue
        {
            get
            {
                if (_cachedMarketValue < 0)
                {
                    float t = 0f;
                    foreach (var kv in automataData.ingredients.Where(x => x.Key.tradeability == Tradeability.All))
                    {
                        t += kv.Key.BaseMarketValue * kv.Value;
                    }

                    _cachedMarketValue = (int)t;
                }

                return _cachedMarketValue;
            }
        }

        public CompAutomataDataHolder() : base()
        {
        }

        public void CopyFrom(CompAutomataDataHolder other)
        {
            automataData = other.automataData;
        }

        public override void PostExposeData()
        {
            Scribe_Deep.Look(ref automataData, "automataData");
        }

        public override string CompInspectStringExtra()
        {
            if (automataData != null)
            {
                return $"{"PN_AUTOMATA_MATERIAL".Translate()}: {automataData.baseMaterialDef.LabelCap}";
            }

            return null;
        }

        public override string TransformLabel(string label)
        {
            if (parent is Pawn)
            {
                return label;
            }
            else
            {
                QualityCategory quality = parent.GetComp<CompQuality>().Quality;
                return $"{automataData.specializationDef.PackagedThingDefLabelCap} ({quality.GetLabel()})";
            }
        }
    }
}
