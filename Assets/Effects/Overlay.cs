using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Random = UnityEngine.Random;

[Serializable]
[PostProcess(typeof(OverlayRenderer), PostProcessEvent.AfterStack, "Custom/Overlay")]
public sealed class Overlay : PostProcessEffectSettings
{
    [Tooltip("Overlay texture.")] 
    public TextureParameter overlayTexture = new TextureParameter();

    [Range(0f, 1f), Tooltip("Overlay effect intensity.")]
    public FloatParameter intensity = new FloatParameter {value = 0.5f};

    [Range(0f, 1f), Tooltip("Displacement.")]
    public FloatParameter displacement = new FloatParameter {value = 0f};
    
    [Range(0.01f, 1f), Tooltip("Offset interval.")] 
    public FloatParameter displacementSpeed = new FloatParameter {value = 0.5f};

}
 
public sealed class OverlayRenderer : PostProcessEffectRenderer<Overlay>
{
    private float lastRenderTime;
    private Shader shader;
    
    public override void Init()
    {
        base.Init();
        shader = Shader.Find("Hidden/Custom/Overlay");
    }

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(shader);
        sheet.properties.SetFloat("_Blend", settings.intensity);
        sheet.properties.SetTexture("_OverlayTex", settings.overlayTexture);

        if (Time.time - lastRenderTime > settings.displacementSpeed)
        {
            var vector = new Vector4(
                1f - settings.displacement.value,
                1f - settings.displacement.value,
                Random.Range(0f, 1f) * settings.displacement.value,
                Random.Range(0f, 1f) * settings.displacement.value
            );
            sheet.properties.SetVector("_DisplacementUVTransform", vector);
            lastRenderTime = Time.time;
        }

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}