namespace AnimChainsGenerator
{
    public class AnimDef
    {
        public ushort CellXstartIndex { get; set; } = 1;
        public ushort CellYstartIndex { get; set; } = 1;
        public ushort FramesPerRotation { get; set; } = 1;
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

        public static AnimDef Clone(AnimDef animDef)
        {
            return new AnimDef(animDef.CellXstartIndex, animDef.CellYstartIndex, animDef.FramesPerRotation, animDef.AnimName);
        }
    }
}
