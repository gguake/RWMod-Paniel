using Verse;
using RimWorld;

namespace AutomataRace
{
    public class CompUseEffect_GeneratePawn : CompUseEffect
    {
        public CompProperties_UseEffectGeneratePawn Props => (CompProperties_UseEffectGeneratePawn)props;

        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);

            var settings = this.props as CompProperties_UseEffectGeneratePawn;
            if (settings == null)
            {
                Log.Error("CompUseEffect_GeneratePawn should created by CompProperties_UseEffectGeneratePawn.");
                return;
            }
            
            var selected = settings.samples.RandomElementByWeight((GeneratePawnSample sample) =>
            {
                return sample.weight;
            });
            
            var pawnKindDef = DefDatabase<PawnKindDef>.GetNamed(selected.pawnKindDefName);
            if (pawnKindDef == null)
            {
                Log.Error($"Tried to generate not existing pawnKindDef named '{selected.pawnKindDefName}' from item '{parent.def.defName}'.");
                return;
            }

            PawnGenerationRequest pawnGenReq = new PawnGenerationRequest(pawnKindDef,
                faction: Faction.OfPlayer,
                context: PawnGenerationContext.NonPlayer,
                tile: -1,
                forceGenerateNewPawn: true,
                fixedBiologicalAge: 0,
                fixedChronologicalAge: 0);

            Pawn generated = PawnGenerator.GeneratePawn(pawnGenReq);
            GenSpawn.Spawn(generated, parent.Position, parent.Map);

            var title = Props.letterLabel.Formatted(generated.Named("PAWN")).AdjustedFor(generated);
            var text = Props.letterText.Formatted(generated.Named("PAWN")).AdjustedFor(generated);
            Find.LetterStack.ReceiveLetter(title, text, LetterDefOf.PositiveEvent, generated);
        }

        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            return true;
        }
    }
}
