using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace AutomataRace
{
    public class CompAddCompHediffConditional : ThingComp
    {
        private static FieldInfo FieldInfo_ThingWithComps_comps = AccessTools.Field(typeof(ThingWithComps), "comps");
        
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (respawningAfterLoad)
            {
                return;
            }

            var compProps = FindAdaptComp();
            if (compProps != null)
            {
                var thingComp = (ThingComp)Activator.CreateInstance(compProps.compClass);
                thingComp.parent = parent;

                List<ThingComp> comps = FieldInfo_ThingWithComps_comps.GetValue(parent) as List<ThingComp>;
                if (comps == null)
                {
                    Log.Warning("Failed to get comps by reflection.");
                    return;
                }

                comps.Add(thingComp);
                thingComp.Initialize(compProps);

                comps.Remove(this);
            }
        }

        private CompProperties FindAdaptComp()
        {
            var Props = props as CompProperties_AddCompHediffSelector;
            if (Props == null)
            {
                return null;
            }

            Pawn pawn = parent as Pawn ?? (parent as Corpse)?.InnerPawn ?? null;
            if (pawn == null || pawn.health == null || pawn.health.hediffSet == null)
            {
                return null;
            }

            foreach (var hediffCondition in Props.hediffConditionalBlock)
            {
                if (hediffCondition.hediff == null || pawn.health.hediffSet.HasHediff(hediffCondition.hediff))
                {
                    return hediffCondition.comp;
                }
            }

            return null;
        }
    }
}
