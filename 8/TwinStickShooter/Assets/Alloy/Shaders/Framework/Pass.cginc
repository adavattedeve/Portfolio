// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Pass.cginc
/// @brief Passes uber-header.
///////////////////////////////////////////////////////////////////////////////

#ifndef A_FRAMEWORK_PASS_CGINC
#define A_FRAMEWORK_PASS_CGINC

#include "Assets/Alloy/Shaders/Framework/Brdf.cginc"
#include "Assets/Alloy/Shaders/Framework/FeatureImpl.cginc"
#include "Assets/Alloy/Shaders/Framework/ModelImpl.cginc"
#include "Assets/Alloy/Shaders/Framework/SurfaceImpl.cginc"
#include "Assets/Alloy/Shaders/Framework/Tessellation.cginc"
#include "Assets/Alloy/Shaders/Framework/Unity.cginc"
#include "Assets/Alloy/Shaders/Framework/Utility.cginc"

#include "HLSLSupport.cginc"
#include "Lighting.cginc"
#include "UnityCG.cginc"
#include "UnityGlobalIllumination.cginc"
#include "UnityInstancing.cginc"
#include "UnityLightingCommon.cginc"
#include "UnityShaderVariables.cginc"
#include "UnityStandardBRDF.cginc"
#include "UnityStandardUtils.cginc"

#ifdef A_TWO_SIDED_ON
    #define A_FACING_TYPE ,half facingSign : VFACE
    #define A_FACING_SIGN facingSign
#else
    #define A_FACING_TYPE
    #define A_FACING_SIGN 1.0h
#endif

/// Transfers the per-vertex surface data to the pixel shader.
/// @param[in]  v       Vertex input data.
/// @param[out] o       Vertex to fragment transfer data.
/// @param[out] opos    Clip space position.
void aTransferVertex(
    AVertex v,
    out AVertexToFragment o, 
    out float4 opos)
{
#ifdef A_SURFACE_SHADER_OFF
    opos = 0.0h;
#else
    UNITY_INITIALIZE_OUTPUT(AVertexToFragment, o);

    #ifdef A_INSTANCING_PASS
        UNITY_SETUP_INSTANCE_ID(v);

        #ifdef A_TRANSFER_INSTANCE_ID_ON
            UNITY_TRANSFER_INSTANCE_ID(v, o);
        #endif

        #ifdef A_STEREO_PASS
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        #endif
    #endif

    aVertex(v);

    o.color = v.color; // Gamma-space vertex color, unless modified.
    o.texcoords.xy = v.uv0.xy;
    o.texcoords.zw = v.uv1.xy;
    opos = UnityObjectToClipPos(v.vertex.xyz);
    
    float3 positionWorld = mul(unity_ObjectToWorld, v.vertex).xyz;

    #ifdef A_POSITION_TEXCOORD_ON
        #ifdef A_POSITION_WORLD_ON
            o.positionWorldAndViewDepth.xyz = positionWorld;
        #endif

        #ifdef A_VIEW_DEPTH_ON
            COMPUTE_EYEDEPTH(o.positionWorldAndViewDepth.w);
        #endif
    #endif
        
    #ifndef A_TANGENT_TO_WORLD_ON
        #ifdef A_NORMAL_WORLD_ON
            o.normalWorld.xyz = UnityObjectToWorldNormal(v.normal);
        #endif

        #ifdef A_VIEW_DIR_WORLD_ON
            o.viewDirWorld.xyz = normalize(UnityWorldSpaceViewDir(positionWorld));
        #endif

        #ifndef A_TWO_SIDED_NORMALS
            #if defined(A_NORMAL_WORLD_ON) && defined(A_VIEW_DIR_WORLD_ON)
                o.normalWorld.w = aDotClamp(o.normalWorld, o.viewDirWorld);
                o.viewDirWorld.w = aFresnel(o.normalWorld.w);
            #endif

            #ifdef A_REFLECTION_VECTOR_WORLD_ON
                o.reflectionVectorWorld = reflect(-o.viewDirWorld.xyz, o.normalWorld.xyz);
            #endif
        #endif
    #else
        float3 normalWorld = UnityObjectToWorldNormal(v.normal);
        float4 tangentWorld = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
        float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);

        o.tangentWorld.xyz = tangentToWorld[0];
        o.bitangentWorld.xyz = tangentToWorld[1];
        o.normalWorld.xyz = tangentToWorld[2];

        #ifdef A_VIEW_DIR_WORLD_ON
            half3 viewDirWorld = UnityWorldSpaceViewDir(positionWorld);
            o.tangentWorld.w = viewDirWorld.x;
            o.bitangentWorld.w = viewDirWorld.y;
            o.normalWorld.w = viewDirWorld.z;
        #endif
    #endif
        
    #ifdef A_FOG_TEXCOORD_ON
        #if defined(A_SCREEN_UV_ON) || defined(A_COMPUTE_VERTEX_SCREEN_UV)
            o.fogCoord.yzw = ComputeScreenPos(opos).xyw;
        #else
            o.fogCoord.yzw = A_AXIS_Z;
        #endif

        UNITY_TRANSFER_FOG(o, opos);
    #endif
#endif
}

/// Create a ASurface populated with data from the vertex shader.
/// @param  i           Vertex to fragment transfer data.
/// @param  facingSign  Sign of front/back facing direction.
/// @return             Initialized surface data object.
ASurface aForwardSurface(
    AVertexToFragment i,
    half facingSign)
{
    ASurface s = aCreateSurface();

#ifndef A_SURFACE_SHADER_OFF
    #ifdef A_TRANSFER_INSTANCE_ID_ON
        UNITY_SETUP_INSTANCE_ID(i);
    #endif

    s.uv01 = i.texcoords;
    s.vertexColor = i.color;
    s.facingSign = facingSign;

    #ifdef A_POSITION_TEXCOORD_ON
        #ifdef A_POSITION_WORLD_ON
            s.positionWorld = i.positionWorldAndViewDepth.xyz;
        #endif
        
        #ifdef A_VIEW_DEPTH_ON
            s.viewDepth = i.positionWorldAndViewDepth.w;
        #endif
    #endif

    #ifdef A_TWO_SIDED_NORMALS
        i.normalWorld.xyz *= facingSign;
    #endif

    #ifndef A_TANGENT_TO_WORLD_ON
        #ifdef A_NORMAL_WORLD_ON
            s.tangentToWorld = half3x3(half3(0.0h, 0.0h, 0.0h), half3(0.0h, 0.0h, 0.0h), i.normalWorld.xyz);
        #endif

        #ifdef A_VIEW_DIR_WORLD_ON
            s.viewDirWorld = normalize(i.viewDirWorld.xyz);
        #endif

        #ifndef A_TWO_SIDED_NORMALS
            #if defined(A_NORMAL_WORLD_ON) && defined(A_VIEW_DIR_WORLD_ON)
                s.NdotV = i.normalWorld.w;
                s.FV = i.viewDirWorld.w;
            #endif

            #ifdef A_REFLECTION_VECTOR_WORLD_ON
                s.reflectionVectorWorld = i.reflectionVectorWorld;
            #endif
        #endif
    #else
        half3 t = i.tangentWorld.xyz;
        half3 b = i.bitangentWorld.xyz;
        half3 n = i.normalWorld.xyz;
        
        #if UNITY_TANGENT_ORTHONORMALIZE
            n = normalize(n);
    
            // ortho-normalize Tangent
            t = normalize (t - n * dot(t, n));

            // recalculate Binormal
            half3 newB = cross(n, t);
            b = newB * sign (dot (newB, b));
        #endif

        s.tangentToWorld = half3x3(t, b, n);

        #ifdef A_VIEW_DIR_WORLD_ON
            s.viewDirWorld = normalize(half3(i.tangentWorld.w, i.bitangentWorld.w, i.normalWorld.w));

            #ifdef A_VIEW_VECTOR_TANGENT_ON
                // IMPORTANT: Calculated in the pixel shader to fix distortion issues in POM!
                s.viewDirTangent = normalize(mul(s.tangentToWorld, s.viewDirWorld));
            #endif
        #endif
    #endif

    #ifdef A_NORMAL_WORLD_ON
        // Give these sane defaults in case the surface shader doesn't set them.
        s.vertexNormalWorld = normalize(i.normalWorld.xyz);
        s.normalWorld = s.vertexNormalWorld;
        s.ambientNormalWorld = s.vertexNormalWorld;
    #endif

    #if defined(A_TANGENT_TO_WORLD_ON) || defined(A_TWO_SIDED_NORMALS)
        aUpdateViewData(s);
    #endif
        
    #ifdef A_FOG_TEXCOORD_ON
        #ifdef A_FOG_ON
            s.fogCoord = i.fogCoord;
        #endif

        #ifdef A_SCREEN_UV_ON
            s.screenPosition.xyw = i.fogCoord.yzw;
            s.screenPosition.z = 0.0h;

            s.screenUv.xy = s.screenPosition.xy / s.screenPosition.w;

            #ifdef LOD_FADE_CROSSFADE
                half2 projUV = s.screenUv.xy * _ScreenParams.xy * 0.25h;
                projUV.y = frac(projUV.y) * 0.0625h /* 1/16 */ + unity_LODFade.y; // quantized lod fade by 16 levels
                clip(tex2D(_DitherMaskLOD2D, projUV).a - 0.5f);
            #endif
        #endif
    #endif

    // Runs the shader and lighting type's surface code.
    aSetDefaultBaseUv(s);

    #ifdef A_LIGHTING_ON
        aPreSurface(s);
    #endif

    aSurface(s);
    aUpdateMetalRoughnessData(s);
#endif
    return s;
}

/// Transfers the per-vertex lightmapping or SH data to the fragment shader.
/// @param[in,out]  i   Vertex to fragment transfer data.
/// @param[in]      v   Vertex input data.
void aVertexGi(
    inout AVertexToFragment i,
    AVertex v)
{
#ifdef A_INDIRECT_ON
    #ifndef LIGHTMAP_OFF
        i.giData.xy = v.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
        i.giData.zw = 0.0h;
    #elif UNITY_SHOULD_SAMPLE_SH
        // Add approximated illumination from non-important point lights
        half3 normalWorld = i.normalWorld.xyz;

        #ifdef VERTEXLIGHT_ON
            i.giData.rgb = aShade4PointLights(i.positionWorldAndViewDepth.xyz, normalWorld);
        #endif

        i.giData.rgb = ShadeSHPerVertex(normalWorld, i.giData.rgb);
    #endif

    #ifdef DYNAMICLIGHTMAP_ON
        i.giData.zw = v.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif
#endif
}

/// Populates a UnityGI descriptor in the fragment shader.
/// @param  i       Vertex to fragment transfer data.
/// @param  s       Material surface data.
/// @param  shadow  Forward Base directional light shadow.
/// @return         Initialized UnityGI descriptor.
UnityGI aFragmentGi(
    AVertexToFragment i,
    ASurface s,
    half shadow)
{
    UnityGI gi;
    UNITY_INITIALIZE_OUTPUT(UnityGI, gi);

#ifdef A_INDIRECT_ON
    UnityGIInput d;

    UNITY_INITIALIZE_OUTPUT(UnityGIInput, d);
    d.worldPos = s.positionWorld;
    d.worldViewDir = -s.viewDirWorld; // ???
    d.atten = shadow;

    #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
        d.ambient = 0;
        d.lightmapUV = i.giData;
    #else
        d.ambient = i.giData.rgb;
        d.lightmapUV = 0;
    #endif

    d.boxMax[0] = unity_SpecCube0_BoxMax;
    d.boxMin[0] = unity_SpecCube0_BoxMin;
    d.probePosition[0] = unity_SpecCube0_ProbePosition;
    d.probeHDR[0] = unity_SpecCube0_HDR;

    d.boxMax[1] = unity_SpecCube1_BoxMax;
    d.boxMin[1] = unity_SpecCube1_BoxMin;
    d.probePosition[1] = unity_SpecCube1_ProbePosition;
    d.probeHDR[1] = unity_SpecCube1_HDR;

    // Pass 1.0 for occlusion so we can apply it later in indirect().  
    gi = UnityGI_Base(d, 1.0h, s.ambientNormalWorld);

    #ifdef A_REFLECTION_PROBES_ON
        Unity_GlossyEnvironmentData g;

        g.reflUVW = s.reflectionVectorWorld;
        g.roughness = s.roughness;
        gi.indirect.specular = UnityGI_IndirectSpecular(d, 1.0h, s.normalWorld, g);
    #endif
#endif

    return gi;
}

/// Final processing of the forward output.
/// @param  s       Material surface data.
/// @param  color   Lighting + Emission + Fog + etc.
/// @return         Final HDR output color with alpha opacity.
half4 aOutputForward(
    ASurface s,
    half3 color)
{
    half4 output;
    output.rgb = color;

#ifdef A_ALPHA_BLENDING_ON
    output.a = s.opacity;
#else
    UNITY_OPAQUE_ALPHA(output.a);
#endif

#ifdef VTRANSPARENCY_ON
    float4 data = s.screenPosition;

    data.z = s.viewDepth;

    #if defined(UNITY_PASS_FORWARDBASE) || defined(A_PASS_DISTORT)
        output = VolumetricTransparencyBase(output, data);
    #else
        output = VolumetricTransparencyAdd(output, data);
    #endif
#endif

    aFinalColor(output, s);
    return aHdrClamp(output);
}

/// Final processing of the deferred output.
/// @param  i           Vertex to fragment transfer data.
/// @param  facingSign  Sign of front/back facing direction.
/// @return             G-buffer with surface data and ambient illumination.
AGbuffer aOutputDeferred(
    AVertexToFragment i,
    half facingSign)
{
    AGbuffer gb;
    ASurface s = aForwardSurface(i, facingSign);
    UnityGI gi = aFragmentGi(i, s, 1.0h);

    UNITY_INITIALIZE_OUTPUT(AGbuffer, gb);
    gb.target3 = aUnityOutputDeferred(s, gi, gb.target0, gb.target1, gb.target2);
    
#ifndef UNITY_HDR_ON
    gb.target3.rgb = exp2(-gb.target3.rgb);
#endif

    aFinalGbuffer(gb, s);
    return gb;
}

#endif // A_FRAMEWORK_PASS_CGINC
