using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe
{
    public static class ThingFilterExtensions
    {
        static ThingFilterExtensions()
        {
        }

        public static ThingFilter MakeClone(this ThingFilter thingFilter)
        {
            return thingFilter;
        }
    }
}
