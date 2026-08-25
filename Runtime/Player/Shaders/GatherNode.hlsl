#ifndef GATHER_NODE_INCLUDED
#define GATHER_NODE_INCLUDED

// ==========================================
// 1. Vertex Deformation Logic
// ==========================================
// Call this function in the Vertex stage of the Shader Graph.
void GatherVertex_float(
    float3 OriginalPositionOS,
    float ScanY,
    float FadeRange,
    float SpiralIntensity,
    float ExpandScale,
    out float3 ModifiedPositionOS
) {
    // Reconstruct world position and object center
    float3 worldPos = TransformObjectToWorld(OriginalPositionOS);
    float3 worldCenter = float3(UNITY_MATRIX_M[0][3], UNITY_MATRIX_M[1][3], UNITY_MATRIX_M[2][3]);
    
    // Relative world position
    float3 relPos = worldPos - worldCenter;

    // Fade factor based on Y height
    float factor = saturate((worldPos.y - ScanY) / FadeRange);

    // Spiral rotation
    float rotation = factor * SpiralIntensity * PI;
    float cosA = cos(rotation);
    float sinA = sin(rotation);
    float2x2 rot = float2x2(cosA, -sinA, sinA, cosA);
    relPos.xz = mul(rot, relPos.xz);

    // Expansion
    float expandScaleVal = 1.0 + factor * ExpandScale;
    relPos.xz *= expandScaleVal;

    // Convert back to Object Space
    float3 finalWorldPos = worldCenter + relPos;
    ModifiedPositionOS = TransformWorldToObject(finalWorldPos);
}

// ==========================================
// 2. Fragment Dissolve & Glow Logic
// ==========================================
// Call this function in the Fragment stage of the Shader Graph.
void GatherFragment_float(
    float3 OriginalPositionOS,
    float3 OriginalNormalOS,
    float ScanY,
    float FadeRange,
    float NoiseTiling,
    float BaseGrainPixelSize,
    float EdgeWidth,
    float3 EdgeColor,
    UnityTexture2D NoiseTex,
    UnitySamplerState Sampler_NoiseTex,
    out float ClipMask,
    out float3 EdgeGlowEmission
) {
    // Reconstruct original world position and center (using the ORIGINAL position to avoid swimming noise)
    float3 worldPos = TransformObjectToWorld(OriginalPositionOS);
    float3 worldCenter = float3(UNITY_MATRIX_M[0][3], UNITY_MATRIX_M[1][3], UNITY_MATRIX_M[2][3]);
    float3 relWorldPos = worldPos - worldCenter;

    // World normal for Triplanar Blending
    float3 worldNormal = TransformObjectToWorldNormal(OriginalNormalOS);
    float3 blending = abs(worldNormal);
    blending /= (blending.x + blending.y + blending.z + 1e-5);

    // Distance Factor
    float distFactor = saturate((worldPos.y - ScanY) / FadeRange);

    // View Depth for dynamic tiling (Compensation for resolution/distance)
    float3 viewPos = TransformWorldToView(worldPos);
    float viewDepth = -viewPos.z;

    // GPU dynamic pixel scaling compensation (using abs for projection matrix stability)
    float pixelsPerUnit = (1080.0 * abs(UNITY_MATRIX_P[1][1])) * 0.5 / max(0.01, viewDepth);
    float dynamicTiling = clamp(pixelsPerUnit / BaseGrainPixelSize, 0.0001, 100.0);
    float finalTiling = NoiseTiling * dynamicTiling;

    // Triplanar Noise Sampling
    float noiseX = SAMPLE_TEXTURE2D(NoiseTex.tex, Sampler_NoiseTex.samplerstate, relWorldPos.yz * finalTiling).r;
    float noiseY = SAMPLE_TEXTURE2D(NoiseTex.tex, Sampler_NoiseTex.samplerstate, relWorldPos.xz * finalTiling).r;
    float noiseZ = SAMPLE_TEXTURE2D(NoiseTex.tex, Sampler_NoiseTex.samplerstate, relWorldPos.xy * finalTiling).r;
    float noiseVal = noiseX * blending.x + noiseY * blending.y + noiseZ * blending.z;

    // Clip Threshold
    float dissolveStep = 1.1 + EdgeWidth;
    float currentThreshold = noiseVal - (distFactor * dissolveStep);
    
    // Output 0 for discard, 1 for keep (to be plugged into Alpha / Alpha Clip Threshold)
    ClipMask = currentThreshold < 0.0 ? 0.0 : 1.0;

    // Edge Glow
    float edge = 1.0 - saturate(currentThreshold / (EdgeWidth + 1e-5));
    float edgeWeight = saturate(distFactor * 10.0) * saturate((1.0 - distFactor) * 20.0);
    float edgeGlowAmount = edge * edgeWeight;

    // Edge glow color multiplier (add this to existing Emission)
    EdgeGlowEmission = EdgeColor * edgeGlowAmount * 5.0;
}

#endif
