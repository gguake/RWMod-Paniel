using RimWorld;
using Verse;

namespace ModuleAutomata
{
    public class CompProperties_AutomataCore : CompProperties
    {
        public CompProperties_AutomataCore()
        {
            compClass = typeof(CompAutomataCore);
        }
    }

    public class CompAutomataCore : ThingComp 
    {
        public CompProperties_AutomataCore Props => (CompProperties_AutomataCore)props;

        private AutomataCoreInfo _coreInfo;
        public AutomataCoreInfo CoreInfo => _coreInfo;

        public override void PostExposeData()
        {
            Scribe_Deep.Look(ref _coreInfo, "coreInfo");
        }

        public override string TransformLabel(string label)
        {
            if (CoreInfo == null || parent is Pawn) { return label; }

            if (CoreInfo.sourceName != null)
            {
                return $"{label} ({CoreInfo.sourceName.ToStringShort})";
            }

            return label;
        }

        public void InitializePawnInfo(AutomataModuleDef moduleDef, QualityCategory quality, Pawn pawn)
        {
            _coreInfo = new AutomataCoreInfo();
            _coreInfo.Initialize(moduleDef, quality, pawn);
        }

        public override void Notify_DuplicatedFrom(Pawn source)
        {
            var sourceComp = source.GetComp<CompAutomataCore>();
            _coreInfo = sourceComp._coreInfo.Clone();
        }
    }
}
