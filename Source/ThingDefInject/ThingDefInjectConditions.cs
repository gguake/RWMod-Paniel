using System.Collections.Generic;
using Verse;

namespace AutomataRace.ThingDefInject
{
    public interface ThingDefInjectCondition
    {
        bool Check(ThingDef thingDef);
    }

    public class PassAll : ThingDefInjectCondition
    {
        public List<ThingDefInjectCondition> conditions;

        public bool Check(ThingDef thingDef)
        {
            foreach (var condition in conditions)
            {
                if (!condition.Check(thingDef))
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class PassAny : ThingDefInjectCondition
    {
        public List<ThingDefInjectCondition> conditions;

        public bool Check(ThingDef thingDef)
        {
            foreach (var condition in conditions)
            {
                if (condition.Check(thingDef))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class IsDefNameEqual : ThingDefInjectCondition
    {
        public string defName;
        public bool not;

        public bool Check(ThingDef thingDef)
        {
            return not ^ (thingDef.defName == defName);
        }
    }

    public class IsHumanlike : ThingDefInjectCondition
    {
        public bool not = false;

        public bool Check(ThingDef thingDef)
        {
            return not ^ (thingDef.race?.Humanlike ?? false);
        }
    }

    public class IsFlesh : ThingDefInjectCondition
    {
        public bool not = false;

        public bool Check(ThingDef thingDef)
        {
            return not ^ (thingDef.race?.IsFlesh ?? false);
        }
    }
}
