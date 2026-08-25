Shader "UI/UIImageSpaceGather"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        [HideInInspector] _ScanY ("Scan Y", Float) = 0
        [HideInInspector] _FadeRange ("Fade Range", Float) = 1.0     
        [HideInInspector] _NoiseTiling ("Noise Tiling", Float) = 1.0
        [HideInInspector] _BaseGrainPixelSize ("Base Grain Pixel Size", Float) = 50.0
        [HideInInspector] _NoiseTex ("Noise Texture", 2D) = "white" {}
        [HideInInspector] _IsScreenSpace ("Is Screen Space", Float) = 0
        _SpiralIntensity ("Spiral Intensity", Float) = 2.0
        
        [HDR] _EdgeColor ("Edge Color", Color) = (0, 1, 1, 1) 
        _EdgeWidth ("Edge Width", Range(0, 0.5)) = 0.1

        _GridScale ("Grid Scale", Float) = 5.0
        _GridSpeed ("Grid Speed", Float) = 2.0
        _ExpandScale ("Expand Scale", Float) = 2.0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [ZTest]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            // UI specific utilities (Manual reproduction of necessary parts)
            float UnityGet2DClipping (float2 position, float4 clipRect)
            {
                float2 inside = step(clipRect.xy, position.xy) * step(position.xy, clipRect.zw);
                return inside.x * inside.y;
            }

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 color        : COLOR;
                float2 uv           : TEXCOORD0;
                float3 normalOS     : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                half4 color         : COLOR;
                float2 uv           : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float distFactor : TEXCOORD3;
                float3 worldNormal : TEXCOORD4;
                float3 localPos : TEXCOORD5;
                float3 relWorldPos : TEXCOORD6; 
                float viewDepth : TEXCOORD7;
                float4 screenPos : TEXCOORD8;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _TextureSampleAdd;
                float4 _ClipRect;
                float4 _MainTex_ST;
                float _ScanY;
                float _FadeRange;
                float _SpiralIntensity;
                float4 _EdgeColor;
                float _EdgeWidth;
                float _GridScale;
                float _GridSpeed;
                float _ExpandScale;
                float _NoiseTiling;
                float _BaseGrainPixelSize;
                float _IsScreenSpace;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.localPos = input.positionOS.xyz;
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                output.worldPosition = float4(worldPos, 1.0);
                
                float3 worldCenter = float3(UNITY_MATRIX_M[0][3], UNITY_MATRIX_M[1][3], UNITY_MATRIX_M[2][3]);
                float3 relPos = worldPos - worldCenter;
                output.relWorldPos = relPos;

                float factor = saturate((worldPos.y - _ScanY) / _FadeRange);
                output.distFactor = factor;

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
                output.color = input.color * _Color;
                output.worldNormal = TransformObjectToWorldNormal(input.normalOS);

                float3 viewPos = TransformWorldToView(finalWorldPos);
                output.viewDepth = -viewPos.z;

                output.screenPos = ComputeScreenPos(output.positionCS);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half4 col = (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) + _TextureSampleAdd) * input.color;
                float noiseVal = 0;

                if (_IsScreenSpace > 0.5)
                {
                    float resScale = _ScreenParams.y / 1080.0;
                    float2 screenPx = input.screenPos.xy / input.screenPos.w * _ScreenParams.xy;
                    noiseVal = SAMPLE_TEXTURE2D_LOD(_NoiseTex, sampler_NoiseTex, screenPx / (_BaseGrainPixelSize * max(resScale, 0.1)), 0).r;
                }
                else
                {
                    float pixelsPerUnit = (1080.0 * abs(UNITY_MATRIX_P[1][1])) * 0.5 / max(0.01, input.viewDepth);
                    float dynamicTiling = clamp(pixelsPerUnit / _BaseGrainPixelSize, 0.0001, 100.0);
                    float finalTiling = _NoiseTiling * dynamicTiling;

                    float noiseA = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, input.relWorldPos.xy * finalTiling).r;
                    float noiseB = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, input.relWorldPos.yz * finalTiling).r;
                    noiseVal = noiseA * 0.75 + noiseB * 0.25;
                }

                float dissolveStep = 1.1 + _EdgeWidth;
                float currentThreshold = noiseVal - (input.distFactor * dissolveStep);
                clip(currentThreshold);

                float2 gridPos = input.uv * _GridScale * 10.0 - _Time.y * _GridSpeed;
                float gridX = abs(frac(gridPos.x) - 0.5) * 2.0; 
                float gridZ = abs(frac(gridPos.y) - 0.5) * 2.0; 
                float gridLine = max(smoothstep(0.8, 1.0, gridX), smoothstep(0.8, 1.0, gridZ));

                float edge = 1.0 - saturate(currentThreshold / (_EdgeWidth + 1e-5));
                float edgeWeight = saturate(input.distFactor * 10.0) * saturate((1.0 - input.distFactor) * 20.0);
                float edgeGlow = edge * edgeWeight;

                half3 glowCol = _EdgeColor.rgb * (1.0 + gridLine * 2.0);
                col.rgb = lerp(col.rgb, glowCol, edgeGlow);
                col.rgb += edgeGlow * glowCol * 3.0 * col.a;

                #ifdef UNITY_UI_CLIP_RECT
                col.a *= UnityGet2DClipping(input.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (col.a - 0.001);
                #endif

                return col;
            }
            ENDHLSL
        }
    }
}
