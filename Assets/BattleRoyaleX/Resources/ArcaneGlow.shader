Shader "BattleRoyaleX/ArcaneGlow"
{
    Properties { _Shape ("Shape: glow or ribbon", Float) = 0 }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha One
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes { float4 positionOS:POSITION; float4 color:COLOR; float2 uv:TEXCOORD0; };
            struct Varyings { float4 positionCS:SV_POSITION; half4 color:COLOR; float2 uv:TEXCOORD0; };
            CBUFFER_START(UnityPerMaterial)
            float _Shape;
            CBUFFER_END
            Varyings vert(Attributes v)
            {
                Varyings o; o.positionCS=TransformObjectToHClip(v.positionOS.xyz); o.color=v.color; o.uv=v.uv; return o;
            }
            half4 frag(Varyings i):SV_Target
            {
                float2 p=i.uv*2-1;
                float glow=pow(saturate(1-length(p)), 2.4);
                float ribbon=pow(saturate(1-abs(p.y)),1.7)*smoothstep(0,0.06,i.uv.x)*(1-smoothstep(0.94,1,i.uv.x));
                return half4(i.color.rgb*1.8, i.color.a*(_Shape>0.5?ribbon:glow));
            }
            ENDHLSL
        }
    }
}
