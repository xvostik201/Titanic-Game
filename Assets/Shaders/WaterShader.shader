Shader "URP/WaterMinimalFoamLit"
{
    Properties
    {
        _BaseColor     ("Base Color", Color) = (0,0.3,0.5,0.7)
        _FoamColor     ("Foam Color", Color) = (1,1,1,1)
        _WaveScale     ("Wave Scale", Float) = 0.12
        _WaveAmp       ("Wave Amplitude", Float) = 0.08
        _WaveSpeed     ("Wave Speed", Float) = 1
        _FoamThreshold ("Foam Threshold", Float) = 0.02
        _FoamDensity   ("Foam Density", Float) = 40
        _SpecPower     ("Spec Power", Float) = 64
        _SpecStrength  ("Spec Strength", Float) = 0.35
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityInput.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _FoamColor;
                float  _WaveScale;
                float  _WaveAmp;
                float  _WaveSpeed;
                float  _FoamThreshold;
                float  _FoamDensity;
                float  _SpecPower;
                float  _SpecStrength;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS  : SV_POSITION;
                float3 worldPos    : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float4 screenPos   : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                float t = _Time.y * _WaveSpeed;
                float w = sin(v.positionOS.x * _WaveScale + t) * _WaveAmp +
                          cos(v.positionOS.z * _WaveScale * 0.8 + t * 0.7) * (_WaveAmp * 0.5);
                float3 posOS = v.positionOS.xyz + float3(0, w, 0);

                VertexPositionInputs vp = GetVertexPositionInputs(posOS);
                o.positionCS  = vp.positionCS;
                o.worldPos    = vp.positionWS;
                o.worldNormal = normalize(TransformObjectToWorldNormal(v.normalOS));
                o.screenPos   = ComputeScreenPos(vp.positionCS);

                #if defined(_MAIN_LIGHT_SHADOWS)
                    o.shadowCoord = TransformWorldToShadowCoord(vp.positionWS);
                #else
                    o.shadowCoord = 0;
                #endif
                return o;
            }

            float4 frag (Varyings i) : SV_Target
            {
                float2 uv = i.screenPos.xy / i.screenPos.w;

                float rawScene  = SampleSceneDepth(uv);
                float sceneLin  = LinearEyeDepth(rawScene, _ZBufferParams);
                float rawWater  = i.screenPos.z / i.screenPos.w;
                float waterLin  = LinearEyeDepth(rawWater, _ZBufferParams);
                float diff      = abs(sceneLin - waterLin);
                float foam      = saturate((_FoamThreshold - diff) * _FoamDensity);

                float3 N = normalize(i.worldNormal);

                Light mainLight;
                #if defined(_MAIN_LIGHT_SHADOWS)
                    mainLight = GetMainLight(i.shadowCoord);
                #else
                    mainLight = GetMainLight();
                #endif

                float3 L = normalize(mainLight.direction);
                float ndotl = saturate(dot(N, L));
                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 H = normalize(L + V);
                float spec = pow(saturate(dot(N, H)), _SpecPower) * _SpecStrength;

                float3 lit = (_BaseColor.rgb * (0.05 + ndotl) + spec) * mainLight.color.rgb;
                float3 rgb = lerp(lit, _FoamColor.rgb, foam);
                return float4(rgb, _BaseColor.a);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
