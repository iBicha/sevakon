using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(RenderWithMaterialRenderer), PostProcessEvent.AfterStack, "Custom/Render With Material 3")]
public class RenderWithMaterial3 : RenderWithMaterial
{

    
}