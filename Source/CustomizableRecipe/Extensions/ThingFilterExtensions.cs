using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe
{
    public static class ThingFilterExtensions
    {
        static FieldInfo _field_ThingFilter_thingDefs = AccessTools.Field(typeof(ThingFilter), "thingDefs");

        public static void SetThingDefs(this ThingFilter thingFilter, ThingDef thingDef)
        {
            _field_ThingFilter_thingDefs.SetValue(thingFilter, new List<ThingDef> { thingDef });
        }

        public static void SetThingDefs(this ThingFilter thingFilter, IEnumerable<ThingDef> thingDefs)
        {
            _field_ThingFilter_thingDefs.SetValue(thingFilter, thingDefs.ToList());
        }
    }
}
