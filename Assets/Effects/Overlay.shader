Shader "Hidden/Custom/Overlay"
{

    SubShader
    {
        Cull Off 
        ZWrite Off 
        ZTest Always

        Pass
        {
            HLSLINCLUDE

            #include "PostProcessing/Shaders/StdLib.hlsl"
            
            TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
            TEXTURE2D_SAMPLER2D(_OverlayTex, sampler_OverlayTex);
            float _Blend;
    
            float4 _DisplacementUVTransform;
            
            float4 Frag(VaryingsDefault i) : SV_Target
            {
                float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
                
                float2 overlayTexcoord = i.texcoord * _DisplacementUVTransform.xy + _DisplacementUVTransform.zw;
                float4 overlayColor = SAMPLE_TEXTURE2D(_OverlayTex, sampler_OverlayTex, overlayTexcoord);
                
                color.rgb = lerp(color.rgb, overlayColor.xxx, _Blend.xxx);
                return color;
            }
    
            ENDHLSL 

            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}
