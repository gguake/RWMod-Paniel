using Verse;
using RimWorld;

namespace AutomataRace
{
    public class CompUseEffect_GeneratePawn : CompUseEffect
    {
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
                forceGenerateNewPawn: true);

            Pawn generated = PawnGenerator.GeneratePawn(pawnGenReq);
            GenSpawn.Spawn(generated, parent.Position, parent.Map);
        }

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            failReason = null;
            return true;
        }
    }
}
