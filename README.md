Automatic AnimationChains generator for FlatRedBall Game Engine.

For cases when you need to create a lot of animations (AnimationChains) that can be parametrized.  

Saves you a lot of clicking in FRB Anim editor in case you, for xample, have character in your game 
that has "rotations" (walking down, walking left, walking right) that have same amount of animation frames.

 * Made to only work with a single sprite sheet.
 * All frames (cells) of sprite sheet must have uniform size. (You can later pack the spritesheet and achx file with my AnimChainsSheetPacker tool)
 * Requires that the animation frames are layed out in sprite sheet in sorted manner:  
   each row representing a rotation, each column containing same frames in different rotations.
   
---

The tool is divided into a AnimChainsGenerator.dll where all the generation functionality is, and Windows GUI app.
