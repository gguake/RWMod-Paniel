using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace CustomizableRecipe.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public static partial class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new Harmony("gguake.customizablerecipe");

            harmony.Patch(AccessTools.Method(typeof(BillUtility), "MakeNewBill"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(BillUtility_MakeNewBill_Prefix)));

            harmony.Patch(AccessTools.Method(typeof(BillStack), "AddBill"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(BillStack_AddBill_Prefix)));

            harmony.Patch(AccessTools.Method(typeof(RecipeDef), "PawnSatisfiesSkillRequirements"),
                prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(RecipeDef_PawnSatisfiesSkillRequirements_Prefix)));

            Log.Message($"[CustomizableRecipe] Harmony patch completed.");
        }

        public static bool BillUtility_MakeNewBill_Prefix(RecipeDef recipe, ref Bill __result)
        {
            var customizableRecipe = recipe as CustomizableRecipeDef;
            if (customizableRecipe != null)
            {
                __result = new Bill_Customizer(customizableRecipe);
                return false;
            }

            return true;
        }

        public static bool BillStack_AddBill_Prefix(BillStack __instance, Bill bill)
        {
            var customizableBill = bill as Bill_Customizer;
            if (customizableBill != null)
            {
                var worker = customizableBill.Worker;
                worker.billStack = __instance;

                if (!worker.OnAddBill())
                {
                    return false;
                }
            }

            return true;
        }

        public static bool RecipeDef_PawnSatisfiesSkillRequirements_Prefix(RecipeDef __instance, ref bool __result)
        {
            if (__instance is CustomizableRecipeDef)
            {
                __result = true;
                return false;
            }

            return true;
        }
    }
}
