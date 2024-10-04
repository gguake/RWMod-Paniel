using RimWorld;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace ModuleAutomata
{
    public class AutomataDefaultAssemblePartSetting
    {
        public AutomataModulePartDef modulePartDef;
        public AutomataModuleDef moduleDef;

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "modulePartDef", xmlRoot.Name);
            if (xmlRoot.HasChildNodes)
            {
                DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "moduleDef", xmlRoot.FirstChild.Value);
            }
        }
    }

    public class AutomataAssembleUIExtension : DefModExtension
    {
        public List<PawnCapacityDef> capacityDefs;

        public List<StatDef> statDefs;

        public List<AutomataDefaultAssemblePartSetting> defaultModuleSetting;
    }
}
