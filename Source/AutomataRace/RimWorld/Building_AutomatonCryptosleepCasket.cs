using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using Verse.AI;

namespace AutomataRace
{
    public class Building_AutomatonCryptosleepCasket : Building_CryptosleepCasket
	{
		//static readonly Func<Building_Casket, Pawn, IEnumerable<FloatMenuOption>> buildingCasket_GetFloatMenuOptions;
		//static Building_AutomatonCryptosleepCasket()
		//{
		//	var dynMethod = new DynamicMethod(
		//		"GetFloatMenuOptions_Building_Casket", 
		//		typeof(IEnumerable<FloatMenuOption>), 
		//		new Type[] { typeof(Building_Casket), typeof(Pawn) }, 
		//		typeof(Building_Casket));

		//	ILGenerator il = dynMethod.GetILGenerator();
		//	il.Emit(OpCodes.Ldarg, 0);
		//	il.Emit(OpCodes.Ldarg, 1);
		//	il.EmitCall(OpCodes.Call, typeof(Building_Casket).GetMethod("GetFloatMenuOptions", BindingFlags.Public | BindingFlags.Instance), null);
		//	il.Emit(OpCodes.Ret);

		//	buildingCasket_GetFloatMenuOptions = (Func<Building_Casket, Pawn, IEnumerable<FloatMenuOption>>)dynMethod.CreateDelegate(typeof(Func<Building_Casket, Pawn, IEnumerable<FloatMenuOption>>));
		//}

		public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn myPawn)
		{
			if (myPawn.def != AutomataRaceDefOf.Paniel_Race)
			{
				yield return new FloatMenuOption("CannotUseReason".Translate("PN_AUTOMATACRYPTOSLEEPONLY".Translate()), null);
				yield break;
			}

			foreach (var option in base.GetFloatMenuOptions(myPawn))
            {
				yield return option;
            }
		}
    }
}
