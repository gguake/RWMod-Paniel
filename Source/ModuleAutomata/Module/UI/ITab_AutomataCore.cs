﻿using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ModuleAutomata
{
    [StaticConstructorOnStartup]
    public static class ITab_AutomataCoreTexture
    {
        public static Texture2D SkillBarFillTex = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.1f));
    }

    public class ITab_AutomataCore : ITab
    {
        private List<SkillDef> skillDefsInListOrderCached;
        private float levelLabelWidth = -1f;

        public ITab_AutomataCore()
        {
            labelKey = "PN_AutomataCoreTabLabel";
        }

        protected override void UpdateSize()
        {
            base.UpdateSize();

            var compCore = Find.Selector.SingleSelectedThing?.TryGetComp<CompAutomataCore>();
            if (compCore == null) { return; }

            var automataCoreModExt = compCore.CoreInfo?.coreModuleDef.GetModExtension<AutomataCoreModExtension>();
            if (automataCoreModExt == null) { return; }

            skillDefsInListOrderCached = DefDatabase<SkillDef>.AllDefsListForReading
                .OrderByDescending(def => def.listOrder)
                .Where(v => compCore.CoreInfo.sourceSkill.ContainsKey(v))
                .ToList();

            size = new Vector2(280f, skillDefsInListOrderCached.Count * 27f + 20);
        }

        public override bool IsVisible
        {
            get
            {
                return Find.Selector.SingleSelectedThing?.TryGetComp<CompAutomataCore>() != null;
            }
        }

        protected override void FillTab()
        {
            var mainRect = new Rect(0f, 0f, size.x, size.y).ContractedBy(10f);
            GUI.BeginGroup(mainRect);
            try
            {
                var compCore = Find.Selector.SingleSelectedThing?.TryGetComp<CompAutomataCore>();
                if (compCore == null) { return; }

                for (int i = 0; i < skillDefsInListOrderCached.Count; i++)
                {
                    var skillDef = skillDefsInListOrderCached[i];
                    float x = Text.CalcSize(skillDefsInListOrderCached[i].skillLabel.CapitalizeFirst()).x;
                    if (x > levelLabelWidth)
                    {
                        levelLabelWidth = x;
                    }
                }

                for (int i = 0; i < skillDefsInListOrderCached.Count; i++)
                {
                    var skillDef = skillDefsInListOrderCached[i];
                    DrawSkill(skillDef, compCore.CoreInfo.sourceSkill[skillDef], new Vector2(0f, i * 27f));
                }
            }
            finally
            {
                GUI.EndGroup();
            }
        }

        private void DrawSkill(SkillDef skill, int level, Vector2 topLeft)
        {
            DrawSkill(skill, level, new Rect(topLeft.x, topLeft.y, 230f, 24f));
        }

        private void DrawSkill(SkillDef skillDef, int level, Rect holdingRect)
        {
            if (Mouse.IsOver(holdingRect))
            {
                GUI.DrawTexture(holdingRect, TexUI.HighlightTex);
            }

            Widgets.BeginGroup(holdingRect);
            try
            {
                using (new TextBlock(TextAnchor.MiddleLeft))
                {
                    var labelTextRect = new Rect(6f, 0f, levelLabelWidth + 6f, holdingRect.height);
                    Widgets.Label(labelTextRect, skillDef.skillLabel.CapitalizeFirst());

                    var position = new Rect(labelTextRect.xMax, 0f, 24f, 24f);
                    var fillBarRect = new Rect(position.xMax, 0f, holdingRect.width - position.xMax, holdingRect.height);
                    var fillPercent = Mathf.Max(0.01f, (float)level / 20f);
                    Widgets.FillableBar(fillBarRect, fillPercent, ITab_AutomataCoreTexture.SkillBarFillTex, null, doBorder: false);

                    var levelTextRect = new Rect(position.xMax + 4f, 0f, 999f, holdingRect.height);
                    levelTextRect.yMin += 3f;
                    var label = level.ToStringCached();

                    Widgets.Label(levelTextRect, label);
                }
            }
            finally
            {
                Widgets.EndGroup();
            }
        }
    }
}
