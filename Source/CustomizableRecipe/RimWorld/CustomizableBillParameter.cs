using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CustomizableRecipe
{
    public abstract class CustomizableBillParameter : IExposable
    {
        public abstract void ExposeData();

        public abstract void Apply(Bill bill);
    }
}
