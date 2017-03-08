// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Wetness.cginc
/// @brief Handles all wet material effects.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_FEATURES_WETNESS_CGINC
#define A_FEATURES_WETNESS_CGINC

#include "Assets/Alloy/Shaders/Framework/Feature.cginc"

#ifdef _WETNESS_ON
    #ifndef A_WETNESS_MASK_OFF
        /// Mask that controls the wetness influence on the base material.
        /// Expects an alpha data map.
        A_SAMPLER_2D(_WetMask);
    #endif
    
    /// Controls how much the vertex alpha masks the detail maps.
    /// Expects values in the range [0,1].
    half _WetMaskStrength;
    
    /// The tint color for liquid wetting the surface.
    /// Expects a linear LDR color.
    half3 _WetTint;

    #ifndef A_WETNESS_NORMAL_MAP_OFF
        /// Wetness normal map.
        /// Expects a compressed normal map.
        A_SAMPLER_2D(_WetNormalMap);
    #endif
    
    /// Controls the wetness influence on the base material.
    /// Expects values in the range [0,1].
    half _WetWeight;

    /// The roughness for wet porous materials.
    /// Expects values in the range [0,1].
    half _WetRoughness;

    #ifndef A_WETNESS_NORMAL_MAP_OFF
        /// Normal map XY scale.
        half _WetNormalMapScale;
    #endif
#endif

/// Applies the Wetness feature to the given material data.
/// @param[in,out] s Material surface data.
void aWetness(
    inout ASurface s) 
{
#ifdef _WETNESS_ON
    half mask = s.mask * _WetWeight;

    #ifndef A_WETNESS_MASK_OFF
        #ifdef _WETMASKSOURCE_VERTEXCOLORALPHA
            mask *= s.vertexColor.a;
        #else
            float2 maskUv = A_TEX_TRANSFORM_UV_SCROLL(s, _WetMask);
            mask *= tex2D(_WetMask, maskUv).a;
        #endif

        mask = aLerpOneTo(mask, _WetMaskStrength);
    #endif

    // Physically-based wet surfaces. 
    // cf https://seblagarde.files.wordpress.com/2013/08/gdce13_lagarde_harduin_light.pdf pg 63
    half porosity = 1.0h;

    #ifndef A_WETNESS_POROSITY_OFF
        porosity = (1.0h - s.metallic) * saturate(s.roughness * 2.5h - 1.25h);
    #endif

    s.baseColor *= aLerpWhiteTo(_WetTint * aLerpOneTo(0.2h, porosity), mask);
    s.roughness *= aLerpOneTo(_WetRoughness, aLerpOneTo(0.2h, 0.5h * porosity) * mask);

    #ifndef A_WETNESS_NORMAL_MAP_OFF
        float2 normalUv = A_TEX_TRANSFORM_UV_SCROLL(s, _WetNormalMap);
        half3 wetNormals = UnpackScaleNormal(tex2D(_WetNormalMap, normalUv), mask * _WetNormalMapScale);
        aSetNormalTangent(s, BlendNormals(s.normalTangent, wetNormals));
    #endif
#endif
} 

#endif // A_FEATURES_WETNESS_CGINC
