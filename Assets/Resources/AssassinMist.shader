Shader "BattleRoyaleX/Assassin Mist"
{
    Properties
    {
        [MainColor] _Color ("Tint", Color) = (1,1,1,1)
        _Opacity ("Opacity", Float) = 1
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
            struct Attributes { float4 positionOS:POSITION; float4 color:COLOR; float2 uv:TEXCOORD0; };
            struct Varyings { float4 positionCS:SV_POSITION; float4 color:COLOR; float2 uv:TEXCOORD0; float3 positionWS:TEXCOORD1; };
            CBUFFER_START(UnityPerMaterial)
                float4 _Color; float _Opacity;
            CBUFFER_END
            Varyings Vert(Attributes v)
            { Varyings o; o.positionCS=TransformObjectToHClip(v.positionOS.xyz); o.positionWS=TransformObjectToWorld(v.positionOS.xyz); o.color=v.color*_Color; o.uv=v.uv; return o; }
            float Hash(float2 p)
            { float3 q=frac(float3(p.xyx)*.1031); q+=dot(q,q.yzx+33.33); return frac((q.x+q.y)*q.z); }
            float Noise(float2 p)
            {
                float2 i=floor(p), f=frac(p); f=f*f*(3-2*f);
                return lerp(lerp(Hash(i),Hash(i+float2(1,0)),f.x),lerp(Hash(i+float2(0,1)),Hash(i+1),f.x),f.y);
            }
            half4 Frag(Varyings i):SV_Target
            {
                float2 p=(i.uv-.5)*2, drift=float2(_Time.y*.08,-_Time.y*.12);
                float n=Noise(p*3.8+drift), detail=Noise(p*8-drift*.7);
                float edge=1-smoothstep(.42,1,length(p)+(detail-.5)*.16);
                float density=edge*(.44+n*.45+detail*.11);
                float light=.48+n*.65+detail*.16;
                // Flat arena floor is y=0; feather the billboard intersection without enabling depth-texture dependencies.
                float groundFade=smoothstep(.06,.5,i.positionWS.y);
                return half4(i.color.rgb*light,i.color.a*density*_Opacity*groundFade);
            }
            ENDHLSL
        }
    }
}
