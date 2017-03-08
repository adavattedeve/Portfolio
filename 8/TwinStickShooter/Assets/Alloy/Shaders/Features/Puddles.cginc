// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Puddles.cginc
/// @brief Handles all puddle material effects.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_FEATURES_PUDDLES_CGINC
#define A_FEATURES_PUDDLES_CGINC

#include "Assets/Alloy/Shaders/Framework/Feature.cginc"

#ifdef _PUDDLES_ON
    A_SAMPLER_2D(_PuddlesRippleTex);

    // Mask

    half _PuddlesWeight;

    half _PuddlesRippleWeight;

    // Wetness

    // Wet Tint

    // Wet roughness.

    half _PuddlesLevel;

    // Puddle Roughness
#endif

/// Applies the Puddles feature to the given material data.
/// @param[in,out] s Material surface data.
void aPuddles(
    inout ASurface s) 
{
#ifdef _PUDDLES_ON
    half mask = _PuddlesWeight * s.vertexColor.a * s.mask;

    // Physically-based puddles. 
    // cf https://seblagarde.wordpress.com/2013/01/03/water-drop-2b-dynamic-rain-and-its-effects/

    // Wetness

    // Unity uses a Left-handed axis, so it requires clumsy remapping.
    //const half3x3 yTangentToWorld = half3x3(A_AXIS_X, A_AXIS_Z, s.vertexNormalWorld);
    //float2 rippleUv = A_TEX_TRANSFORM_SCROLL(_RippleTex, s.positionWorld.xz);
    //half3 ripples = lerp(A_FLAT_NORMAL, tex2D(_RippleTex, rippleUv) * 2.0h - 1.0h, _PuddlesRippleWeight);

    //ripples = mul(ripples, yTangentToWorld);
    //ripples = aWorldToTangent(s, ripples);

    //half puddles = _PuddlesLevel * mask;

    //aSetNormalTangent(s, normalize(lerp(s.normalTangent, ripples, puddles)));
#endif
} 

#endif // A_FEATURES_PUDDLES_CGINC
