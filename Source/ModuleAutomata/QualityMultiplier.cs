using RimWorld;
using System;
using System.Xml;
using Verse;

namespace ModuleAutomata
{
    public class QualityMultiplier
    {
        public QualityCategory quality;
        public float multiplier;

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            if (Enum.TryParse(xmlRoot.Name, out quality))
            {
                multiplier = ParseHelper.FromString<float>(xmlRoot.FirstChild.Value);
            }
        }
    }

}
