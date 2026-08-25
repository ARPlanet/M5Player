Shader "Custom/glTFUnlitGather"
{
    Properties
    {
        baseColorFactor ("Base Color", Color) = (1,1,1,1)
        baseColorTexture ("Base Color Texture", 2D) = "white" {}
        [HideInInspector] _ScanY("Scan Y", Float) = 0
        [HideInInspector] _FadeRange("Fade Range", Float) = 1.0
        [HideInInspector] _NoiseTiling("Noise Tiling", Float) = 1.0
        [HideInInspector] _BaseGrainPixelSize ("Base Grain Pixel Size", Float) = 50.0
        [HideInInspector] _NoiseTex("Noise Texture", 2D) = "white" {}
        [HideInInspector] _ExpandScale("Expand Scale", Float) = 2.0
        [HideInInspector] _SpiralIntensity("Spiral Intensity", Float) = 2.0
        [HideInInspector] _EdgeColor ("Edge Color", Color) = (0, 1, 1, 1)
        [HideInInspector] _EdgeWidth ("Edge Width", Range(0, 0.5)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float distFactor : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
                float3 relWorldPos : TEXCOORD4;
                float viewDepth : TEXCOORD5;
            };

            sampler2D baseColorTexture;
            float4 baseColorTexture_ST;
            fixed4 baseColorFactor;

            sampler2D _NoiseTex;
            float _NoiseTiling;
            float _BaseGrainPixelSize;
            float _ScanY;
            float _FadeRange;
            float _ExpandScale;
            float _SpiralIntensity;
            float4 _EdgeColor;
            float _EdgeWidth;

            v2f vert (appdata v)
            {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = worldPos.xyz;
                
                float3 worldCenter = float3(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3]);
                float3 relPos = worldPos.xyz - worldCenter;
                o.relWorldPos = relPos;

                float factor = saturate((worldPos.y - _ScanY) / _FadeRange);
                o.distFactor = factor;

                float rotation = factor * _SpiralIntensity * UNITY_PI;
                float cosA = cos(rotation);
                float sinA = sin(rotation);
                float2x2 rot = float2x2(cosA, -sinA, sinA, cosA);
                relPos.xz = mul(rot, relPos.xz);

                float expandScale = 1.0 + factor * _ExpandScale;
                relPos.xz *= expandScale;

                float3 finalWorldPos = worldCenter + relPos;
                o.vertex = mul(UNITY_MATRIX_VP, float4(finalWorldPos, 1.0));
                
                o.uv = TRANSFORM_TEX(v.uv, baseColorTexture);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                // 計算視距深度
                float3 viewPos = UnityObjectToViewPos(v.vertex);
                o.viewDepth = -viewPos.z;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 blending = abs(i.worldNormal);
                blending /= (blending.x + blending.y + blending.z + 1e-5);
                
                // --- GPU 動態像素縮放補償 ---
                // 將 _ScreenParams.y 替換為固定的 1080.0，以破除平台實際解析度造成的影響
                // 這樣可以保證 3D 物件的消融顆粒在任何解析度（如 4K 手機或 720p 螢幕）下看起來比例都一模一樣
                float pixelsPerUnit = (1080.0 * unity_CameraProjection[1][1]) * 0.5 / max(0.01, i.viewDepth);
                float dynamicTiling = clamp(pixelsPerUnit / _BaseGrainPixelSize, 0.0001, 100.0);
                float finalTiling = _NoiseTiling * dynamicTiling;

                float noiseX = tex2D(_NoiseTex, i.relWorldPos.yz * finalTiling).r;
                float noiseY = tex2D(_NoiseTex, i.relWorldPos.xz * finalTiling).r;
                float noiseZ = tex2D(_NoiseTex, i.relWorldPos.xy * finalTiling).r;
                float noiseVal = noiseX * blending.x + noiseY * blending.y + noiseZ * blending.z;

                // --- 統一消融公式 ---
                float dissolveStep = 1.1 + _EdgeWidth;
                float currentThreshold = noiseVal - (i.distFactor * dissolveStep);
                clip(currentThreshold);

                fixed4 col = tex2D(baseColorTexture, i.uv) * baseColorFactor;
                
                // 邊緣發光
                float edge = 1.0 - saturate(currentThreshold / (_EdgeWidth + 1e-5));
                float edgeWeight = saturate(i.distFactor * 10.0) * saturate((1.0 - i.distFactor) * 20.0);
                float edgeGlow = edge * edgeWeight;

                // 強制覆蓋底色 + 加法亮度
                col.rgb = lerp(col.rgb, _EdgeColor.rgb, edgeGlow);
                col.rgb += edgeGlow * _EdgeColor.rgb * 5.0;

                return col;
            }
            ENDCG
        }
    }
}
