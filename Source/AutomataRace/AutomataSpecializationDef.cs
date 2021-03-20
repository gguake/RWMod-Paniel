using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AutomataRace
{
    public class AutomataSpecializationBonusSkill
    {
        public SkillDef skill;
        public int addition;
        public float multiplier;
    }

    public class AutomataSpecializationDef : Def
    {
        public string uiIconPath;
        public List<AutomataSpecializationBonusSkill> bonusSkills = new List<AutomataSpecializationBonusSkill>();

        private Texture2D _uiIcon;

        public Texture2D UIIcon
        {
            get
            {
                if (_uiIcon == null)
                {
                    _uiIcon = ContentFinder<Texture2D>.Get(uiIconPath);
                }

                return _uiIcon;
            }
        }
    }
}
