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

    }

}
