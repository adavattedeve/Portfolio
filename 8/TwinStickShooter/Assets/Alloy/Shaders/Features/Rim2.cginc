// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Rim2.cginc
/// @brief Secondary rim lighting effects.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_FEATURES_RIM2_CGINC
#define A_FEATURES_RIM2_CGINC

#ifdef _RIM2_ON
    #ifndef A_NORMAL_WORLD_ON
        #define A_NORMAL_WORLD_ON
    #endif

    #ifndef A_VIEW_DIR_WORLD_ON
        #define A_VIEW_DIR_WORLD_ON
    #endif
#endif

#include "Assets/Alloy/Shaders/Framework/Feature.cginc"

#ifdef _RIM2_ON
    /// Secondary rim lighting tint color.
    /// Expects a linear HDR color.
    half3 _Rim2Color;

    #ifndef A_RIM2_EFFECTS_MAP_OFF
        /// Secondary rim effect texture.
        /// Expects an RGB map with sRGB sampling.
        A_SAMPLER_2D(_RimTex2);
    #endif
    
    /// The weight of the secondary rim lighting effect.
    /// Expects linear space value in the range [0,1].
    half _Rim2Weight;
    
    /// Fills in the center of the secondary rim lighting effect.
    /// Expects linear-space values in the range [0,1].
    half _Rim2Bias;
    
    /// Controls the falloff of the secondary rim lighting effect.
    /// Expects values in the range [0.01,n].
    half _Rim2Power;
#endif

/// Applies the Rim Lighting feature to the given material data.
/// @param[in,out] s Material surface data.
void aRim2(
    inout ASurface s)
{	
#ifdef _RIM2_ON 
    half3 rim = _Rim2Color;

    #ifndef A_RIM2_EFFECTS_MAP_OFF
        float2 rimUv2 = A_TEX_TRANSFORM_UV_SCROLL(s, _RimTex2);
        rim *= tex2D(_RimTex2, rimUv2).rgb;
    #endif
    
    s.emission += rim * aRimLight(_Rim2Weight * aGammaToLinear(s.mask), _Rim2Bias, _Rim2Power, s.NdotV);
#endif
} 

#endif // A_FEATURES_RIM2_CGINC
