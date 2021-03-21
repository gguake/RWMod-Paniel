using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomataRace.Logic
{
    public static class AutomataQualityService
    {
        public static int GetProbabilityWeightFromComponentScore(QualityCategory qualityCategory, int componentScore)
        {
            switch (qualityCategory)
            {
                case QualityCategory.Awful:
                    return Math.Max(0, 125 - componentScore);

                case QualityCategory.Poor:
                    return Math.Max(0, 200 - componentScore);

                case QualityCategory.Normal:
                    return Math.Max(50, 300 - componentScore);

                case QualityCategory.Good:
                    return Math.Max(componentScore - 180, 0);

                case QualityCategory.Excellent:
                    return Math.Max(componentScore - 250, 0);

                case QualityCategory.Masterwork:
                    return Math.Max(componentScore - 350, 0);

                case QualityCategory.Legendary:
                    return Math.Max(componentScore - 500, 0);

                default:
                    return 0;
            }
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
