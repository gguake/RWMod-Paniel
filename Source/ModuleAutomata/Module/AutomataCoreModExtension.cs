using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ModuleAutomata
{
    public class AutomataCoreModExtension : DefModExtension
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

        public BackstoryDef childhoodBackstory;
        public BackstoryDef adulthoodBackstory;

    }
}
