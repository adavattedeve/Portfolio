// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Model.cginc
/// @brief Model type uber-header.
///////////////////////////////////////////////////////////////////////////////

#ifndef A_FRAMEWORK_MODEL_CGINC
#define A_FRAMEWORK_MODEL_CGINC

// NOTE: Config comes first to override Unity settings!
#include "Assets/Alloy/Shaders/Config.cginc"
#include "Assets/Alloy/Shaders/Framework/Surface.cginc"
#include "Assets/Alloy/Shaders/Framework/Utility.cginc"

// Features
#include "Assets/Alloy/Shaders/Features/AO2.cginc"
#include "Assets/Alloy/Shaders/Features/CarPaint.cginc"
#include "Assets/Alloy/Shaders/Features/Decal.cginc"
#include "Assets/Alloy/Shaders/Features/Detail.cginc"
#include "Assets/Alloy/Shaders/Features/DirectionalBlend.cginc"
#include "Assets/Alloy/Shaders/Features/Dissolve.cginc"
#include "Assets/Alloy/Shaders/Features/Emission.cginc"
#include "Assets/Alloy/Shaders/Features/Emission2.cginc"
#include "Assets/Alloy/Shaders/Features/Eye.cginc"
#include "Assets/Alloy/Shaders/Features/MainTextures.cginc"
#include "Assets/Alloy/Shaders/Features/OrientedTextures.cginc"
#include "Assets/Alloy/Shaders/Features/Parallax.cginc"
#include "Assets/Alloy/Shaders/Features/Puddles.cginc"
#include "Assets/Alloy/Shaders/Features/Rim.cginc"
#include "Assets/Alloy/Shaders/Features/Rim2.cginc"
#include "Assets/Alloy/Shaders/Features/SecondaryTextures.cginc"
#include "Assets/Alloy/Shaders/Features/SpeedTree.cginc"
#include "Assets/Alloy/Shaders/Features/TeamColor.cginc"
#include "Assets/Alloy/Shaders/Features/Terrain.cginc"
#include "Assets/Alloy/Shaders/Features/TransitionBlend.cginc"
#include "Assets/Alloy/Shaders/Features/Transmission.cginc"
#include "Assets/Alloy/Shaders/Features/TriPlanar.cginc"
#include "Assets/Alloy/Shaders/Features/VertexBlend.cginc"
#include "Assets/Alloy/Shaders/Features/WeightedBlend.cginc"
#include "Assets/Alloy/Shaders/Features/Wetness.cginc"

#include "HLSLSupport.cginc"
#include "UnityCG.cginc"
#include "UnityInstancing.cginc"
#include "UnityStandardBRDF.cginc"
#include "UnityStandardUtils.cginc"

#if !defined(A_UV2_ON) && (defined(DYNAMICLIGHTMAP_ON) || defined(UNITY_PASS_META))
    #define A_UV2_ON
#endif

#if !defined(A_TANGENT_TO_WORLD_ON) && !defined(A_SHADOW_META_PASS) && (defined(A_NORMAL_MAPPING_ON) || defined(A_VIEW_VECTOR_TANGENT_ON))
    #define A_TANGENT_TO_WORLD_ON

    #ifndef A_NORMAL_WORLD_ON
        #define A_NORMAL_WORLD_ON
    #endif
#endif

/// Deferred geometry buffer representation of surface data.
struct AGbuffer {
    /// RGB: Albedo, A: Specular occlusion.
    half4 target0 : SV_Target0;

    /// RGB: f0, A: 1-Roughness.
    half4 target1 : SV_Target1;

    /// RGB: Packed normal, A: Material type.
    half4 target2 : SV_Target2;

    /// RGB: Emission, A: 1-Transmission.
    half4 target3 : SV_Target3;
};

/// Vertex input from the model data.
struct AVertex 
{
    float4 vertex : POSITION;
    float4 uv0 : TEXCOORD0;
    float4 uv1 : TEXCOORD1;
    half3 normal : NORMAL;
#ifdef A_UV2_ON
    float4 uv2 : TEXCOORD2;
#endif
#ifdef A_UV3_ON
    float4 uv3 : TEXCOORD3;
#endif
#ifdef A_TANGENT_TO_WORLD_ON
    half4 tangent : TANGENT;
#endif
    half4 color : COLOR;
    UNITY_INSTANCE_ID
};

/// Converts vertex color from gamma to linear space before interpolation.
/// @param  v   Input vertex data.
void aDeGammaVertexColor(inout AVertex v);

#endif // A_FRAMEWORK_MODEL_CGINC
