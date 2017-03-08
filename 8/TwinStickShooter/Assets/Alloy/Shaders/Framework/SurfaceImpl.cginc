// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file SurfaceImpl.cginc
/// @brief Surface method implementations to allow disabling of features.
///////////////////////////////////////////////////////////////////////////////

#ifndef A_FRAMEWORK_SURFACE_IMPL_CGINC
#define A_FRAMEWORK_SURFACE_IMPL_CGINC

#include "Assets/Alloy/Shaders/Framework/Brdf.cginc"
#include "Assets/Alloy/Shaders/Framework/Surface.cginc"
#include "Assets/Alloy/Shaders/Framework/Utility.cginc"

#include "HLSLSupport.cginc"
#include "UnityStandardBRDF.cginc"
#include "UnityStandardUtils.cginc"

ASurface aCreateSurface() {
    ASurface s;

    UNITY_INITIALIZE_OUTPUT(ASurface, s);
    s.mask = 1.0h;
    s.opacity = 1.0h;
    s.baseColor = 1.0h;
    s.specularTint = 0.0h;
    s.clearCoat = 0.0h;
    s.clearCoatRoughness = 0.0h;
    s.metallic = 0.0h;
    s.specularity = 0.5h;
    s.roughness = 0.0h;
    s.emission = 0.0h;
    s.ambientOcclusion = 1.0h;
    s.normalTangent = A_FLAT_NORMAL;
    s.materialType = 1.0h;
    s.transmission = 0.0h;
    s.specularOcclusion = 1.0h;

    // Stop divide by zero warnings from the compiler.
    s.vertexNormalWorld = A_FLAT_NORMAL;
    s.normalWorld = A_FLAT_NORMAL;
    s.viewDirWorld = A_AXIS_X;
    s.viewDirTangent = A_AXIS_X;
    s.reflectionVectorWorld = A_AXIS_X;
    s.tangentToWorld = 0.0h;
    s.facingSign = 1.0h;
    s.fogCoord = 0.0f;
    s.NdotV = 0.0h;
    s.FV = 0.0h;

    return s;
}

void aUpdateViewData(
    inout ASurface s)
{
#if defined(A_NORMAL_WORLD_ON) && defined(A_VIEW_DIR_WORLD_ON)
    // Skip re-calculating world normals in some cases.
    #ifdef A_VIEW_VECTOR_TANGENT_ON
        s.NdotV = aDotClamp(s.normalTangent, s.viewDirTangent);
    #else
        s.NdotV = aDotClamp(s.normalWorld, s.viewDirWorld);
    #endif

    s.FV = aFresnel(s.NdotV);

    #ifdef A_REFLECTION_VECTOR_WORLD_ON
        s.reflectionVectorWorld = reflect(-s.viewDirWorld, s.normalWorld);
    #endif
#endif
}

half3 aTangentToWorld(
    ASurface s,
    half3 normalTangent)
{
#ifndef A_TANGENT_TO_WORLD_ON
    return s.vertexNormalWorld;
#else
    return normalize(mul(normalTangent, s.tangentToWorld));
#endif
}

half3 aWorldToTangent(
    ASurface s,
    half3 normalWorld)
{
#ifndef A_TANGENT_TO_WORLD_ON
    return A_FLAT_NORMAL;
#else
    return normalize(mul(s.tangentToWorld, normalWorld));
#endif
}

void aSetNormalTangent(
    inout ASurface s,
    half3 normalTangent)
{
#ifdef A_TANGENT_TO_WORLD_ON
    s.normalTangent = normalTangent;
    s.normalWorld = aTangentToWorld(s, normalTangent);
    aUpdateViewData(s);
#endif
}

void aSetNormalWorld(
    inout ASurface s,
    half3 normalWorld)
{
#ifdef A_TANGENT_TO_WORLD_ON
    s.normalTangent = aWorldToTangent(s, normalWorld);
    s.normalWorld = normalWorld;
    aUpdateViewData(s);
#endif
}

#endif // A_FRAMEWORK_SURFACE_IMPL_CGINC
