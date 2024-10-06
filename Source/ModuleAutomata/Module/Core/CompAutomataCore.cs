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
        [NoTranslate]
        public string specializationIconPath;

        private Texture2D _specializationIcon;
        public Texture2D SpecializationIcon
        {
            get
            {
                if (_specializationIcon == null)
                {
                    _specializationIcon = ContentFinder<Texture2D>.Get(specializationIconPath);
                }

                return _specializationIcon;
            }
        }

        public List<QualitySkill> qualitySkillValues;
        public float sourcePawnSkillMultiplier = 1f;

        public WorkTags workDisables;

        private List<WorkTypeDef> _disabledWorkTypeDefs;
        public List<WorkTypeDef> DisabledWorkTypeDefs
        {
            get
            {
                if (_disabledWorkTypeDefs == null)
                {
                    _disabledWorkTypeDefs = DefDatabase<WorkTypeDef>.AllDefsListForReading
                        .Where(def => (def.workTags & workDisables) != 0)
                        .ToList();
                }

                return _disabledWorkTypeDefs;
            }
        }

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

        public int GetSkillLevel(SkillDef skillDef)
        {
            var quality = parent.GetComp<CompQuality>()?.Quality ?? QualityCategory.Normal;
            var baseSkillLevel = Props.qualitySkillValues.FirstOrDefault(v => v.quality == quality).skillLevel;

            if (CoreInfo != null && CoreInfo.sourceSkill.TryGetValue(skillDef, out var sourceSkillLevel))
            {
                return (int)Mathf.Min(20f, baseSkillLevel + Mathf.FloorToInt(sourceSkillLevel * Props.sourcePawnSkillMultiplier));
            }

            return (int)Mathf.Min(20f, baseSkillLevel);
        }

        public bool IsDisabledSkill(SkillDef skillDef)
        {
            return skillDef.IsDisabled(Props.workDisables, Props.DisabledWorkTypeDefs);
        }
    }
}
