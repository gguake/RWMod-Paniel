﻿using RimWorld;
using UnityEngine;
using Verse;

namespace AutomataRace
{
    public class CompUseEffect_GenerateAutomata : CompUseEffect
    {
        public CompProperties_UseEffectGenerateAutomata Props => (CompProperties_UseEffectGenerateAutomata)props;

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
                    forceGenerateNewPawn: true,
                    fixedBiologicalAge: 0,
                    fixedChronologicalAge: 0);

                Pawn generated = PawnGenerator.GeneratePawn(pawnGenReq);
                generated.GetComp<CompAutomataDataHolder>().CopyFrom(automataDataComp);

                generated.story.hairDef = automataData.appearance?.hairDef;
                generated.story.HairColor = Color.white;

                if (automataData.appearance?.headTypeDef != null)
                {
                    generated.story.headType = automataData.appearance.headTypeDef;
                }

                GenSpawn.Spawn(generated, parent.Position, parent.Map);

                //CachedData.headGraphicPath(generated.story) = automataData.appearance.headGraphicPath;

                // CHECKME: Face Addon
                // generated.SetBodyAddonVariant(automataData.appearance.bodyAddonVariant);

                var title = Props.letterLabel.Formatted(generated.Named("PAWN")).AdjustedFor(generated);
                var text = Props.letterText.Formatted(generated.Named("PAWN")).AdjustedFor(generated);
                Find.LetterStack.ReceiveLetter(title, text, LetterDefOf.PositiveEvent, generated);
            }
        }
    }
}
