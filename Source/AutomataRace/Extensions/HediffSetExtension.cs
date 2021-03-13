using RimWorld;
using Verse;

namespace AutomataRace.Extensions
{
    public static class HediffSetExtension
	{
		public static BodyPartRecord GetSpine(this HediffSet hediffSet)
		{
			foreach (BodyPartRecord notMissingPart in hediffSet.GetNotMissingParts())
			{
				if (notMissingPart.def.tags.Contains(BodyPartTagDefOf.Spine))
				{
					return notMissingPart;
				}
			}
			return null;
		}
	}
}
