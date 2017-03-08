// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file SpeedTree.cginc
/// @brief SpeedTree model inputs and outputs.
///////////////////////////////////////////////////////////////////////////////

#ifndef A_MODELS_SPEED_TREE_CGINC
#define A_MODELS_SPEED_TREE_CGINC

#define A_TEX_UV_OFF
#define A_UV2_ON
#define A_UV3_ON
#define A_VERTEX_COLOR_IS_DATA
#define A_AMBIENT_OCCLUSION_ON

#define ENABLE_WIND
#define SPEEDTREE_Y_UP

#ifdef GEOM_TYPE_BRANCH_DETAIL
    #define GEOM_TYPE_BRANCH
#endif

#if defined(GEOM_TYPE_FROND) || defined(GEOM_TYPE_LEAF) || defined(GEOM_TYPE_FACING_LEAF)
    #define _ALPHATEST_ON
    #define A_TWO_SIDED_ON
#endif

#include "Assets/Alloy/Shaders/Framework/Model.cginc"

#include "SpeedTreeVertex.cginc"

void aVertex(
    inout AVertex v)
{
    v.color.r = aAmbientOcclusion(aGammaToLinear(v.color.r).rrrr);

#ifdef EFFECT_HUE_VARIATION
    float hueVariationAmount = frac(unity_ObjectToWorld[0].w + unity_ObjectToWorld[1].w + unity_ObjectToWorld[2].w);
    hueVariationAmount += frac(v.vertex.x + v.normal.y + v.normal.x) * 0.5f - 0.3f;
    v.color.b = saturate(hueVariationAmount * _HueVariation.a);
#endif

    // Adapt vertex data so we can reuse wind code.
    SpeedTreeVB IN;

    UNITY_INITIALIZE_OUTPUT(SpeedTreeVB, IN);
    IN.vertex = v.vertex;
    IN.normal = v.normal;
    IN.texcoord = v.uv0;
    IN.texcoord1 = v.uv1;
    IN.texcoord2 = v.uv2;
    IN.texcoord3 = v.uv3;
    IN.color = v.color;
    
    OffsetSpeedTreeVertex(IN, unity_LODFade.x);
    v.vertex = IN.vertex;

    // NOTE: Down here since it hijacks uv1 to pass uv2.
#ifdef GEOM_TYPE_BRANCH_DETAIL
    v.uv1.xy = v.uv2.xy;
    v.color.g = v.color.a == 0 ? v.uv2.z : 2.5f;
#endif
}

void aFinalColor(
    inout half4 color,
    ASurface s)
{
    UNITY_APPLY_FOG(s.fogCoord, color);
}

void aFinalGbuffer(
    inout AGbuffer gb,
    ASurface s)
{

}

#endif // A_MODELS_SPEED_TREE_CGINC
