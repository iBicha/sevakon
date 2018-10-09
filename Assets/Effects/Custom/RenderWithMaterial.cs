using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(RenderWithMaterialRenderer), PostProcessEvent.BeforeStack, "Custom/Render With Material")]
public class RenderWithMaterial : PostProcessEffectSettings
{
    [Tooltip("Material used for rendering.")]
    public MaterialParameter material = new MaterialParameter();
}

public class RenderWithMaterialRenderer : PostProcessEffectRenderer<RenderWithMaterial>
{
    private static int _MainTex = Shader.PropertyToID("_MainTex");

    public override void Render(PostProcessRenderContext context)
    {
        Blit(context.command, context.source, context.destination, settings.material);
    }

    private static void Blit(CommandBuffer command, RenderTargetIdentifier source, RenderTargetIdentifier destination,
        Material material, int pass = -1)
    {
        if (material == null)
        {
            command.BuiltinBlit(source, destination);
            return;
        }

        command.SetGlobalTexture(_MainTex, source);
        command.SetRenderTargetWithLoadStoreAction(destination, RenderBufferLoadAction.DontCare,
            RenderBufferStoreAction.Store);

        command.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, material, 0, pass);
    }
}