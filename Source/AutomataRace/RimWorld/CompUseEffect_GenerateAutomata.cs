using RimWorld;
using Verse;
using AutomataRace.Extensions;
using UnityEngine;

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
            var qualityProperty = AutomataQualityProperty.GetQualityProperty(qualityCategory);
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

                generated.story.hairDef = automataData.appearance?.hairDef;
                generated.story.hairColor = Color.white;

                GenSpawn.Spawn(generated, parent.Position, parent.Map);
                generated.SetFaceBodyAddonVariant(automataData.appearance.faceVariantIndex);
            }
        }
    }
}
