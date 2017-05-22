using FlatRedBall.Content.AnimationChain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimChainsGenerator
{
    public sealed class Generator
    {
        private static Size _FrameSize;
        private static string _SpriteSheetFileName;
        private static ushort _Rotations;

        public static AnimationChainListSave Generate(
            Size frameSize,
            string spriteSheetFileName,
            ushort rotations,
            IEnumerable<AnimDef> animsDefinitions
        )
        {
            _FrameSize = frameSize;
            _SpriteSheetFileName = spriteSheetFileName;
            _Rotations = rotations;

            AnimationChainListSave animChainListSave = new AnimationChainListSave
            {
                CoordinateType = FlatRedBall.Graphics.TextureCoordinateType.Pixel,
                FileRelativeTextures = true,
                TimeMeasurementUnit = FlatRedBall.TimeMeasurementUnit.Second
            };

            foreach (var animDef in animsDefinitions)
            {
                animChainListSave.AnimationChains.AddRange(
                    GenerateAnimChain(animDef)
                );
            }

            return animChainListSave;
        }

        private static AnimationChainSave[] GenerateAnimChain(AnimDef animDef)
        {
            // * frame coordinates are in pixels. top-left = 0,0  bottom-right = +X +Y

            float xStart = animDef.CellXstartIndex * _FrameSize.Width;
            float yStart = animDef.CellYstartIndex * _FrameSize.Height;

            AnimationChainSave[] animsList = new AnimationChainSave[_Rotations];

            //float currentXStart;
            float currentYTop;
            float currentYBottom;
            float left;
            AnimationChainSave oneRotAnim;
            AnimationFrameSave frame;
            for (ushort iRotation = 0; iRotation < 16; iRotation++)
            {
                currentYTop = yStart + iRotation * _FrameSize.Height;
                currentYBottom = currentYTop + _FrameSize.Height;
                oneRotAnim = new AnimationChainSave(); // animDef.FramesPerRotation
                oneRotAnim.Name = animDef.AnimName + '_' + iRotation.ToString();

                for (ushort iFrame = 0; iFrame < animDef.FramesPerRotation; iFrame++)
                {
                    left = xStart + iFrame * _FrameSize.Width;

                    frame = new AnimationFrameSave // (_FramesTexture, 0)
                    {
                        TextureName = _SpriteSheetFileName,
                        TopCoordinate = currentYTop,
                        BottomCoordinate = currentYBottom,
                        LeftCoordinate = left,
                        RightCoordinate = left + _FrameSize.Width,
                        FrameLength = 0.1f
                    };

                    //oneRotAnim.Add(frame);
                    oneRotAnim.Frames.Add(frame);
                }

                //animsList.Add(oneRotAnim);
                animsList[iRotation] = oneRotAnim;
            }

            return animsList;
        }

        public static void SaveAchx(AnimationChainListSave animChainListSave, string path, string achxFileName, string spriteSheetLocation)
        {
            if ( ! File.Exists( Path.Combine(path, _SpriteSheetFileName) ) )
            {
                File.Copy(
                    Path.Combine(spriteSheetLocation, _SpriteSheetFileName),
                    Path.Combine(path, _SpriteSheetFileName)
                );
            }

            animChainListSave.Save( Path.Combine(path, achxFileName + ".achx") );
        }
    }
}
