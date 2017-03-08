// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Parallax.cginc
/// @brief Surface heightmap-based texcoord modification.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_FEATURES_PARALLAX_CGINC
#define A_FEATURES_PARALLAX_CGINC

#ifdef _PARALLAXMAP
    #ifndef A_VIEW_VECTOR_TANGENT_ON
        #define A_VIEW_VECTOR_TANGENT_ON
    #endif
#endif

#include "Assets/Alloy/Shaders/Framework/Feature.cginc"
    
#ifdef _PARALLAXMAP    
    /// Number of samples used for direct view of POM effect.
    /// Expects values in the range [1,n].
    float _MinSamples;
    
    /// Number of samples used for grazing view of POM effect.
    /// Expects values in the range [1,n].
    float _MaxSamples;
#endif

/// Applies the Parallax feature to the given material data.
/// @param[in,out] s Material surface data.
void aParallax(
    inout ASurface s) 
{
#ifdef _PARALLAXMAP
    #ifndef _BUMPMODE_POM
        aOffsetBumpMapping(s);
    #else
        aParallaxOcclusionMapping(s, _MinSamples, _MaxSamples);
    #endif 
#endif 
}

#endif // A_FEATURES_PARALLAX_CGINC
