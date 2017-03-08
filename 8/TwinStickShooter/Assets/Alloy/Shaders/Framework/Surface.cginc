// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Surface.cginc
/// @brief ASurface structure, and related methods.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_FRAMEWORK_SURFACE_CGINC
#define A_FRAMEWORK_SURFACE_CGINC

#include "Assets/Alloy/Shaders/Config.cginc"
#include "Assets/Alloy/Shaders/Framework/Brdf.cginc"
#include "Assets/Alloy/Shaders/Framework/Utility.cginc"

#include "UnityCG.cginc"

/// Picks either UV0 or UV1.
#ifdef A_TEX_UV_OFF
    #define A_TEX_UV(s, name) (s.uv01.xy)
#else
    #define A_TEX_UV(s, name) (name##UV < 0.5f ? s.uv01.xy : s.uv01.zw)
#endif

/// Applies Unity texture transforms plus UV0.
#define A_TEX_TRANSFORM(s, name) (TRANSFORM_TEX(s.uv01.xy, name))

/// Applies Unity texture transforms plus UV-switching effect.
#define A_TEX_TRANSFORM_UV(s, name) (TRANSFORM_TEX(A_TEX_UV(s, name), name))

/// Applies Unity texture transforms plus UV-switching and our scrolling effects.
#define A_TEX_TRANSFORM_UV_SCROLL(s, name) (A_TEX_TRANSFORM_SCROLL(name, A_TEX_UV(s, name)))

/// Contains ALL data and state for rendering a surface.
/// Can set state to control how features are combined into the surface data.
struct ASurface {
    /////////////////////////////////////////////////////////////////////////////
    // Vertex Inputs.
    /////////////////////////////////////////////////////////////////////////////

    /// Screen-space position.
    float4 screenPosition;
    
    /// Screen-space texture coordinates.
    float2 screenUv;

    /// Unity's fog data.
    float fogCoord;

    /// The model's UV0 & UV1 texture coordinate data.
    /// Be aware that it can have parallax precombined with it.
    float4 uv01;
        
    /// Tangent space to World space rotation matrix.
    half3x3 tangentToWorld;

    /// Position in world space.
    float3 positionWorld;
        
    /// View direction in world space.
    /// Expects a normalized vector.
    half3 viewDirWorld;
        
    /// View direction in tangent space.
    /// Expects a normalized vector.
    half3 viewDirTangent;
    
    /// Distance from the camera to the given fragement.
    /// Expects values in the range [0,n].
    half viewDepth;
    
    /// Vertex color.
    /// Expects linear-space LDR color values.
    half4 vertexColor;

    /// Vertex normal in world space.
    /// Expects normalized vectors in the range [-1,1].
    half3 vertexNormalWorld;

    /// Indicates via sign whether a triangle is front or back facing.
    /// Positive is front-facing, negative is back-facing. 
    half facingSign;


    /////////////////////////////////////////////////////////////////////////////
    // Feature layering options.
    /////////////////////////////////////////////////////////////////////////////
    
    /// Masks where the next feature layer will be applied.
    /// Expects values in the range [0,1].
    half mask;
        
    /// The base map's texture transform tiling amount.
    float2 baseTiling;
        
    /// Transformed texture coordinates for the base map.
    float2 baseUv;

#ifdef _VIRTUALTEXTURING_ON
    /// Transformed texture coordinates for the virtual base map.
    VirtualCoord baseVirtualCoord;
#endif
    

    /////////////////////////////////////////////////////////////////////////////
    // Material data.
    /////////////////////////////////////////////////////////////////////////////

    /// Albedo and/or Metallic f0 based on settings. Used by Enlighten.
    /// Expects linear-space LDR color values.
    half3 baseColor;

    /// Controls opacity or cutout regions.
    /// Expects values in the range [0,1].
    half opacity;

    /// Interpolates material from dielectric to metal.
    /// Expects values in the range [0,1].
    half metallic;

    /// Diffuse ambient occlusion.
    /// Expects values in the range [0,1].
    half ambientOcclusion;
    
    /// Linear control of dielectric f0 from [0.00,0.08].
    /// Expects values in the range [0,1].
    half specularity;

    /// Linear roughness value, where zero is smooth and one is rough.
    /// Expects values in the range [0,1].
    half roughness;
    
    /// Tints the dielectric specularity by the base color chromaticity.
    /// Expects values in the range [0,1].
    half specularTint;

    /// Strength of clearcoat layer, used to apply masks.
    /// Expects values in the range [0,1].
    half clearCoat;
    
    /// Roughness of clearcoat layer.
    /// Expects values in the range [0,1].
    half clearCoatRoughness;
    
    /// Normal in tangent space.
    /// Expects a normalized vector.
    half3 normalTangent;
    
    /// Light emission by the material. Used by Enlighten.
    /// Expects linear-space HDR color values.
    half3 emission;

    /// Monochrome transmission thickness.
    /// Expects gamma-space LDR values.
    half transmission;


    /////////////////////////////////////////////////////////////////////////////
    // BRDF inputs.
    /////////////////////////////////////////////////////////////////////////////
    
    /// Diffuse albedo.
    /// Expects linear-space LDR color values.
    half3 albedo;
    
    /// Fresnel reflectance at incidence zero.
    /// Expects linear-space LDR color values.
    half3 f0;
    
    /// Beckmann roughness.
    /// Expects values in the range [0,1].
    half beckmannRoughness;
    
    /// Specular occlusion.
    /// Expects values in the range [0,1].
    half specularOcclusion;
        
    /// Color tint for transmission effect.
    /// Expects linear-space LDR color values.
    half3 transmissionColor;
    
    /// Normal in world space.
    /// Expects normalized vectors in the range [-1,1].
    half3 normalWorld;

    /// Deferred material lighting type.
    /// Expects the values 0, 1/3, 2/3, or 1.
    half materialType;

    /// View reflection vector in world space.
    /// Expects a non-normalized vector.
    half3 reflectionVectorWorld;
    
    /// Clamped N.V.
    /// Expects values in the range [0,1].
    half NdotV;

    /// Fresnel weight of N.V.
    /// Expects values in the range [0,1].
    half FV;
    
    /// Ambient diffuse normal in world space.
    /// Expects normalized vectors in the range [-1,1].
    half3 ambientNormalWorld;


    /////////////////////////////////////////////////////////////////////////////
    // Custom fields.
    /////////////////////////////////////////////////////////////////////////////
    half4 custom0;
    half4 custom1;
    half4 custom2;
    half4 custom3;
    half4 custom4;
    half4 custom5;
    half4 custom6;
};

/// Constructor. 
/// @return Structure initialized with sane default values.
ASurface aCreateSurface();

/// Calculates and sets view-dependent data.
/// @param[in,out] s Material surface data.
void aUpdateViewData(inout ASurface s);

/// Transforms a normal from tangent-space to world-space.
/// @param  s               Material surface data.
/// @param  normalTangent   Normal in tangent space.
/// @return                 Normal in world space.
half3 aTangentToWorld(ASurface s, half3 normalTangent);

/// Transforms a normal from world-space to tangent-space.
/// @param  s           Material surface data.
/// @param  normalWorld Normal in world space.
/// @return             Normal in tangent space.
half3 aWorldToTangent(ASurface s, half3 normalWorld);

/// Sets tangent-space normal, and updates normal data.
/// @param[in,out]  s               Material surface data.
/// @param[in]      normalTangent   Normal in tangent space.
void aSetNormalTangent(inout ASurface s, half3 normalTangent);

/// Sets world-space normal, and updates normal data.
/// @param[in,out]  s           Material surface data.
/// @param[in]      normalWorld Normal in world space.
void aSetNormalWorld(inout ASurface s, half3 normalWorld);

#endif // A_FRAMEWORK_SURFACE_CGINC
