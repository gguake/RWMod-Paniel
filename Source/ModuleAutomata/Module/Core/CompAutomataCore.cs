using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Verse;

namespace ModuleAutomata
{
    public class CompProperties_AutomataCore : CompProperties
    {
        public CompProperties_AutomataCore()
        {
            compClass = typeof(CompAutomataCore);
        }
    }

    public class CompAutomataCore : ThingComp 
    {
        public CompProperties_AutomataCore Props => (CompProperties_AutomataCore)props;

        private AutomataCoreInfo _coreInfo;
        public AutomataCoreInfo CoreInfo => _coreInfo;

        public override void PostExposeData()
        {
            Scribe_Deep.Look(ref _coreInfo, "coreInfo");
        }

        public override string TransformLabel(string label)
        {
            if (CoreInfo == null || parent is Pawn) { return label; }

            return PNLocale.PN_AutomataCoreItemLabel.Translate(label, CoreInfo.sourceName.ToStringShort).Resolve();
        }

        public void InitializePawnInfo(ThingDef thingDef, QualityCategory quality, Pawn pawn)
        {
            _coreInfo = new AutomataCoreInfo();
            _coreInfo.Initialize(thingDef, quality, pawn);
        }
    }
}
