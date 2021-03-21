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

            PawnGenerationRequest pawnGenReq = new PawnGenerationRequest(AutomataRaceDefOf.Paniel_Randombox_Normal,
                faction: Faction.OfPlayer,
                context: PawnGenerationContext.NonPlayer,
                tile: -1,
                forceGenerateNewPawn: true);

            Pawn generated = PawnGenerator.GeneratePawn(pawnGenReq);

            generated.GetComp<CompAutomataDataHolder>().CopyFrom(automataDataComp);
            GenSpawn.Spawn(generated, parent.Position, parent.Map);
        }
    }
}
