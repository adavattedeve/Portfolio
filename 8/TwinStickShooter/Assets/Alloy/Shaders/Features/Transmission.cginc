// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Transmission.cginc
/// @brief Basic transmission, handling render path differences.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_FEATURES_TRANSMISSION_CGINC
#define A_FEATURES_TRANSMISSION_CGINC

#include "Assets/Alloy/Shaders/Framework/Feature.cginc"

#ifdef A_TRANSMISSION_ON
    /// Transmission tint color.
    /// Expects a linear LDR color.
    half3 _TransColor;

    /// Transmission color * thickness texture.
    /// Expects an RGB map with sRGB sampling.
    sampler2D _TransTex;

    /// Weight of the transmission effect.
    /// Expects gamma or linear-space values in the range [0,1].
    half _TransScale;
#endif

/// Applies the Transmission feature to the given material data.
/// @param[in,out] s Material surface data.
void aTransmission(
    inout ASurface s) 
{
#ifdef A_TRANSMISSION_ON
    #ifdef A_TRANSMISSION_ALPHA
        // Gamma space inputs.
        half value = _TransScale * tex2D(_TransTex, s.baseUv).a;

        #ifdef A_FORWARD_ONLY
            s.transmissionColor = _TransColor * aGammaToLinear(value);
        #elif defined(A_FORWARD_PASS)
            s.transmissionColor = aGammaToLinear(value);
        #else
            s.transmission = value;
        #endif
    #else
        // Linear space inputs.
        half3 color = _TransScale * tex2D(_TransTex, s.baseUv).rgb;

        #ifdef A_FORWARD_ONLY
            s.transmissionColor = _TransColor * color;
        #elif defined(A_FORWARD_PASS)
            s.transmissionColor = color;
        #else
            s.transmission = LinearToGammaSpace(color).g;
        #endif
    #endif
#endif
} 

#endif // A_FEATURES_TRANSMISSION_CGINC
