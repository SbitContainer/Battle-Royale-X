Shader "BattleRoyaleX/ArenaStone"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            struct A { float4 positionOS:POSITION; };
            struct V { float4 positionCS:SV_POSITION; float3 world:TEXCOORD0; };
            V vert(A v) { V o; o.world=TransformObjectToWorld(v.positionOS.xyz); o.positionCS=TransformWorldToHClip(o.world); return o; }
            half4 frag(V i):SV_Target
            {
                float2 cell=i.world.xz/2.2; float2 uv=frac(cell);
                float edge=min(min(uv.x,1-uv.x),min(uv.y,1-uv.y));
                float aa=max(fwidth(cell.x),fwidth(cell.y));
                float seam=smoothstep(0.008,0.008+aa,edge);
                float noise=frac(sin(dot(floor(cell),float2(12.9898,78.233)))*43758.5453);
                half3 baseColor=lerp(half3(0.06,0.08,0.11),half3(0.14,0.19,0.24),seam)*(0.9+noise*0.18);
                float ring=1-smoothstep(0.04,0.04+aa,abs(length(i.world.xz)-9));
                baseColor+=ring*half3(0.05,0.12,0.16);
                Light light=GetMainLight(TransformWorldToShadowCoord(i.world));
                half lighting=0.45+saturate(light.direction.y)*light.shadowAttenuation*0.8;
                return half4(baseColor*lighting,1);
            }
            ENDHLSL
        }
    }
}
