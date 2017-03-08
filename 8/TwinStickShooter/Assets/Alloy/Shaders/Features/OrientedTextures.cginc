// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file OrientedTextures.cginc
/// @brief Secondary set of textures using world/object position XZ as their UVs.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_FEATURES_ORIENTED_TEXTURES_CGINC
#define A_FEATURES_ORIENTED_TEXTURES_CGINC

#ifdef A_ORIENTED_TEXTURES_ON
    #ifndef A_NORMAL_WORLD_ON
        #define A_NORMAL_WORLD_ON
    #endif

    #ifndef A_POSITION_WORLD_ON
        #define A_POSITION_WORLD_ON
    #endif

    #ifndef A_METALLIC_ON
        #define A_METALLIC_ON
    #endif

    #if !defined(A_AMBIENT_OCCLUSION_ON) && !defined(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A)
        #define A_AMBIENT_OCCLUSION_ON
    #endif
#endif

#include "Assets/Alloy/Shaders/Framework/Feature.cginc"

#ifdef A_ORIENTED_TEXTURES_ON
    /// The world-oriented tint color.
    /// Expects a linear LDR color with alpha.
    half4 _OrientedColor;
    
    /// The world-oriented color map.
    /// Expects an RGB(A) map with sRGB sampling.
    A_SAMPLER_2D(_OrientedMainTex);

    #ifndef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        /// The world-oriented packed material map.
        /// Expects an RGBA data map.
        sampler2D _OrientedMaterialMap;
    #endif

    /// The world-oriented normal map.
    /// Expects a compressed normal map.
    sampler2D _OrientedBumpMap;
    
    /// Toggles tinting the world-oriented color by the vertex color.
    /// Expects values in the range [0,1].
    half _OrientedColorVertexTint;

    /// The world-oriented metallic scale.
    /// Expects values in the range [0,1].
    half _OrientedMetallic;

    /// The world-oriented specularity scale.
    /// Expects values in the range [0,1].
    half _OrientedSpecularity;
    
    /// The world-oriented roughness scale.
    /// Expects values in the range [0,1].
    half _OrientedRoughness;

    /// Ambient Occlusion strength.
    /// Expects values in the range [0,1].
    half _OrientedOcclusion;
    
    /// Normal map XY scale.
    half _OrientedNormalMapScale;
#endif

/// Applies the Emission feature to the given material data.
/// @param[in,out] s Material surface data.
void aOrientedTextures(
    inout ASurface s)
{
#ifdef A_ORIENTED_TEXTURES_ON
    // Unity uses a Left-handed axis, so it requires clumsy remapping.
    const half3x3 yTangentToWorld = half3x3(A_AXIS_X, A_AXIS_Z, s.vertexNormalWorld);
    float2 baseUv2 = A_TEX_TRANSFORM_SCROLL(_OrientedMainTex, s.positionWorld.xz);
    half mask = s.mask;
    half4 material = A_ONE4;
    half4 base = tex2D(_OrientedMainTex, baseUv2);    

    #ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        material.A_ROUGHNESS_CHANNEL = base.a;
        base.a = 1.0h;
    #else
        material = tex2D(_OrientedMaterialMap, baseUv2);
        material.A_AO_CHANNEL = aOcclusionStrength(material.A_AO_CHANNEL, _OrientedOcclusion);
    #endif

    half3 normals = UnpackScaleNormal(tex2D(_OrientedBumpMap, baseUv2), _OrientedNormalMapScale);
    normals = mul(normals, yTangentToWorld);

    base *= _OrientedColor;
    base.rgb *= aVertexColorTint(s, _OrientedColorVertexTint);
    material.A_METALLIC_CHANNEL *= _OrientedMetallic;
    material.A_SPECULARITY_CHANNEL *= _OrientedSpecularity;
    material.A_ROUGHNESS_CHANNEL *= _OrientedRoughness;

    #ifdef A_ORIENTED_TEXTURES_BLEND_OFF
        s.baseColor = base.rgb;
        s.opacity = base.a;
        s.metallic = material.A_METALLIC_CHANNEL;
        s.ambientOcclusion = material.A_AO_CHANNEL;
        s.specularity = material.A_SPECULARITY_CHANNEL;
        s.roughness = material.A_ROUGHNESS_CHANNEL;
        aSetNormalWorld(s, normalize(normals));
    #else
        #ifndef A_ORIENTED_TEXTURES_ALPHA_BLEND_OFF
            mask *= base.a;
            base.a = 1.0h;
        #endif

        s.baseColor = lerp(s.baseColor, base.rgb, mask);
        s.opacity = lerp(s.opacity, base.a, mask);
        s.metallic = lerp(s.metallic, material.A_METALLIC_CHANNEL, mask);
        s.ambientOcclusion = lerp(s.ambientOcclusion, material.A_AO_CHANNEL, mask);
        s.specularity = lerp(s.specularity, material.A_SPECULARITY_CHANNEL, mask);
        s.roughness = lerp(s.roughness, material.A_ROUGHNESS_CHANNEL, mask);
        aSetNormalWorld(s, normalize(lerp(s.normalWorld, normals, mask)));
        s.emission *= (1.0h - mask);
    #endif
#endif
}

#endif // A_FEATURES_ORIENTED_TEXTURES_CGINC
