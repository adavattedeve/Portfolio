// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file SecondaryTextures.cginc
/// @brief Secondary set of textures.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_FEATURES_SECONDARY_TEXTURES_CGINC
#define A_FEATURES_SECONDARY_TEXTURES_CGINC

#ifdef A_SECONDARY_TEXTURES_ON
    #ifndef A_METALLIC_ON
        #define A_METALLIC_ON
    #endif

    #if !defined(A_AMBIENT_OCCLUSION_ON) && !defined(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A)
        #define A_AMBIENT_OCCLUSION_ON
    #endif
#endif

#include "Assets/Alloy/Shaders/Framework/Feature.cginc"

#ifdef A_SECONDARY_TEXTURES_ON
    /// The secondary tint color.
    /// Expects a linear LDR color with alpha.
    half4 _Color2;
    
    /// The secondary color map.
    /// Expects an RGB(A) map with sRGB sampling.
    A_SAMPLER_2D(_MainTex2);

    #ifndef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        /// The secondary packed material map.
        /// Expects an RGBA data map.
        sampler2D _MaterialMap2;
    #endif

    /// The secondary normal map.
    /// Expects a compressed normal map.
    sampler2D _BumpMap2;
    
    /// Toggles tinting the secondary color by the vertex color.
    /// Expects values in the range [0,1].
    half _BaseColorVertexTint2;

    /// The secondary metallic scale.
    /// Expects values in the range [0,1].
    half _Metallic2;

    /// The secondary specularity scale.
    /// Expects values in the range [0,1].
    half _Specularity2;
    
    /// The secondary roughness scale.
    /// Expects values in the range [0,1].
    half _Roughness2;
    
    /// Ambient Occlusion strength.
    /// Expects values in the range [0,1].
    half _Occlusion2;

    /// Normal map XY scale.
    half _BumpScale2;
#endif

/// Applies the Secondary Textures feature to the given material data.
/// @param[in,out] s Material surface data.
void aSecondaryTextures(
    inout ASurface s)
{
#ifdef A_SECONDARY_TEXTURES_ON
    float2 baseUv2 = A_TEX_TRANSFORM_UV_SCROLL(s, _MainTex2);
    half mask = s.mask;
    half4 material = A_ONE4;
    half4 base = tex2D(_MainTex2, baseUv2);
    
    #ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        material.A_ROUGHNESS_CHANNEL = base.a;
        base.a = 1.0h;
    #else
        material = tex2D(_MaterialMap2, baseUv2);
        material.A_AO_CHANNEL = aOcclusionStrength(material.A_AO_CHANNEL, _Occlusion2);
    #endif

    half3 normals = UnpackScaleNormal(tex2D(_BumpMap2, baseUv2), _BumpScale2);

    base *= _Color2;
    base.rgb *= aVertexColorTint(s, _BaseColorVertexTint2);
    material.A_METALLIC_CHANNEL *= _Metallic2;
    material.A_SPECULARITY_CHANNEL *= _Specularity2;
    material.A_ROUGHNESS_CHANNEL *= _Roughness2;

    #ifndef A_SECONDARY_TEXTURES_ALPHA_BLEND_OFF
        mask *= base.a;
        base.a = 1.0h;
    #endif

    s.baseColor = lerp(s.baseColor, base.rgb, mask);
    s.opacity = lerp(s.opacity, base.a, mask);
    s.metallic = lerp(s.metallic, material.A_METALLIC_CHANNEL, mask);
    s.ambientOcclusion = lerp(s.ambientOcclusion, material.A_AO_CHANNEL, mask);
    s.specularity = lerp(s.specularity, material.A_SPECULARITY_CHANNEL, mask);
    s.roughness = lerp(s.roughness, material.A_ROUGHNESS_CHANNEL, mask);
    aSetNormalTangent(s, normalize(lerp(s.normalTangent, normals, mask)));
    s.emission *= (1.0h - mask);

    // NOTE: These are applied in here so we can use baseUv2.
    float2 baseUv = s.baseUv;

    s.baseUv = baseUv2;
    aEmission2(s);
    aRim2(s);
    
    s.baseUv = baseUv;
#endif
}

#endif // A_FEATURES_SECONDARY_TEXTURES_CGINC
