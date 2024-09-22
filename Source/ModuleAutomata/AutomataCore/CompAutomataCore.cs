using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace ModuleAutomata
{
    public class QualitySkill
    {
        public QualityCategory quality;
        public int skillLevel;

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            if (Enum.TryParse(xmlRoot.Name, out quality))
            {
                if (xmlRoot.HasChildNodes)
                {
                    skillLevel = ParseHelper.FromString<int>(xmlRoot.FirstChild.Value);
                }
            }
        }
    }

    public class CompProperties_AutomataCore : CompProperties
    {
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

        public Name sourceName;
        public Dictionary<SkillDef, int> sourceSkill;

        public override void PostExposeData()
        {
            Scribe_Deep.Look(ref sourceName, "sourceName");
            Scribe_Collections.Look(ref sourceSkill, "sourceSkill", LookMode.Def, LookMode.Value);
        }

        public override string TransformLabel(string label)
        {
            if (sourceName == null) { return label; }

            return "PN_AutomataCoreItemLabel".Translate(label, sourceName.ToStringShort).Resolve();
        }

        public void SetPawn(Pawn pawn)
        {
            sourceName = pawn.Name;
            sourceSkill = new Dictionary<SkillDef, int>();
            foreach (var skillRecord in pawn.skills.skills)
            {
                sourceSkill.Add(skillRecord.def, skillRecord.Level);
            }
        }

        public int GetSkillLevel(SkillDef skillDef)
        {
            var quality = parent.GetComp<CompQuality>()?.Quality ?? QualityCategory.Normal;
            var baseSkillLevel = Props.qualitySkillValues.FirstOrDefault(v => v.quality == quality).skillLevel;

            if (sourceSkill != null && sourceSkill.TryGetValue(skillDef, out var sourceSkillLevel))
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
