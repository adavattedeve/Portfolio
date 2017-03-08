// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Transmission.cginc
/// @brief Standard lighting model with Transmission. Deferred+Forward.
///////////////////////////////////////////////////////////////////////////////

#ifndef A_LIGHTING_STANDARD_TRANSMISSION_CGINC
#define A_LIGHTING_STANDARD_TRANSMISSION_CGINC

#include "Assets/Alloy/Shaders/Framework/Lighting.cginc"

#define transmissionShadowWeight custom0.x

#ifdef A_FORWARD_ONLY
    #define A_TRANSMISSION_WEIGHT 1.0h
    #define A_TRANSMISSION_FALLOFF _TransPower
    #define A_TRANSMISSION_DISTORTION _TransDistortion
    // Exploit auto zero if not set.
    #define A_TRANSMISSION_SHADOW _TransShadowWeight
#else
    #define A_TRANSMISSION_WEIGHT _DeferredTransmissionParams.x
    #define A_TRANSMISSION_FALLOFF _DeferredTransmissionParams.y
    #define A_TRANSMISSION_DISTORTION _DeferredTransmissionParams.z
    #define A_TRANSMISSION_SHADOW _DeferredTransmissionParams.w
#endif

half _ShadowCullMode;

#ifdef A_FORWARD_ONLY
    /// Shadow influence on the transmission effect.
    /// Expects values in the range [0,1].
    half _TransShadowWeight;

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

    /// X=Linear Weight, Y=Falloff, Z=Bump Distortion, W=Shadow Weight.
    /// Expects a vector of non-zero values.
    half4 _DeferredTransmissionParams;
#endif

void aTransmissionColor(
    inout ASurface s)
{
#ifdef A_FORWARD_ONLY
    s.transmissionColor *= s.albedo;
#elif defined(A_FORWARD_PASS)
    s.transmissionColor *= s.albedo * A_TRANSMISSION_WEIGHT;
#else
    s.transmissionColor = s.albedo * (A_TRANSMISSION_WEIGHT * aGammaToLinear(s.transmission.r));
#endif
}

void aPreSurface(
    inout ASurface s)
{

}

void aPostSurface(
    inout ASurface s)
{
    s.ambientNormalWorld = s.normalWorld;
    aTransmissionColor(s);

#ifdef A_TWO_SIDED_ON
    s.materialType = 1.0h;
    s.transmissionShadowWeight = A_TRANSMISSION_SHADOW;
#else
    s.materialType = 0.5h < _ShadowCullMode && 1.5h > _ShadowCullMode ? 1.0h : 2.0h / 3.0h;
    s.transmissionShadowWeight = 0.5 < _ShadowCullMode && 1.5 > _ShadowCullMode ? 1.0h : 0.0h;
#endif
}

void aGbufferSurface(
    inout ASurface s)
{
#ifndef A_FORWARD_ONLY
    s.transmission = 1.0h - tex2D(_DeferredPlusBuffer, s.screenUv).a;
    s.transmissionShadowWeight = 0.75 < s.materialType ? A_TRANSMISSION_SHADOW : 0.0h;
    aTransmissionColor(s);
#endif
}

half3 aDirect( 
    ADirect d,
    ASurface s)
{
    return aStandardDirect(d, s) 
        + aStandardTransmission(d, s, A_TRANSMISSION_DISTORTION, A_TRANSMISSION_FALLOFF, s.transmissionShadowWeight);
}

half3 aIndirect(
    AIndirect i,
    ASurface s)
{
    return aStandardIndirect(i, s);
}

#endif // A_LIGHTING_STANDARD_TRANSMISSION_CGINC
