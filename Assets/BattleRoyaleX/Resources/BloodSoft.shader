Shader "BattleRoyaleX/BloodSoft"
{
    Properties { _Shape ("Shape: droplet or ribbon", Float) = 0 }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
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

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.color = input.color;
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input):SV_Target
            {
                float2 coord = input.uv * 2 - 1;
                float droplet = smoothstep(1, 0.12, length(coord));
                float ribbon = pow(saturate(1 - abs(coord.y)), 1.5) *
                    smoothstep(0, 0.05, input.uv.x) * (1 - smoothstep(0.95, 1, input.uv.x));
                float alpha = input.color.a * (_Shape > 0.5 ? ribbon : droplet);
                return half4(saturate(input.color.rgb * 1.25), alpha);
            }
            ENDHLSL
        }
    }
}
