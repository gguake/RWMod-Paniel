using RimWorld;
using System;
using System.Collections.Generic;
using System.Xml;
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

    public class AutomataCorePawnInfo : IExposable
    {
        public Name sourceName;
        public Dictionary<SkillDef, int> sourceSkill;

        public void ExposeData()
        {
            Scribe_Deep.Look(ref sourceName, "sourceName");
            Scribe_Collections.Look(ref sourceSkill, "sourceSkill", LookMode.Def, LookMode.Value);
        }

        public void InitializeFromPawn(Pawn pawn)
        {
            sourceName = pawn.Name;
            sourceSkill = new Dictionary<SkillDef, int>();
            foreach (var skillRecord in pawn.skills.skills)
            {
                sourceSkill.Add(skillRecord.def, skillRecord.Level);
            }
        }
    }
}
