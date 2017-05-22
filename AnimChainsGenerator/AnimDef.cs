using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimChainsGenerator
{
    public class AnimDef
    {
        public ushort CellXstartIndex { get; set; }
        public ushort CellYstartIndex { get; set; }
        public ushort FramesPerRotation { get; set; }
        public string AnimName { get; set; }

        public AnimDef()
        { }

        public AnimDef(ushort cellXstartIndex, ushort cellYstartIndex, ushort framesPerRotation, string animName = null)
        {
            CellXstartIndex = cellXstartIndex;
            CellYstartIndex = cellYstartIndex;
            FramesPerRotation = framesPerRotation;
            AnimName = animName;
        }
    }
}
