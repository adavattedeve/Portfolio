// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Semantics.cginc
/// @brief Semantics for passing Vertex data to the Fragment shader.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_FRAMEWORK_SEMANTICS_CGINC
#define A_FRAMEWORK_SEMANTICS_CGINC

#ifdef VTRANSPARENCY_ON
    #ifndef A_SCREEN_UV_ON
        #define A_SCREEN_UV_ON
    #endif
    
    #ifndef A_VIEW_DEPTH_ON
        #define A_VIEW_DEPTH_ON
    #endif
#endif

#ifdef A_LIGHTING_ON
    #ifdef A_DIRECT_PASS
        #define A_DIRECT_ON
    #endif

    #ifdef A_INDIRECT_PASS
        #define A_INDIRECT_ON
    #endif
#endif

#if defined(A_DIRECT_ON) || defined(A_INDIRECT_ON)
    #ifndef A_POSITION_WORLD_ON
        #define A_POSITION_WORLD_ON
    #endif

    #ifndef A_NORMAL_WORLD_ON
        #define A_NORMAL_WORLD_ON
    #endif

    #ifndef A_VIEW_DIR_WORLD_ON
        #define A_VIEW_DIR_WORLD_ON
    #endif
        
    #if !defined(A_REFLECTION_VECTOR_WORLD_ON) && (defined(A_DIRECT_ON) || defined(A_REFLECTION_PROBES_ON))
        #define A_REFLECTION_VECTOR_WORLD_ON
    #endif
#endif

// NOTE: Custom macro to save calculations and remove dependency on o.pos!
#ifndef A_DIRECT_ON
    #define A_TRANSFER_SHADOW(a)
#elif !defined(SHADOWS_SCREEN) || defined(UNITY_NO_SCREENSPACE_SHADOWS)
    #define A_TRANSFER_SHADOW(a) TRANSFER_SHADOW(a)
#else
    #define A_COMPUTE_VERTEX_SCREEN_UV
    #define A_TRANSFER_SHADOW(a) a._ShadowCoord = unityShadowCoord4(a.fogCoord.y, a.fogCoord.z, 0.0, a.fogCoord.w);
#endif

#if !defined(A_SCREEN_UV_ON) && defined(LOD_FADE_CROSSFADE)
    #define A_SCREEN_UV_ON
#endif

#if defined(A_TWO_SIDED_ON) && defined(A_NORMAL_WORLD_ON)
    #define A_TWO_SIDED_NORMALS
#endif

#if !defined(A_VIEW_DIR_WORLD_ON) && (defined(A_VIEW_VECTOR_TANGENT_ON) || defined(A_REFLECTION_VECTOR_WORLD_ON))
    #define A_VIEW_DIR_WORLD_ON
#endif

#if !defined(A_POSITION_TEXCOORD_ON) && (defined(A_POSITION_WORLD_ON) || defined(A_VIEW_DEPTH_ON))
    #define A_POSITION_TEXCOORD_ON
#endif

#if defined(A_FOG_ON) || defined(A_SCREEN_UV_ON) || defined(A_COMPUTE_VERTEX_SCREEN_UV)
    #define A_FOG_TEXCOORD_ON
#endif

#if defined(A_WORLD_TO_OBJECT_ON) && defined(A_INSTANCING_PASS)
    #define A_TRANSFER_INSTANCE_ID_ON
#endif

// Split vertex data conditions so we only need 8 cases rather than 16.
// UV0-1, TBN, N, V, R, N.V, and F(N.V).
#if defined(A_TANGENT_TO_WORLD_ON)    
    #define A_INNER_VERTEX_DATA2(A, B, C, D) \
        float4 texcoords : TEXCOORD##A; \
        half4 tangentWorld : TEXCOORD##B; \
        half4 bitangentWorld : TEXCOORD##C; \
        half4 normalWorld : TEXCOORD##D;
#elif defined(A_NORMAL_WORLD_ON) && defined(A_REFLECTION_VECTOR_WORLD_ON) && !defined(A_TWO_SIDED_NORMALS)
    #define A_INNER_VERTEX_DATA2(A, B, C, D) \
        float4 texcoords : TEXCOORD##A; \
        half4 normalWorld : TEXCOORD##B; \
        half4 viewDirWorld : TEXCOORD##C; \
        half3 reflectionVectorWorld : TEXCOORD##D;
#elif defined(A_NORMAL_WORLD_ON) && defined(A_VIEW_DIR_WORLD_ON)
    #define A_INNER_VERTEX_DATA2(A, B, C, D) \
        float4 texcoords : TEXCOORD##A; \
        half4 normalWorld : TEXCOORD##B; \
        half4 viewDirWorld : TEXCOORD##C;
#elif defined(A_NORMAL_WORLD_ON)
    #define A_INNER_VERTEX_DATA2(A, B, C, D) \
        float4 texcoords : TEXCOORD##A; \
        half3 normalWorld : TEXCOORD##B;
#elif defined(A_VIEW_DIR_WORLD_ON)
    #define A_INNER_VERTEX_DATA2(A, B, C, D) \
        float4 texcoords : TEXCOORD##A; \
        half3 viewDirWorld : TEXCOORD##B;
#else
    #define A_INNER_VERTEX_DATA2(A, B, C, D) \
        float4 texcoords : TEXCOORD##A;
#endif

// Vertex Color, Position, View Depth, Fog, and Screen UV.
#if defined(A_POSITION_TEXCOORD_ON) && defined(A_FOG_TEXCOORD_ON)
    #define A_INNER_VERTEX_DATA1(A, B, C, D, E, F, G) \
        half4 color : TEXCOORD##A; \
        float4 positionWorldAndViewDepth : TEXCOORD##B; \
        UNITY_FOG_COORDS_PACKED(C, half4) \
        A_INNER_VERTEX_DATA2(D, E, F, G)
#elif defined(A_POSITION_TEXCOORD_ON)
    #define A_INNER_VERTEX_DATA1(A, B, C, D, E, F, G) \
        half4 color : TEXCOORD##A; \
        float4 positionWorldAndViewDepth : TEXCOORD##B; \
        A_INNER_VERTEX_DATA2(C, D, E, F)
#elif defined(A_FOG_TEXCOORD_ON)
    #define A_INNER_VERTEX_DATA1(A, B, C, D, E, F, G) \
        half4 color : TEXCOORD##A; \
        UNITY_FOG_COORDS_PACKED(C, half4) \
        A_INNER_VERTEX_DATA2(D, E, F, G)
#else
    #define A_INNER_VERTEX_DATA1(A, B, C, D, E, F, G) \
        half4 color : TEXCOORD##A; \
        A_INNER_VERTEX_DATA2(B, C, D, E)
#endif

// Instancing
#if defined(A_TRANSFER_INSTANCE_ID_ON) && defined(A_STEREO_PASS)
    #define A_VERTEX_DATA(A, B, C, D, E, F, G) A_INNER_VERTEX_DATA1(A, B, C, D, E, F, G) UNITY_INSTANCE_ID UNITY_VERTEX_OUTPUT_STEREO
#elif defined(A_TRANSFER_INSTANCE_ID_ON)
    #define A_VERTEX_DATA(A, B, C, D, E, F, G) A_INNER_VERTEX_DATA1(A, B, C, D, E, F, G) UNITY_INSTANCE_ID
#elif defined(A_STEREO_PASS)
    #define A_VERTEX_DATA(A, B, C, D, E, F, G) A_INNER_VERTEX_DATA1(A, B, C, D, E, F, G) UNITY_VERTEX_OUTPUT_STEREO
#else
    #define A_VERTEX_DATA(A, B, C, D, E, F, G) A_INNER_VERTEX_DATA1(A, B, C, D, E, F, G)
#endif

// Packed GI data.
#define A_GI_DATA(n) half4 giData : TEXCOORD##n;

#endif // A_FRAMEWORK_SEMANTICS_CGINC
