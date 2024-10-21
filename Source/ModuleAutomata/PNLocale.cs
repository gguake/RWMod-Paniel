using RimWorld;
using System.Text;
using Verse;

namespace ModuleAutomata
{
    public static class PNLocale
    {
        public const string PN_CommandCancelAssembleLabel = nameof(PN_CommandCancelAssembleLabel);
        public const string PN_CommandCancelAssembleDesc = nameof(PN_CommandCancelAssembleDesc);

        public const string PN_CommandAssembleNewLabel = nameof(PN_CommandAssembleNewLabel);
        public const string PN_CommandAssembleNewDesc = nameof(PN_CommandAssembleNewDesc);
        public const string PN_CommandReassembleLabel = nameof(PN_CommandReassembleLabel);
        public const string PN_CommandReassembleDesc = nameof(PN_CommandReassembleDesc);

        public const string PN_FloatMenuReassembleLabel = nameof(PN_FloatMenuReassembleLabel);

        public const string PN_DialogAssembleTitleLabel = nameof(PN_DialogAssembleTitleLabel);
        public const string PN_DialogModifyTitleLabel = nameof(PN_DialogModifyTitleLabel);
        public const string PN_DialogOrderTitleLabel = nameof(PN_DialogOrderTitleLabel);
        public const string PN_DialogTabPawnCapacityLabel = nameof(PN_DialogTabPawnCapacityLabel);
        public const string PN_DialogTabPawnSkillLabel = nameof(PN_DialogTabPawnSkillLabel);
        public const string PN_DialogTabNoSelectedCoreLabel = nameof(PN_DialogTabNoSelectedCoreLabel);
        public const string PN_DialogTabUnknownCoreLabel = nameof(PN_DialogTabUnknownCoreLabel);

        public const string PN_DialogHairSelectorLabel = nameof(PN_DialogHairSelectorLabel);
        public const string PN_DialogHeadSelectorLabel = nameof(PN_DialogHeadSelectorLabel);

        public const string PN_DialogEmptyModuleElementLabel = nameof(PN_DialogEmptyModuleElementLabel);
        public const string PN_DialogCancelInstallModuleOption = nameof(PN_DialogCancelInstallModuleOption);
        public const string PN_DialogCancelInstallModuleIfEmptyOption = nameof(PN_DialogCancelInstallModuleIfEmptyOption);

        public const string PN_DialogFloatMenuOptionNoModuleCandidate = nameof(PN_DialogFloatMenuOptionNoModuleCandidate);

        public static string MakeModuleLabel(Def moduleDef, QualityCategory? quality, ThingDef stuffDef)
        {
            var sb = new StringBuilder();
            if (stuffDef != null)
            {
                sb.Append(stuffDef.LabelAsStuff);
                sb.Append(" ");
            }

            sb.Append(moduleDef.LabelCap);

            if (quality != null)
            {
                sb.Append(" ");
                sb.Append($" ({quality.Value.GetLabelShort()})");
            }

            return sb.ToString();
        }
    }
}
