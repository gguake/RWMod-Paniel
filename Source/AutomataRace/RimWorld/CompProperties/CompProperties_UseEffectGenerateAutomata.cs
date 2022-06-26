using RimWorld;

namespace AutomataRace
{
    public class CompProperties_UseEffectGenerateAutomata : CompProperties_UseEffect
    {
        public string letterLabel;
        public string letterText;

        public CompProperties_UseEffectGenerateAutomata()
        {
            compClass = typeof(CompUseEffect_GenerateAutomata);
        }
    }
}
