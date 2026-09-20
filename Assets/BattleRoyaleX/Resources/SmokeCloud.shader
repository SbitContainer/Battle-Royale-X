Shader "BattleRoyaleX/SmokeCloud"
{
    Properties { _Opacity ("Opacity", Range(0,1)) = 1 }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            CBUFFER_START(UnityPerMaterial)
                float _Opacity;
            CBUFFER_END
            struct Attributes { float4 positionOS:POSITION; float2 uv:TEXCOORD0; half4 color:COLOR; };
            struct Varyings { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; half4 color:COLOR; };
            Varyings vert(Attributes v) { Varyings o; o.positionCS=TransformObjectToHClip(v.positionOS.xyz); o.uv=v.uv; o.color=v.color; return o; }
            half4 frag(Varyings i):SV_Target
            {
                float2 p=i.uv*2-1;
                float edge=saturate(1-dot(p,p));
                float cloud=0.80+0.12*sin(p.x*11+p.y*7)+0.08*cos(p.y*15-p.x*4);
                return half4(i.color.rgb*cloud,i.color.a*edge*edge*_Opacity);
            }
            ENDHLSL
        }
    }
}
