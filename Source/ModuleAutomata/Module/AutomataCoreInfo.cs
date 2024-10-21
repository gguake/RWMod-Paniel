using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
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
            if (Enum.TryParse(xmlRoot.Name, out quality))
            {
                if (xmlRoot.HasChildNodes)
                {
                    skillLevel = ParseHelper.FromString<int>(xmlRoot.FirstChild.Value);
                }
            }
        }
    }

    public class AutomataCoreInfo : IExposable
    {
        public AutomataModuleDef coreModuleDef;
        public QualityCategory quality = QualityCategory.Normal;
        public Name sourceName;
        public Dictionary<SkillDef, int> sourceSkill;

        public string Label => sourceName != null ?
            PNLocale.MakeModuleLabel(coreModuleDef, quality, null) :
            $"{PNLocale.MakeModuleLabel(coreModuleDef, quality, null)} ({sourceName.ToStringShort})";

        public void ExposeData()
        {
            Scribe_Defs.Look(ref coreModuleDef, "coreModuleDef");
            Scribe_Values.Look(ref quality, "quality");

            Scribe_Deep.Look(ref sourceName, "sourceName");
            Scribe_Collections.Look(ref sourceSkill, "sourceSkill", LookMode.Def, LookMode.Value);
        }

        public void Initialize(AutomataModuleDef moduleDef, QualityCategory quality, Pawn pawn = null)
        {
            this.coreModuleDef = moduleDef;
            this.quality = quality;

            var automataCoreModExt = coreModuleDef.GetModExtension<AutomataCoreModExtension>();

            sourceName = pawn?.Name;
            sourceSkill = new Dictionary<SkillDef, int>();

            foreach (var skillDef in DefDatabase<SkillDef>.AllDefsListForReading)
            {
                if (skillDef.IsDisabled(automataCoreModExt.workDisables, automataCoreModExt.DisabledWorkTypeDefs)) { continue; }

                var baseSkillLevel = automataCoreModExt.qualitySkillValues.FirstOrDefault(v => v.quality == quality).skillLevel;
                var skillRecord = pawn?.skills.GetSkill(skillDef);

                var skillLevel = baseSkillLevel;
                if (skillRecord != null)
                {
                    skillLevel = (int)Mathf.Min(20f, baseSkillLevel + Mathf.FloorToInt(skillRecord.Level * automataCoreModExt.sourcePawnSkillMultiplier));
                }

                sourceSkill[skillDef] = skillLevel;
            }
        }

        public AutomataCoreInfo Clone()
        {
            return new AutomataCoreInfo()
            {
                coreModuleDef = coreModuleDef,
                quality = quality,
                sourceName = sourceName,
                sourceSkill = new Dictionary<SkillDef, int>(sourceSkill)
            };
        }
    }
}
