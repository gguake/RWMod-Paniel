using RimWorld;
using System;
using System.Xml;
using Verse;

namespace ModuleAutomata
{
    public class QualityHediff
    {
        public QualityCategory quality;
        public HediffDef hediff;

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            if (Enum.TryParse(xmlRoot.Name, out quality))
            {
                if (xmlRoot.HasChildNodes)
                {
                    DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "hediff", xmlRoot.FirstChild.Value);
                }
            }
        }
    }

}
