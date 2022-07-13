Shader "Custom/URP_Grass"
{
    Properties
    {
        [MainTexture] _BaseMap("Grass Texture", 2D) = "white" {}
        _DistortionMap ("Wind Distortion Map", 2D) = "white" {}
        [HideInInspector] _TrampleMap("Trample Texture", 2D) = "white" {}
        _WindFrequency ("Wind Frequency", Vector) = (0.05, 0.05, 0, 0)
        _WindStrength ("Wind Strength", Float) = 0.1
        _Height ("Height", Float) = 1.0
        _Base ("Base", Float) = 1.0
        [HDR] _Tint ("Tint", Color) = (0.5, 0.5, 0.5, 1)
        _LightFactor ("Light Factor", Float) = 0.5
        _TranslucentFactor ("Translucent Factor", Float) = 0.2
        _AlphaClip ("Alpha Clip", Float) = 0.5
        [HDR] _BlendColor ("Blend Color", Color) = (0.5, 0.5, 0.5, 1)
        _ShadowFactor ("Shadow Factor", Float) = 0.5
        _MinHeight ("Minimum Height", Float) = 0
        _MaxHeight ("Maximum Height", Float) = 1
        _ReceiveShadows("Receive Shadows", Float) = 1
        _SrcBlend("__src", Float) = 1.0
        _DstBlend("__dst", Float) = 0.0
        _ZWrite("__zw", Float) = 1.0
        _TrampleMultiplier("Trample multiplier", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "IgnoreProjector"="True"}
        LOD 300

        /*Pass
        {
            Name "Depth Pass"
            Tags {"LightMode"="DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull Off

            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma multi_compile _ WRITE_NORMAL_BUFFER
            #pragma multi_compile _ WRITE_MSAA_DEPTH
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma multi_compile_instancing

            #pragma require geometry

            #pragma geometry LitPassGeom
            #pragma vertex LitPassVertex
            #pragma fragment DepthPassFragment

            #define SHADOW

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            #include "TexturePackingUtils.cginc"

            #include "GrassPass.hlsl"

            half4 DepthPassFragment(Varyings input) : SV_TARGET
            {
                return half4(0, 0, 0, 0);
            }

            ENDHLSL
        }*/

        Pass
        {
            Name "Geometry Pass"
            Tags {"RenderType"="Opaque" "LightMode"="UniversalForward"}

            Blend[_SrcBlend][_DstBlend]
            ZWrite [_ZWrite]
            Cull Off

            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 4.0

            #pragma shader_feature _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature _GLOSSYREFLECTIONS_OFF
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _SPECULAR_SETUP
            #pragma shader_feature _RECEIVE_SHADOWS_ON
            //#pragma shader_feature _ _RENDERING_CUTOUT _RENDERING_FADE
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS

            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            #pragma require geometry

            #pragma geometry LitPassGeom
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            #include "TexturePackingUtils.cginc"

            #include "GrassPass.hlsl"

            ENDHLSL
        }
    }
}
