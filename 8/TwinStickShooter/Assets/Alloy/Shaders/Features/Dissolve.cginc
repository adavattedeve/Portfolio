// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Dissolve.cginc
/// @brief Surface dissolve effects.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_FEATURES_DISSOLVE_CGINC
#define A_FEATURES_DISSOLVE_CGINC

#ifdef _DISSOLVE_ON
    #ifndef A_SHADOW_SURFACE_ON
        #define A_SHADOW_SURFACE_ON
    #endif
#endif

#include "Assets/Alloy/Shaders/Framework/Feature.cginc"

#ifdef _DISSOLVE_ON
    /// Dissolve glow tint color.
    /// Expects a linear HDR color with alpha.
    half4 _DissolveGlowColor; 
    
    /// Dissolve glow color with effect ramp in the alpha.
    /// Expects an RGBA map with sRGB sampling.
    A_SAMPLER_2D(_DissolveTex);
    
    /// The cutoff value for the dissolve effect in the ramp map.
    /// Expects values in the range [0,1].
    half _DissolveCutoff;

    #ifndef A_DISSOLVE_GLOW_OFF
        /// The weight of the dissolve glow effect.
        /// Expects linear space value in the range [0,1].
        half _DissolveGlowWeight;
    
        /// The width of the dissolve glow effect.
        /// Expects values in the range [0,1].
        half _DissolveEdgeWidth;
    #endif
#endif

/// Applies the Dissolve feature to the given material data.
/// @param[in,out] s Material surface data.
void aDissolve(
    inout ASurface s) 
{
#ifdef _DISSOLVE_ON
    float2 dissolveUv = A_TEX_TRANSFORM_UV(s, _DissolveTex);
    half4 dissolveBase = _DissolveGlowColor * tex2D(_DissolveTex, dissolveUv);
    half dissolveCutoff = s.mask * _DissolveCutoff;
    half clipval = dissolveBase.a * 0.99h - dissolveCutoff;

    clip(clipval); // NOTE: Eliminates need for blend edge.
        
	#ifndef A_DISSOLVE_GLOW_OFF
		// Dissolve glow
        half3 glow = s.emission + dissolveBase.rgb * _DissolveGlowWeight;

        glow = clipval >= _DissolveEdgeWidth ? s.emission : glow; // Outer edge.
        s.emission = dissolveCutoff < A_EPSILON ? s.emission : glow; // Kill when cutoff is zero.
    #endif
#endif
} 

#endif // A_FEATURES_DISSOLVE_CGINC
