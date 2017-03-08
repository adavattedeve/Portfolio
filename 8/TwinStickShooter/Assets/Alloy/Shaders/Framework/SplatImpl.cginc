// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file SplatImpl.cginc
/// @brief Splat method implementations to allow disabling of features.
///////////////////////////////////////////////////////////////////////////////

#ifndef A_FRAMEWORK_SPLAT_IMPL_CGINC
#define A_FRAMEWORK_SPLAT_IMPL_CGINC

#include "Assets/Alloy/Shaders/Framework/Splat.cginc"

ASplat aCreateSplat()
{
    ASplat sp;

    UNITY_INITIALIZE_OUTPUT(ASplat, sp);
    sp.base = 0.0h;
    sp.material = 0.0h;
    sp.normal = 0.0h;
    return sp;
}

ASplatContext aCreateSplatContext(
    ASurface s,
    half sharpness,
    float positionScale)
{
    ASplatContext sc;
    UNITY_INITIALIZE_OUTPUT(ASplatContext, sc);

    sc.uv01 = s.uv01;
    sc.vertexColor = s.vertexColor;

#ifdef A_TRIPLANAR_MAPPING_ON
    // Triplanar mapping
    // cf http://www.gamedev.net/blog/979/entry-2250761-triplanar-texturing-and-normal-mapping/
    #ifdef _TRIPLANARMODE_WORLD
        half3 geoNormal = s.vertexNormalWorld;
        sc.position = s.positionWorld;
    #else
        half3 geoNormal = UnityWorldToObjectDir(s.vertexNormalWorld);
        sc.position = mul(unity_WorldToObject, float4(s.positionWorld, 1.0f)).xyz;
    #endif

    // Unity uses a Left-handed axis, so it requires clumsy remapping.
    sc.xTangentToWorld = half3x3(A_AXIS_Z, A_AXIS_Y, geoNormal);
    sc.yTangentToWorld = half3x3(A_AXIS_X, A_AXIS_Z, geoNormal);
    sc.zTangentToWorld = half3x3(A_AXIS_X, A_AXIS_Y, geoNormal);

    half3 blending = abs(geoNormal);
    blending = normalize(max(blending, A_EPSILON));
    blending = pow(blending, sharpness);
    blending /= dot(blending, A_ONE);
    sc.blend = blending;

    sc.axisMasks = step(A_ZERO, geoNormal);
    sc.position *= positionScale;
#endif

    return sc;
}

void aApplySplat(
    inout ASurface s,
    ASplat sp)
{
    half4 base = sp.base;
    half4 material = sp.material;
    half3 normal = sp.normal;

    s.baseColor = base.rgb;
    s.opacity = base.a;
    s.metallic = material.A_METALLIC_CHANNEL;
    s.specularity = material.A_SPECULARITY_CHANNEL;
    s.roughness = material.A_ROUGHNESS_CHANNEL;

#ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
    s.specularTint = material.A_AO_CHANNEL;
#else
    s.ambientOcclusion = material.A_AO_CHANNEL;
#endif

#ifndef A_TRIPLANAR_MAPPING_ON
    aSetNormalTangent(s, normalize(normal));
#else
    #ifdef _TRIPLANARMODE_WORLD
        half3 normalWorld = normalize(normal);
    #else
        half3 normalWorld = UnityObjectToWorldNormal(normal);
    #endif    

    aSetNormalWorld(s, normalWorld);
#endif
}

void aMergeSplats(
    inout ASplat sp0,
    ASplat sp1)
{
    sp0.base += sp1.base;
    sp0.material += sp1.material;
    sp0.normal += sp1.normal;
}

void aSplatMaterial(
    inout ASplat sp,
    ASplatContext sc,
    half4 tint,
    half vertexTint,
    half metallic,
    half specularity,
    half specularTint,
    half roughness)
{
    sp.base *= tint;

#ifndef A_VERTEX_COLOR_IS_DATA
    sp.base.rgb *= aLerpWhiteTo(sc.vertexColor.rgb, vertexTint);
#endif

    sp.material.A_METALLIC_CHANNEL *= metallic;
    sp.material.A_SPECULARITY_CHANNEL *= specularity;
    sp.material.A_ROUGHNESS_CHANNEL *= roughness;

#ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
    sp.material.A_AO_CHANNEL = specularTint;
#endif
}

void aTriPlanarAxis(
    inout ASplat sp,
    half mask,
    half3x3 tbn,
    float2 uv,
    half occlusion,
    half bumpScale,
    sampler2D base,
    sampler2D material,
    sampler2D normal)
{
    half4 color = tex2D(base, uv);
    half4 mat = A_ONE4;

#ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
    mat.A_ROUGHNESS_CHANNEL = color.a;
    color.a = 1.0h;
#else
    mat = tex2D(material, uv);
    mat.A_AO_CHANNEL = aOcclusionStrength(mat.A_AO_CHANNEL, occlusion);
#endif

    sp.base += mask * color;
    sp.material += mask * mat;
    sp.normal += mask * mul(UnpackScaleNormal(tex2D(normal, uv), bumpScale), tbn);
}

void aTriPlanarX(
    inout ASplat sp,
    ASplatContext sc,
    A_SAMPLER_PARAM(base),
    sampler2D material,
    sampler2D normal,
    half occlusion,
    half bumpScale)
{
    aTriPlanarAxis(sp, sc.blend.x, sc.xTangentToWorld, A_TEX_TRANSFORM_SCROLL(base, sc.position.zy), occlusion, bumpScale, base, material, normal);
}

void aTriPlanarY(
    inout ASplat sp,
    ASplatContext sc,
    A_SAMPLER_PARAM(base),
    sampler2D material,
    sampler2D normal,
    half occlusion,
    half bumpScale)
{
    aTriPlanarAxis(sp, sc.blend.y, sc.yTangentToWorld, A_TEX_TRANSFORM_SCROLL(base, sc.position.xz), occlusion, bumpScale, base, material, normal);
}

void aTriPlanarZ(
    inout ASplat sp,
    ASplatContext sc,
    A_SAMPLER_PARAM(base),
    sampler2D material,
    sampler2D normal,
    half occlusion,
    half bumpScale)
{
    aTriPlanarAxis(sp, sc.blend.z, sc.zTangentToWorld, A_TEX_TRANSFORM_SCROLL(base, sc.position.xy), occlusion, bumpScale, base, material, normal);
}

void aTriPlanarPositiveY(
    inout ASplat sp,
    ASplatContext sc,
    A_SAMPLER_PARAM(base),
    sampler2D material,
    sampler2D normal,
    half occlusion,
    half bumpScale)
{
    aTriPlanarAxis(sp, sc.axisMasks.y * sc.blend.y, sc.yTangentToWorld, A_TEX_TRANSFORM_SCROLL(base, sc.position.xz), occlusion, bumpScale, base, material, normal);
}

void aTriPlanarNegativeY(
    inout ASplat sp,
    ASplatContext sc,
    A_SAMPLER_PARAM(base),
    sampler2D material,
    sampler2D normal,
    half occlusion,
    half bumpScale)
{
    aTriPlanarAxis(sp, (1.0h - sc.axisMasks.y) * sc.blend.y, sc.yTangentToWorld, A_TEX_TRANSFORM_SCROLL(base, sc.position.xz), occlusion, bumpScale, base, material, normal);
}

ASplat aCreateTerrainSplat(
    ASplatContext sc,
    A_SAMPLER_PARAM(base),
    sampler2D material,
    sampler2D normal,
    half4 tint,
    half metallic,
    half specularity,
    half specularTint,
    half roughness,
    half occlusion,
    half bumpScale)
{
    ASplat sp = aCreateSplat();

#ifdef A_TRIPLANAR_MAPPING_ON
    aTriPlanarX(sp, sc, A_SAMPLER_INPUT(base), material, normal, occlusion, bumpScale);
    aTriPlanarY(sp, sc, A_SAMPLER_INPUT(base), material, normal, occlusion, bumpScale);
    aTriPlanarZ(sp, sc, A_SAMPLER_INPUT(base), material, normal, occlusion, bumpScale);
#else
    float2 uv = A_TEX_TRANSFORM_UV_SCROLL(sc, base);
    half4 color = tex2D(base, uv);
    half4 mat = A_ONE4;

    #ifdef _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        mat.A_ROUGHNESS_CHANNEL = color.a;
        color.a = 1.0h;
    #else
        mat = tex2D(material, uv);
        mat.A_AO_CHANNEL = aOcclusionStrength(mat.A_AO_CHANNEL, occlusion);
    #endif

    sp.base = color;
    sp.material = mat;
    sp.normal = UnpackScaleNormal(tex2D(normal, uv), bumpScale);
#endif

    aSplatMaterial(sp, sc, tint, 0.0h, metallic, specularity, specularTint, roughness);
    return sp;
}

void aApplyTerrainSplats(
    inout ASurface s,
    half3 weights,
    ASplat sp0,
    ASplat sp1,
    ASplat sp2)
{
    ASplat sp = aCreateSplat();
    sp.base = weights.r * sp0.base + weights.g * sp1.base + weights.b * sp2.base;
    sp.material = weights.r * sp0.material + weights.g * sp1.material + weights.b * sp2.material;
    sp.normal = weights.r * sp0.normal + weights.g * sp1.normal + weights.b * sp2.normal;
    aApplySplat(s, sp);
}

void aApplyTerrainSplats(
    inout ASurface s,
    half4 weights,
    ASplat sp0,
    ASplat sp1,
    ASplat sp2,
    ASplat sp3)
{
    ASplat sp = aCreateSplat();
    sp.base = weights.r * sp0.base + weights.g * sp1.base + weights.b * sp2.base + weights.a * sp3.base;
    sp.material = weights.r * sp0.material + weights.g * sp1.material + weights.b * sp2.material + weights.a * sp3.material;
    sp.normal = weights.r * sp0.normal + weights.g * sp1.normal + weights.b * sp2.normal + weights.a * sp3.normal;
    aApplySplat(s, sp);
}

#endif // A_FRAMEWORK_SPLAT_IMPL_CGINC
