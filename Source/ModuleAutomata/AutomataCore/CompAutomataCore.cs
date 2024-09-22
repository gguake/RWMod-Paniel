using RimWorld;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Verse;

namespace ModuleAutomata
{
    public class QualitySkill
    {
        public QualityCategory quality;
        public int skillLevel;

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "quality", xmlRoot.Name);
            if (xmlRoot.HasChildNodes)
            {
                skillLevel = ParseHelper.FromString<int>(xmlRoot.FirstChild.Value);
            }
        }
    }

    public class CompProperties_AutomataCore : CompProperties
    {
        public List<QualitySkill> qualitySkillValues;
        public WorkTags workDisables;

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

            return "PN_AUTOMATA_CORE".Translate(label, sourceName.ToStringShort).Resolve();
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
                return baseSkillLevel + Mathf.FloorToInt(sourceSkillLevel / 3f);
            }

            return baseSkillLevel;
        }
    }
}
