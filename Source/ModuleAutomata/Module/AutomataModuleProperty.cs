using RimWorld;
using System;
using System.Collections.Generic;
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

    public abstract class AutomataModuleProperty
    {
        public abstract void OnApplyPawn(Pawn pawn, AutomataModuleBill moduleBill);
    }

    public class AutomataModuleProperty_Core : AutomataModuleProperty
    {
        public override void OnApplyPawn(Pawn pawn, AutomataModuleBill moduleBill)
        {
        }
    }

    public class AutomataModuleProperty_ImplantHediff : AutomataModuleProperty
    {
        public List<QualityHediff> hediffs;

        public override void OnApplyPawn(Pawn pawn, AutomataModuleBill moduleBill)
        {
            var quality = moduleBill.moduleDef.IngredientWorker.HasQuality ? moduleBill.quality : QualityCategory.Normal;

            var hediffDef = hediffs.FirstOrDefault(qh => qh.quality == quality)?.hediff;
            if (hediffDef != null)
            {
                pawn.health.AddHediff(hediffDef, moduleBill.modulePartDef.FindBodyPartRecordFromPawn(pawn));
            }
        }
    }

    public class AutomataModuleProperty_BodyPartHediff : AutomataModuleProperty
    {
        public List<QualityHediff> hediffs;

        public override void OnApplyPawn(Pawn pawn, AutomataModuleBill moduleBill)
        {
            var quality = moduleBill.moduleDef.IngredientWorker.HasQuality ? moduleBill.quality : QualityCategory.Normal;

            var hediffDef = hediffs.FirstOrDefault(qh => qh.quality == quality)?.hediff;
            if (hediffDef != null)
            {
                pawn.health.AddHediff(hediffDef, moduleBill.modulePartDef.FindBodyPartRecordFromPawn(pawn));
            }
        }
    }

    public class AutomataModuleProperty_StatChange : AutomataModuleProperty
    {
        public List<QualityMultiplier> qualityMultiplier;
        public List<StatModifier> statOffsets;
        public List<StatModifier> statFactors;

        public override void OnApplyPawn(Pawn pawn, AutomataModuleBill moduleBill)
        {
        }
    }
}
