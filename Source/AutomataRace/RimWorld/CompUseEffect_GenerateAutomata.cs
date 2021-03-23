using RimWorld;
using Verse;

namespace AutomataRace
{
    public class CompUseEffect_GenerateAutomata : CompUseEffect
    {
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);

            var automataDataComp = parent.GetComp<CompAutomataDataHolder>();
            var automataData = automataDataComp?.automataData;

            QualityCategory qualityCategory = parent.TryGetComp<CompQuality>()?.Quality ?? QualityCategory.Normal;
            var qualityProperty = GetQualityProperty(qualityCategory);
            if (qualityProperty == null)
            {
                Log.Error($"There is no AutomataQualityProperty for quality '{qualityCategory}'");
                return;
            }

            PawnKindDef pawnKindDef = qualityProperty.pawnKindDefs.TryGetValue(automataData.specializationDef, null);
            if (pawnKindDef != null)
            {
                PawnGenerationRequest pawnGenReq = new PawnGenerationRequest(
                    pawnKindDef,
                    faction: Faction.OfPlayer,
                    context: PawnGenerationContext.NonPlayer,
                    tile: -1,
                    forceGenerateNewPawn: true);

                Pawn generated = PawnGenerator.GeneratePawn(pawnGenReq);
                generated.GetComp<CompAutomataDataHolder>().CopyFrom(automataDataComp);

                GenSpawn.Spawn(generated, parent.Position, parent.Map);
            }
        }

        AutomataQualityProperty GetQualityProperty(QualityCategory quality)
        {
            switch (quality)
            {
                case QualityCategory.Awful:
                    return AutomataRaceDefOf.PN_AutomataQualityProperty_Awful;

                case QualityCategory.Poor:
                    return AutomataRaceDefOf.PN_AutomataQualityProperty_Poor;

                case QualityCategory.Normal:
                    return AutomataRaceDefOf.PN_AutomataQualityProperty_Normal;

                case QualityCategory.Good:
                    return AutomataRaceDefOf.PN_AutomataQualityProperty_Good;

                case QualityCategory.Excellent:
                    return AutomataRaceDefOf.PN_AutomataQualityProperty_Excellent;

                case QualityCategory.Masterwork:
                    return AutomataRaceDefOf.PN_AutomataQualityProperty_Masterwork;

                case QualityCategory.Legendary:
                    return AutomataRaceDefOf.PN_AutomataQualityProperty_Legendary;

                default:
                    return null;
            }
        }
    }
}
