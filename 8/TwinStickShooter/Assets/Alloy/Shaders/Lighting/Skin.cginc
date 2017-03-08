// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Skin.cginc
/// @brief Skin lighting model with SSS & Transmission. Deferred+Forward.
///////////////////////////////////////////////////////////////////////////////

#ifndef A_LIGHTING_STANDARD_SKIN_CGINC
#define A_LIGHTING_STANDARD_SKIN_CGINC

#include "Assets/Alloy/Shaders/Framework/Lighting.cginc"

#define blurredNormalTangent custom0.xyz
#define scatteringMask custom0.w
#define scatteringValue custom1.x
#define transmissionShadowWeight custom1.y

#ifdef A_FORWARD_ONLY
    #define A_SCATTERING_LUT _SssBrdfTex
    #define A_SCATTERING_WEIGHT _SssWeight
    #define A_SCATTERING_INV_MASK_CUTOFF 1.0h / _SssMaskCutoff
    #define A_SCATTERING_BIAS _SssBias
    #define A_SCATTERING_SCALE _SssScale
    #define A_SCATTERING_NORMAL_BLUR _SssBumpBlur
    #define A_SCATTERING_ABSORPTION _SssTransmissionAbsorption
    #define A_SCATTERING_AO_COLOR_BLEED _SssColorBleedAoWeights

    #define A_TRANSMISSION_WEIGHT _TransWeight
    #define A_TRANSMISSION_FALLOFF _TransPower
    #define A_TRANSMISSION_DISTORTION _TransDistortion
    #define A_TRANSMISSION_SHADOW 0.0h
#else
    #define A_SCATTERING_LUT _DeferredSkinLut
    #define A_SCATTERING_WEIGHT _DeferredSkinParams.x
    #define A_SCATTERING_INV_MASK_CUTOFF _DeferredSkinParams.y
    #define A_SCATTERING_BIAS _DeferredSkinTransmissionAbsorption.w
    #define A_SCATTERING_SCALE _DeferredSkinColorBleedAoWeights.w
    #define A_SCATTERING_NORMAL_BLUR _DeferredSkinParams.z
    #define A_SCATTERING_ABSORPTION _DeferredSkinTransmissionAbsorption.xyz
    #define A_SCATTERING_AO_COLOR_BLEED _DeferredSkinColorBleedAoWeights.xyz

    #define A_TRANSMISSION_WEIGHT _DeferredTransmissionParams.x
    #define A_TRANSMISSION_FALLOFF _DeferredTransmissionParams.y
    #define A_TRANSMISSION_DISTORTION _DeferredTransmissionParams.z
    #define A_TRANSMISSION_SHADOW _DeferredTransmissionParams.w
#endif

#ifdef A_FORWARD_ONLY
    /// Pre-Integrated scattering LUT.
    sampler2D _SssBrdfTex;

    /// Weight of the scattering effect.
    /// Expects values in the range [0,1].
    half _SssWeight;

    /// Cutoff value used to convert tranmission data to scattering mask.
    /// Expects values in the range [0.01,1].
    half _SssMaskCutoff;

    /// Biases the thickness value used to look up in the skin LUT.
    /// Expects values in the range [0,1].
    half _SssBias;

    /// Scales the thickness value used to look up in the skin LUT.
    /// Expects values in the range [0,1].
    half _SssScale;

    /// Increases the bluriness of the normal map for diffuse lighting.
    /// Expects values in the range [0,1].
    half _SssBumpBlur;

    /// Per-channel weights for thickness-based transmission color absorption.
    half3 _SssTransmissionAbsorption;

    /// Per-channel RGB gamma weights for colored AO.
    /// Expects a vector of non-zero values.
    half3 _SssColorBleedAoWeights;

    /// Weight of the transmission effect.
    /// Expects linear-space values in the range [0,1].
    half _TransWeight;

    /// Amount that the transmission is distorted by surface normals.
    /// Expects values in the range [0,1].
    half _TransDistortion;

    /// Falloff of the transmission effect.
    /// Expects values in the range [1,n).
    half _TransPower;
#else
    /// RGB=Blurred normals, A=Transmission thickness.
    /// Expects value in the buffer alpha.
    sampler2D _DeferredPlusBuffer;

    /// Pre-Integrated scattering LUT.
    sampler2D _DeferredSkinLut;

    /// X=Scattering Weight, Y=1/Mask Cutoff, Z=Blur Weight.
    /// Expects a vector of non-zero values.
    half3 _DeferredSkinParams;

    /// Per-channel weights for thickness-based transmission color absorption.
    /// LUT Bias in alpha.
    half4 _DeferredSkinTransmissionAbsorption;

    /// Per-channel RGB gamma weights for colored AO. LUT Scale in alpha.
    /// Expects a vector of non-zero values.
    half4 _DeferredSkinColorBleedAoWeights;

    /// X=Linear Weight, Y=Falloff, Z=Bump Distortion, W=Shadow Weight.
    /// Expects a vector of non-zero values.
    half4 _DeferredTransmissionParams;
#endif

void aSkinScattering(
    inout ASurface s,
    half3 blurredNormalWorld)
{
    // Scattering mask.
    s.scatteringMask *= A_SCATTERING_WEIGHT * saturate(A_SCATTERING_INV_MASK_CUTOFF * s.transmission);
    s.scatteringValue = saturate(s.transmission * A_SCATTERING_SCALE + A_SCATTERING_BIAS);

    // Skin depth absorption tint.
    // cf http://www.crytek.com/download/2014_03_25_CRYENGINE_GDC_Schultz.pdf pg 35
    half3 absorption = exp((1.0h - s.transmission) * A_SCATTERING_ABSORPTION);
    s.transmissionColor = s.albedo * aGammaToLinear(s.transmission);
    s.transmissionColor = A_TRANSMISSION_WEIGHT * lerp(s.transmissionColor, absorption, s.scatteringMask);

    // Blurred normals for indirect diffuse and direct scattering.
    s.ambientNormalWorld = normalize(lerp(s.normalWorld, blurredNormalWorld, A_SCATTERING_NORMAL_BLUR * s.scatteringMask));
}

void aPreSurface(
    inout ASurface s)
{
    s.blurredNormalTangent = A_FLAT_NORMAL;
}

void aPostSurface(
    inout ASurface s)
{
    // Apply metallic to skin transmission, since it doesn't apply albedo.
    // Also disables scattering effects over metals.
    s.transmission *= (1.0h - s.metallic);
    s.transmissionShadowWeight = A_TRANSMISSION_SHADOW;
    s.scatteringMask = 1.0h;
    s.materialType = 0.0h;
    aSkinScattering(s, aTangentToWorld(s, s.blurredNormalTangent));
}

void aGbufferSurface(
    inout ASurface s)
{
#ifndef A_FORWARD_ONLY
    half4 buffer = tex2D(_DeferredPlusBuffer, s.screenUv);
    s.transmission = 1.0h - buffer.a;
    s.transmissionShadowWeight = 0.75 < s.materialType ? A_TRANSMISSION_SHADOW : 0.0h;
    s.scatteringMask = 0.5h < s.materialType ? 0.0h : 1.0h;
    aSkinScattering(s, normalize(buffer.xyz * 2.0h - 1.0h));
#endif
}

half3 aDirect( 
    ADirect d,
    ASurface s)
{
    return aStandardDirect(d, s, 1.0h - s.scatteringMask)
        + aStandardSkin(d, s, A_SCATTERING_LUT, s.scatteringValue, s.scatteringMask)
        + aStandardTransmission(d, s, A_TRANSMISSION_DISTORTION, A_TRANSMISSION_FALLOFF, s.transmissionShadowWeight);
}

half3 aIndirect(
    AIndirect i,
    ASurface s)
{
#ifdef A_AMBIENT_OCCLUSION_ON
    // Color Bleed AO.
    // cf http://www.iryoku.com/downloads/Next-Generation-Character-Rendering-v6.pptx pg113
    i.diffuse *= pow(s.ambientOcclusion, A_ONE - (A_SCATTERING_AO_COLOR_BLEED * s.scatteringMask));
    s.ambientOcclusion = 1.0h;
#endif

    return aStandardIndirect(i, s);
}

#endif // A_LIGHTING_STANDARD_SKIN_CGINC
