using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutomataRace.Logic
{
    public static class AutomataQualityService
    {
        public static int GetProbabilityWeightFromComponentScore(QualityCategory qualityCategory, int componentScore)
        {
            var qualityProperty = AutomataQualityProperty.GetQualityProperty(qualityCategory);
            return Mathf.FloorToInt(Mathf.Max(0, qualityProperty.scoreCurve.Evaluate(componentScore)));
        }

        public static Dictionary<QualityCategory, int> GetProductProbabilityWeights(int score)
        {
            var result = new Dictionary<QualityCategory, int>();
            result[QualityCategory.Awful] = GetProbabilityWeightFromComponentScore(QualityCategory.Awful, score);
            result[QualityCategory.Poor] = GetProbabilityWeightFromComponentScore(QualityCategory.Poor, score);
            result[QualityCategory.Normal] = GetProbabilityWeightFromComponentScore(QualityCategory.Normal, score);
            result[QualityCategory.Good] = GetProbabilityWeightFromComponentScore(QualityCategory.Good, score);
            result[QualityCategory.Excellent] = GetProbabilityWeightFromComponentScore(QualityCategory.Excellent, score);
            result[QualityCategory.Masterwork] = GetProbabilityWeightFromComponentScore(QualityCategory.Masterwork, score);
            result[QualityCategory.Legendary] = GetProbabilityWeightFromComponentScore(QualityCategory.Legendary, score);

            return result;
        }

        public static Dictionary<QualityCategory, float> GetProductProbability(int score)
        {
            var weights = GetProductProbabilityWeights(score);
            Dictionary<QualityCategory, float> result = new Dictionary<QualityCategory, float>();

            int weightSum = weights.Sum(x => x.Value);
            foreach (var kv in weights)
            {
                if (kv.Value > 0)
                {
                    result[kv.Key] = (float)kv.Value / weightSum;
                }
            }

            return result;
        }
    }
}
