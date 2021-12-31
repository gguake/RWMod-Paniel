using UnityEngine;
using Verse;

namespace AutomataRace
{
    public class Graphic_Sprite : Graphic_Random
	{
		public GraphicData_Sprite Data => data as GraphicData_Sprite;

		public override Material MatSingle
        {
			get
			{
				var keyFrames = Data.KeyFrameAligned;
				var currentFrame = CurrentFrame;

				for (int i = 0; i < keyFrames.Count; ++i)
				{
					var keyFrame = keyFrames[i];
					if (currentFrame >= keyFrame.frame) { return subGraphics[keyFrame.index].MatSingle; }
				}

				return subGraphics[0].MatSingle;
			}
        }

		protected int CurrentFrame
        {
			get
            {
				if (Data.totalFrameLength <= 0) { return 0; }

				return GenTicks.TicksGame % Data.totalFrameLength;
            }
        }

		public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
		{
			if (newColorTwo != Color.white)
			{
				Log.ErrorOnce("Cannot use Graphic_Random.GetColoredVersion with a non-white colorTwo.", 9910251);
			}
			return GraphicDatabase.Get<Graphic_Sprite>(path, newShader, drawSize, newColor, Color.white, data);
		}

		public override Material MatSingleFor(Thing thing)
		{
			if (thing == null)
			{
				return MatSingle;
			}

			return SubGraphicFor(thing).MatSingle;
		}

		public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
		{
			Graphic graphic = ((thing == null) ? subGraphics[0] : SubGraphicFor(thing));
			graphic.DrawWorker(loc, rot, thingDef, thing, extraRotation);
			if (base.ShadowGraphic != null)
			{
				base.ShadowGraphic.DrawWorker(loc, rot, thingDef, thing, extraRotation);
			}
		}

		public new Graphic SubGraphicFor(Thing thing)
		{
			if (thing == null)
			{
				return subGraphics[0];
			}

			var keyFrames = Data.KeyFrameAligned;
			var currentFrame = CurrentFrame;
			for (int i = 0; i < keyFrames.Count; ++i)
            {
				if (currentFrame >= keyFrames[i].frame) { return subGraphics[keyFrames[i].index]; }
            }

			return subGraphics[0];
		}

		public override void Print(SectionLayer layer, Thing thing, float extraRotation)
		{
			Graphic graphic = ((thing == null) ? subGraphics[0] : SubGraphicFor(thing));
			graphic.Print(layer, thing, extraRotation);
			if (base.ShadowGraphic != null && thing != null)
			{
				base.ShadowGraphic.Print(layer, thing, extraRotation);
			}
		}

		public override string ToString()
		{
			return "Sprite(path=" + path + ", count=" + subGraphics.Length + ")";
		}
	}
}
