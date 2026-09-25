Shader "BattleRoyaleX/ArcaneSurface"
{
    Properties
    {
        _BaseColor ("Energy", Color) = (0.15,0.65,1,0.35)
        _Rune ("Ground rune", Float) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _Rune;
            CBUFFER_END
            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };
            Varyings Vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }
            half4 Frag(Varyings i) : SV_Target
            {
                float2 p = i.uv * 2 - 1;
                float edge = pow(max(abs(p.x), abs(p.y)), 16);
                float scan = pow(saturate(0.5 + 0.5 * sin(i.uv.y * 75 - _Time.y * 2)), 14);
                float grid = pow(saturate(0.5 + 0.5 * cos(i.uv.x * 62.83)), 24) * scan;
                float strength = 0.09 + edge * 0.7 + scan * 0.1 + grid * 0.4;
                if (_Rune > 0.5)
                {
                    float r = length(p);
                    float ring = 1 - smoothstep(0.014, 0.035, min(abs(r - 0.88), abs(r - 0.66)));
                    float angle = atan2(p.y, p.x);
                    float marks = step(0.86, cos(angle * 12 + _Time.y * 0.3)) *
                        smoothstep(0.69,0.72,r) * (1-smoothstep(0.81,0.84,r));
                    strength = saturate(ring + marks);
                }
                return half4(_BaseColor.rgb * (1 + strength), _BaseColor.a * strength);
            }
            ENDHLSL
        }
    }
}
