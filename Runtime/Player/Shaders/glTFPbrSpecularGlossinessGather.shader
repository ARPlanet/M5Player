Shader "Custom/glTFPbrSpecularGlossinessGather"
{
    Properties
    {
        [MainColor] baseColorFactor ("Diffuse Factor", Color) = (1,1,1,1)
        [MainTexture] _MainTex ("Diffuse Tex", 2D) = "white" {}
        specularGlossinessTexture ("Specular-Glossiness Tex", 2D) = "white" {}
        [HDR] specularFactor ("Specular Factor", Color) = (1,1,1,1)
        glossinessFactor ("Glossiness Factor", Range(0,1)) = 1.0
        [Normal] normalTexture ("Normal Tex", 2D) = "bump" {}
        normalTexture_scale("Normal Scale", Float) = 1.0
        occlusionTexture ("Occlusion Tex", 2D) = "white" {}
        occlusionTexture_strength("Occlusion Strength", Range(0.0, 1.0)) = 1.0
        emissiveTexture ("Emissive Tex", 2D) = "white" {}
        [HDR] emissiveFactor ("Emissive Factor", Color) = (0,0,0,0)

        [HideInInspector] _ScanY("Scan Y", Float) = 0
        [HideInInspector] _FadeRange("Fade Range", Float) = 1.0
        [HideInInspector] _NoiseTiling("Noise Tiling", Float) = 1.0
        [HideInInspector] _BaseGrainPixelSize ("Base Grain Pixel Size", Float) = 50.0
        [HideInInspector] _ExpandScale("Expand Scale", Float) = 2.0
        [HideInInspector] _SpiralIntensity("Spiral Intensity", Float) = 2.0
        [HideInInspector] _NoiseTex("Noise Texture", 2D) = "white" {}
        [HideInInspector] _EdgeColor ("Edge Color", Color) = (0, 1, 1, 1)
        [HideInInspector] _EdgeWidth ("Edge Width", Range(0, 0.5)) = 0.1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }
        LOD 200
        Cull Off

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 data0 : TEXCOORD1; // xyz = relWorldPos, w = distFactor
                float4 data1 : TEXCOORD3; // xyz = worldNormal, w = viewDepth
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 baseColorFactor;
                float4 _MainTex_ST;
                float4 specularFactor;
                float glossinessFactor;
                float normalTexture_scale;
                float occlusionTexture_strength;
                float4 emissiveFactor;

                float _ScanY;
                float _FadeRange;
                float _NoiseTiling;
                float _BaseGrainPixelSize;
                float _ExpandScale;
                float _SpiralIntensity;
                float4 _EdgeColor;
                float _EdgeWidth;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(specularGlossinessTexture);
            SAMPLER(sampler_specularGlossinessTexture);
            TEXTURE2D(normalTexture);
            SAMPLER(sampler_normalTexture);
            TEXTURE2D(occlusionTexture);
            SAMPLER(sampler_occlusionTexture);
            TEXTURE2D(emissiveTexture);
            SAMPLER(sampler_emissiveTexture);
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                float3 worldCenter = float3(UNITY_MATRIX_M[0][3], UNITY_MATRIX_M[1][3], UNITY_MATRIX_M[2][3]);
                float3 relPos = worldPos - worldCenter;

                float factor = saturate((worldPos.y - _ScanY) / _FadeRange);
                output.data0.w = factor;
                output.data0.xyz = relPos; // 設定相對座標作為噪點取樣點 (在變形之前)

                float rotation = factor * _SpiralIntensity * PI;
                float cosA = cos(rotation);
                float sinA = sin(rotation);
                float2x2 rot = float2x2(cosA, -sinA, sinA, cosA);
                relPos.xz = mul(rot, relPos.xz);

                float expandScale = 1.0 + factor * _ExpandScale;
                relPos.xz *= expandScale;

                float3 finalWorldPos = worldCenter + relPos;
                output.positionCS = TransformWorldToHClip(finalWorldPos);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.data1.xyz = TransformObjectToWorldNormal(input.normalOS);
                
                float3 viewPos = TransformWorldToView(finalWorldPos);
                output.data1.w = -viewPos.z;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float3 relWorldPos = input.data0.xyz;
                float distFactor = input.data0.w;
                float3 worldNormal = normalize(input.data1.xyz);
                float3 blending = abs(worldNormal);
                float viewDepth = input.data1.w;
                blending /= (blending.x + blending.y + blending.z + 1e-5);

                float pixelsPerUnit = (1080.0 * abs(UNITY_MATRIX_P[1][1])) * 0.5 / max(0.01, viewDepth);
                float dynamicTiling = clamp(pixelsPerUnit / _BaseGrainPixelSize, 0.0001, 100.0);
                float finalTiling = _NoiseTiling * dynamicTiling;

                float noiseX = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, relWorldPos.yz * finalTiling).r;
                float noiseY = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, relWorldPos.xz * finalTiling).r;
                float noiseZ = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, relWorldPos.xy * finalTiling).r;
                float noiseVal = noiseX * blending.x + noiseY * blending.y + noiseZ * blending.z;

                float dissolveStep = 1.1 + _EdgeWidth;
                float currentThreshold = noiseVal - (distFactor * dissolveStep);
                clip(currentThreshold);

                half4 baseColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * baseColorFactor;

                float edge = 1.0 - saturate(currentThreshold / (_EdgeWidth + 1e-5));
                float edgeWeight = saturate(distFactor * 10.0) * saturate((1.0 - distFactor) * 20.0);
                float edgeGlow = edge * edgeWeight;

                half3 finalColor = lerp(baseColor.rgb, half3(0,0,0), edgeGlow);
                
                half3 emission = SAMPLE_TEXTURE2D(emissiveTexture, sampler_emissiveTexture, input.uv).rgb * emissiveFactor.rgb;
                finalColor += emission + (edgeGlow * _EdgeColor.rgb * 5.0);

                return half4(finalColor, baseColor.a);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
