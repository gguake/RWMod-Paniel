using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static Dictionary<QualityCategory, int> GetProductProbabilityWeights(CustomizableBillWorker_MakeAutomata billWorker)
        {
            var result = new Dictionary<QualityCategory, int>();
            result[QualityCategory.Awful] = GetProbabilityWeightFromComponentScore(QualityCategory.Awful, billWorker.Score);
            result[QualityCategory.Poor] = GetProbabilityWeightFromComponentScore(QualityCategory.Poor, billWorker.Score);
            result[QualityCategory.Normal] = GetProbabilityWeightFromComponentScore(QualityCategory.Normal, billWorker.Score);
            result[QualityCategory.Good] = GetProbabilityWeightFromComponentScore(QualityCategory.Good, billWorker.Score);
            result[QualityCategory.Excellent] = GetProbabilityWeightFromComponentScore(QualityCategory.Excellent, billWorker.Score);
            result[QualityCategory.Masterwork] = GetProbabilityWeightFromComponentScore(QualityCategory.Masterwork, billWorker.Score);
            result[QualityCategory.Legendary] = GetProbabilityWeightFromComponentScore(QualityCategory.Legendary, billWorker.Score);

            return result;
        }

        public static Dictionary<QualityCategory, float> GetProductProbability(CustomizableBillWorker_MakeAutomata billWorker)
        {
            var weights = GetProductProbabilityWeights(billWorker);
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
