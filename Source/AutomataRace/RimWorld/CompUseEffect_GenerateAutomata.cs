using RimWorld;
using Verse;
using AutomataRace.Extensions;
using UnityEngine;
using AlienRace;

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

                if (automataData.appearance?.headGraphicPath != null)
                {
                    generated.story.SetHeadGraphicPath(automataData.appearance.headGraphicPath);
                }

                GenSpawn.Spawn(generated, parent.Position, parent.Map);
                CachedData.headGraphicPath(generated.story) = automataData.appearance.headGraphicPath;

                // CHECKME: Face Addon
                //generated.SetFaceBodyAddonVariant(automataData.appearance.faceVariantIndex);
            }
        }
    }
}
