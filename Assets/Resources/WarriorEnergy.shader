Shader "BattleRoyaleX/Warrior Energy"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1)
        _Mode ("Ribbon or shield", Float) = 0
        _Opacity ("Opacity", Float) = 1
        _VertexAlphaGain ("Vertex alpha gain", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Destination blend", Float) = 1
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha [_DstBlend]
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes { float4 positionOS:POSITION; float3 normalOS:NORMAL; float2 uv:TEXCOORD0; float4 color:COLOR; };
            struct Varyings { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; float3 normalWS:TEXCOORD1; float3 positionWS:TEXCOORD2; };
            CBUFFER_START(UnityPerMaterial)
                float4 _Color; float _Mode, _Opacity, _VertexAlphaGain;
            CBUFFER_END
            Varyings Vert(Attributes v)
            {
                Varyings o; o.positionCS=TransformObjectToHClip(v.positionOS.xyz);
                o.positionWS=TransformObjectToWorld(v.positionOS.xyz); o.normalWS=TransformObjectToWorldNormal(v.normalOS);
                o.uv=v.uv; o.color=v.color*_Color; return o;
            }
            half4 Frag(Varyings i):SV_Target
            {
                float t=_Time.y;
                if(_Mode>0.5)
                {
                    float2 p=i.uv*float2(8,6), cell=float2(1,1.73205);
                    float2 a=frac(p/cell)*cell-cell*.5, b=frac((p-cell*.5)/cell)*cell-cell*.5;
                    float2 q=dot(a,a)<dot(b,b)?a:b;
                    float hex=max(abs(q.x),dot(abs(q),float2(.5,.866025)));
                    float grid=1-smoothstep(.012,.04,abs(hex-.5));
                    float rim=pow(1-saturate(abs(dot(normalize(i.normalWS),normalize(GetWorldSpaceViewDir(i.positionWS))))),2);
                    float sweep=pow(saturate(sin(i.uv.y*9-t*2)),16);
                    float3 blue=float3(.13,.56,1.1), gold=float3(1.1,.71,.23);
                    float border=1-smoothstep(.01,.065,min(min(i.uv.x,1-i.uv.x),min(i.uv.y,1-i.uv.y)));
                    return half4(lerp(blue,gold,border)*(.7+grid*.65+rim*.5),
                        (.07+grid*.24+rim*.27+border*.35+sweep*.1)*_Opacity*saturate(i.color.a*_VertexAlphaGain));
                }
                float across=abs(i.uv.y*2-1);
                float feather=pow(saturate(1-across),1.6);
                float endFade=smoothstep(0,.025,i.uv.x)*(1-smoothstep(.93,1,i.uv.x));
                float filaments=.8+.2*sin(i.uv.x*43-t*8+sin(i.uv.x*13+t)*2);
                return half4(i.color.rgb*(1.25+pow(saturate(1-across),5)*.8),
                    i.color.a*feather*endFade*filaments*_Opacity);
            }
            ENDHLSL
        }
    }
}
