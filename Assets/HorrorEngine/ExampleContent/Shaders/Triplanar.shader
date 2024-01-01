Shader "HorrorEngine/Tri-Planar World"
{
    Properties
    {
        _Side("Side", 2D) = "white" {}
        _Top("Top", 2D) = "white" {}
        _Bottom("Bottom", 2D) = "white" {}
        _SideScale("Side Scale", Float) = 2
        _TopScale("Top Scale", Float) = 2
        _BottomScale("Bottom Scale", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            float _SideScale, _TopScale, _BottomScale;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.position = TransformObjectToHClip(IN.vertex.xyz);
                OUT.worldPos = TransformObjectToWorld(IN.vertex.xyz);
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normal);
                return OUT;
            }

            TEXTURE2D(_Side);
            TEXTURE2D(_Top);
            TEXTURE2D(_Bottom);
            SAMPLER(sampler_Side);
            SAMPLER(sampler_Top);
            SAMPLER(sampler_Bottom);

            float4 frag(Varyings IN) : SV_Target
            {
                float3 worldNormal = normalize(IN.worldNormal);
                float3 projNormal = saturate(pow(worldNormal * 1.4, 4));

                float3 xColor = SAMPLE_TEXTURE2D(_Side, sampler_Side, frac(IN.worldPos.zy * _SideScale)) * abs(worldNormal.x);
                float3 yColor;
                if (worldNormal.y > 0)
                {
                    yColor = SAMPLE_TEXTURE2D(_Top, sampler_Top, frac(IN.worldPos.zx * _TopScale)) * abs(worldNormal.y);
                }
                else
                {
                    yColor = SAMPLE_TEXTURE2D(_Bottom, sampler_Bottom, frac(IN.worldPos.zx * _BottomScale)) * abs(worldNormal.y);
                }
                float3 zColor = SAMPLE_TEXTURE2D(_Side, sampler_Side, frac(IN.worldPos.xy * _SideScale)) * abs(worldNormal.z);

                float3 albedo = zColor;
                albedo = lerp(albedo, xColor, projNormal.x);
                albedo = lerp(albedo, yColor, projNormal.y);
                
                return float4(albedo, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Forward"
}
