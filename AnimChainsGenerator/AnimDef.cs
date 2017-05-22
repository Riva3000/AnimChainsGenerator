using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimChainsGenerator
{
    public struct AnimDef
    {
        public ushort CellXstartIndex;
        public ushort CellYstartIndex;
        public ushort FramesPerRotation;
        public string AnimName;

        public AnimDef(ushort cellXstartIndex, ushort cellYstartIndex, ushort framesPerRotation, string animName = null)
        {
            CellXstartIndex = cellXstartIndex;
            CellYstartIndex = cellYstartIndex;
            FramesPerRotation = framesPerRotation;
            AnimName = animName;
        }
    }
}
