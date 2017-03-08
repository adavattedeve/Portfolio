// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Terrain.cginc
/// @brief Terrain model inputs and outputs.
///////////////////////////////////////////////////////////////////////////////

#ifndef A_MODELS_TERRAIN_CGINC
#define A_MODELS_TERRAIN_CGINC

#define A_TEX_UV_OFF
#define A_TEX_SCROLL_OFF
#define A_VERTEX_COLOR_IS_DATA
#define A_METALLIC_ON

#include "Assets/Alloy/Shaders/Framework/Model.cginc"

void aVertex(
    inout AVertex v)
{
#ifdef A_TANGENT_TO_WORLD_ON
    v.tangent.xyz = cross(v.normal, A_AXIS_Z);
    v.tangent.w = -1.0f;
#endif
}

void aFinalColor(
    inout half4 color,
    ASurface s)
{
#ifndef A_TERRAIN_NSPLAT
    UNITY_APPLY_FOG(s.fogCoord, color);
#else
    color *= s.opacity;

    #ifdef A_TERRAIN_SPLAT_ADDPASS
        UNITY_APPLY_FOG_COLOR(s.fogCoord, color, A_BLACK4);
    #else
        UNITY_APPLY_FOG(s.fogCoord, color);
    #endif
#endif
}

void aFinalGbuffer(
    inout AGbuffer gb,
    ASurface s)
{
#ifdef A_TERRAIN_NSPLAT
    gb.target0 *= s.opacity;
    gb.target1 *= s.opacity;
    gb.target2 *= s.opacity;
    gb.target3 *= s.opacity;
#endif
}

#endif // A_MODELS_TERRAIN_CGINC
