// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Feature.cginc
/// @brief Features uber-header. Holds global methods and uniforms.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_FRAMEWORK_FEATURE_CGINC
#define A_FRAMEWORK_FEATURE_CGINC

// NOTE: Config comes first to override Unity settings!
#include "Assets/Alloy/Shaders/Config.cginc"
#include "Assets/Alloy/Shaders/Framework/Splat.cginc"
#include "Assets/Alloy/Shaders/Framework/Surface.cginc"
#include "Assets/Alloy/Shaders/Framework/Utility.cginc"

#include "UnityCG.cginc"
#include "UnityStandardBRDF.cginc"
#include "UnityStandardUtils.cginc"

// NOTE: baseTiling is last in order to prevent compilation errors when using AT.
#ifdef _VIRTUALTEXTURING_ON
    #define A_SET_BASE_UV(s, TEX) \
        s.baseUv = A_TEX_TRANSFORM_UV(s, TEX); \
        s.baseVirtualCoord = VTComputeVirtualCoord(s.baseUv); \
        s.baseTiling = TEX##_ST.xy;

    #define A_SET_BASE_UV_SCROLL(s, TEX) \
        s.baseUv = A_TEX_TRANSFORM_UV_SCROLL(s, TEX); \
        s.baseVirtualCoord = VTComputeVirtualCoord(s.baseUv); \
        s.baseTiling = TEX##_ST.xy;
#else
    #define A_SET_BASE_UV(s, TEX) \
        s.baseUv = A_TEX_TRANSFORM_UV(s, TEX); \
        s.baseTiling = TEX##_ST.xy;

    #define A_SET_BASE_UV_SCROLL(s, TEX) \
        s.baseUv = A_TEX_TRANSFORM_UV_SCROLL(s, TEX); \
        s.baseTiling = TEX##_ST.xy; 
#endif

/// Cutoff value that controls where cutout occurs over opacity.
/// Expects values in the range [0,1].
half _Cutoff;

#ifdef A_TWO_SIDED_ON
    /// Toggles inverting the backface normals.
    /// Expects the values 0 or 1.
    half _TransInvertBackNormal;
#endif

/// The base tint color.
/// Expects a linear LDR color with alpha.
half4 _Color;

/// Base color map.
/// Expects an RGB(A) map with sRGB sampling.
A_SAMPLER_2D(_MainTex);

#ifndef A_EXPANDED_MATERIAL_MAPS
    /// Base packed material map.
    /// Expects an RGBA data map.
    A_SAMPLER_2D(_SpecTex);
#else
    /// Metallic map.
    /// Expects an RGB map with sRGB sampling
    sampler2D _MetallicMap;

    /// Ambient Occlusion map.
    /// Expects an RGB map with sRGB sampling
    sampler2D _AoMap;

    /// Specularity map.
    /// Expects an RGB map with sRGB sampling
    sampler2D _SpecularityMap;

    /// Roughness map.
    /// Expects an RGB map with sRGB sampling
    sampler2D _RoughnessMap;
#endif

/// Base normal map.
/// Expects a compressed normal map.
sampler2D _BumpMap;

/// Height map.
/// Expects an RGBA data map.
sampler2D _ParallaxMap;

/// Toggles tinting the base color by the vertex color.
/// Expects values in the range [0,1].
half _BaseColorVertexTint;

/// The base metallic scale.
/// Expects values in the range [0,1].
half _Metal; 

/// The base specularity scale.
/// Expects values in the range [0,1].
half _Specularity;

#ifdef A_SPECULAR_TINT_ON
    // Amount that f0 is tinted by the base color.
    /// Expects values in the range [0,1].
    half _SpecularTint;
#endif

/// The base roughness scale.
/// Expects values in the range [0,1].
half _Roughness;

/// Ambient Occlusion strength.
/// Expects values in the range [0,1].
half _Occlusion;

/// Normal map XY scale.
half _BumpScale;

/// Height scale of the heightmap.
/// Expects values in the range [0,0.08].
float _Parallax;

/// Sets whether backface normals are inverted.
/// @param[in,out] s Material surface data.
void aTwoSided(inout ASurface s);

/// Sets opacity channel as mask.
/// @param[in,out] s Material surface data.
void aOpacityMask(inout ASurface s);

/// Sets mask to replace everything.
/// @param[in,out] s Material surface data.
void aNoMask(inout ASurface s);

/// Inverts current feature mask.
/// @param[in,out] s Material surface data.
void aInvertMask(inout ASurface s);

/// Sets the feature mask by combining a packed mask map with a picker vector.
/// @param[in,out]  s           Material surface data.
/// @param[in]      masks       Masks from packed mask map.
/// @param[in]      channels    Channel picker vector, with values of 0 or 1.
void aChannelPickerMask(inout ASurface s, half4 masks, half4 channels);

/// Sets the feature mask by using a gradient input mask.
/// @param[in,out]  s           Material surface data.
/// @param[in]      mask        Gradient where effect goes from black to white.
/// @param[in]      weight      Weight of the effect.
/// @param[in]      cutoff      Value below which the gradient becomes a mask.
/// @param[in]      blendRange  Range of smooth blend above cutoff.
/// @param[in]      vertexTint  Weight of vertex color alpha cutoff override.
void aBlendRangeMask(inout ASurface s, half mask, half weight, half cutoff, half blendRange, half vertexTint);

/// Sets base UVs to match _MainTex sampler.
/// @param[in,out] s Material surface data.
void aSetDefaultBaseUv(inout ASurface s);

/// Sets base UVs directly.
/// @param[in,out]  s Material surface data.
/// @param[in]      baseUv Material surface data.
void aSetBaseUv(inout ASurface s, float2 baseUv);

/// Applies cutout effect.
/// @param s Material surface data.
void aCutout(ASurface s);

/// Samples the base color map.
/// @param  s   Material surface data.
/// @return     Base Color with alpha.
half4 aSampleBase(ASurface s);

/// Samples the base material map.
/// @param  s   Material surface data.
/// @return     Packed material.
half4 aSampleMaterial(ASurface s);

/// Samples  and scales the base bump map.
/// @param  s       Material surface data.
/// @param  scale   Normal XY scale factor.
/// @return         Normalized tangent-space normal.
half3 aSampleBumpScale(ASurface s, half scale);

/// Samples the base bump map.
/// @param  s   Material surface data.
/// @return     Normalized tangent-space normal.
half3 aSampleBump(ASurface s);

/// Samples the base bump map biasing the mipmap level sampled.
/// @param  s       Material surface data.
/// @param  bias    Mipmap level bias.
/// @return         Normalized tangent-space normal.
half3 aSampleBumpBias(ASurface s, float bias);

/// Samples the base bump map biasing the mipmap level sampled.
/// @param  s   Material surface data.
/// @return     Normalized tangent-space normal.
half aSampleHeight(ASurface s);

/// Applies color based on weight parameter.
/// @param  s           Material surface data.
/// @param  strength    Amount to blend in vertex color.
/// @return             Vertex color tint.
half3 aVertexColorTint(ASurface s, half strength);

/// Applies base vertex color.
/// @param  s   Material surface data.
/// @return     Vertex color tint.
half3 aBaseVertexColorTint(ASurface s);

/// Gets combined base color tint from uniform and vertex color.
/// @param  s   Material surface data.
/// @return     Base Color with alpha.
half4 aBaseTint(ASurface s);

/// Gets combined base color from main channels.
/// @param  s   Material surface data.
/// @return     Base Color with alpha.
half4 aBaseColor(ASurface s);

/// Obtains specularity from packed map.
/// @param  material    Packed material map sample.
/// @return             Specularity.
half aMetallic(half4 material);

/// Obtains ambient occlusion from packed map.
/// @param  material    Packed material map sample.
/// @return             Modified linear-space AO.
half aAmbientOcclusion(half4 material);

/// Obtains specularity from packed map.
/// @param  material    Packed material map sample.
/// @return             Specularity.
half aSpecularity(half4 material);

/// Obtains roughness from packed map.
/// @param  material    Packed material map sample.
/// @return             Roughness.
half aRoughness(half4 material);

/// Applies texture coordinate offsets to surface data.
/// @param[in,out]  s       Material surface data.
/// @param[in]      offset  Texture coordinate offset.
void aParallaxOffset(inout ASurface s, float2 offset);

/// Calculates Offset Bump Mapping texture offsets.
/// @param[in,out]  s   Material surface data.
void aOffsetBumpMapping(inout ASurface s);

/// Calculates Parallax Occlusion Mapping texture offsets.
/// @param[in,out]  s           Material surface data.
/// @param[in]      minSamples  Minimum number of samples for POM effect [1,n].
/// @param[in]      maxSamples  Maximum number of samples for POM effect [1,n].
void aParallaxOcclusionMapping(inout ASurface s, float minSamples, float maxSamples);

#endif // A_FRAMEWORK_FEATURE_CGINC
