using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AutomataRace
{
    public class SpriteKeyFrame
    {
        public int frame;
        public int index;
    }

    public class GraphicData_Sprite : GraphicData
    {
        public List<SpriteKeyFrame> KeyFrameAligned
        {
            get
            {
                if (_keyFrameAligned == null)
                {
                    _keyFrameAligned = keyFrames.OrderByDescending(v => v.frame).ToList();
                }

                return _keyFrameAligned;
            }
        }
        private List<SpriteKeyFrame> _keyFrameAligned;

        public int totalFrameLength;
        public List<SpriteKeyFrame> keyFrames;
    }
}
