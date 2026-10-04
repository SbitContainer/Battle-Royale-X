Shader "BattleRoyaleX/Combat FX"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        _Mode ("Mode", Float) = 0
        _Intensity ("Intensity", Float) = 1
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
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST, _Tint;
                float _Mode, _Intensity;
            CBUFFER_END
            struct Attributes { float4 positionOS:POSITION; float3 normalOS:NORMAL; float2 uv:TEXCOORD0; float4 color:COLOR; };
            struct Varyings { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; float4 color:COLOR; float3 normalWS:TEXCOORD1; float3 positionWS:TEXCOORD2; };
            Varyings Vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); o.color = v.color;
                return o;
            }
            float Hash(float2 p) { return frac(sin(dot(p,float2(127.1,311.7)))*43758.5453); }
            float Noise(float2 p)
            {
                float2 i=floor(p), f=frac(p); f=f*f*(3-2*f);
                return lerp(lerp(Hash(i),Hash(i+float2(1,0)),f.x),lerp(Hash(i+float2(0,1)),Hash(i+1),f.x),f.y);
            }
            half4 Frag(Varyings i):SV_Target
            {
                float4 c = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.uv) * _Tint * i.color;
                float t=_Time.y;
                if (_Mode>1.5 && _Mode<2.5)
                {
                    float2 p=(i.uv-0.5)*2;
                    float r=length(p);
                    float n=Noise(p*6+float2(t*0.19,-t*0.13));
                    float n2=Noise(p*13-float2(t*0.11,t*0.22));
                    float ripple=pow(saturate(sin(r*28-t*3+n*5)),8);
                    float edge=1-smoothstep(0.86,1.0,r+n2*0.05);
                    c.rgb=lerp(float3(0.015,0.07,0.04),float3(0.13,0.5,0.23),n*n)*_Intensity;
                    c.rgb+=float3(0.08,0.45,0.22)*ripple*0.35;
                    c.a=edge*(0.67+n2*0.15)*_Tint.a;
                    return c;
                }
                if (_Mode>0.5)
                {
                    float rim=pow(1-saturate(abs(dot(normalize(i.normalWS),normalize(GetWorldSpaceViewDir(i.positionWS))))),2);
                    float bands=pow(saturate(sin(i.uv.y*30-t*5+Noise(i.uv*8)*4)),6);
                    c.rgb=_Tint.rgb*(0.4+rim*2.2+bands*0.8);
                    c.a=_Tint.a*(_Mode>2.5 ? 0.18+rim*0.7 : 0.8);
                }
                c.rgb*=_Intensity;
                return c;
            }
            ENDHLSL
        }
    }
}
