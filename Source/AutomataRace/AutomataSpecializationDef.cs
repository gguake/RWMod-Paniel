using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AutomataRace
{
    public class AutomataSpecializationDef : Def
    {
        public string uiIconPath;
        public string recipeDefLabel;
        public string packagedDefLabel;

        [Unsaved(false)]
        private Texture2D _uiIcon;

        [Unsaved(false)]
        private TaggedString _cachedRecipeDefLabel;

        [Unsaved(false)]
        private TaggedString _cachedPackagedDefLabel;

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

        public TaggedString RecipeDefLabelCap
        {
            get
            {
                if (recipeDefLabel.NullOrEmpty())
                {
                    return null;
                }
                if (_cachedRecipeDefLabel.NullOrEmpty())
                {
                    _cachedRecipeDefLabel = recipeDefLabel.CapitalizeFirst();
                }

                return _cachedRecipeDefLabel;
            }
        }

        public TaggedString PackagedThingDefLabelCap
        {
            get
            {
                if (packagedDefLabel.NullOrEmpty())
                {
                    return null;
                }
                if (_cachedPackagedDefLabel.NullOrEmpty())
                {
                    _cachedPackagedDefLabel = packagedDefLabel.CapitalizeFirst();
                }

                return _cachedPackagedDefLabel;
            }
        }
    }
}
