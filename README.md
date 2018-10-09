
# Sevakon
[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/uTljPs-2CKk/0.jpg)](https://www.youtube.com/watch?v=uTljPs-2CKk)

Game prototype to experment with some of the newer Unity features:

 - Lightweight render pipeline
 - Postprocessing stack and custom effects
 - The new Input System
 - ECS/Job System/Burst compiler
 - ShaderGraph 

Name was generated with a random game name generator.
Unity 2018.3.0b2

Original sound track [here](https://soundcloud.com/ibicha/dafuq)
Paper texture taken from [here](https://github.com/keijiro/SketchyFx/blob/master/Assets/Textures/OTF_Crumpled_Paper_08.jpg)

## Controls
Use `WASD/Arrow keys` to move, use mouse to point, left click to shoot glowy cubes

## Known issues

 - WebGL build [issues](https://issuetracker.unity3d.com/issues/lwrp-template-scene-is-not-rendered-in-webgl-when-built-with-lightweight-rp-template)
 - Input system [issues](https://github.com/Unity-Technologies/InputSystem/issues/253)
 - The input system fails on first domain load (and also interferes with the job system) making a build not usable (no input, and objects are not animated with music), and only playable in the editor
