using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(RenderWithMaterialRenderer), PostProcessEvent.AfterStack, "Custom/Render With Material 2")]
public class RenderWithMaterial2 : RenderWithMaterial
{

    
}