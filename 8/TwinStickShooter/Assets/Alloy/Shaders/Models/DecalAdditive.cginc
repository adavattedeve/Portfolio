// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file DecalAdditive.cginc
/// @brief Additive decal inputs and outputs.
///////////////////////////////////////////////////////////////////////////////

#ifndef A_MODELS_DECAL_ADDITIVE_CGINC
#define A_MODELS_DECAL_ADDITIVE_CGINC

#include "Assets/Alloy/Shaders/Framework/Decal.cginc"

void aVertex(
    inout AVertex v)
{
    aDeGammaVertexColor(v);
}

void aFinalColor(
    inout half4 color,
    ASurface s)
{
    // Fog to black to allow underlying surface fog to bleed through.
    UNITY_APPLY_FOG_COLOR(s.fogCoord, color, A_BLACK4);
}

void aFinalGbuffer(
    inout AGbuffer gb,
    ASurface s)
{
    gb.target0 = 0.0h;
    gb.target1 = 0.0h;
    gb.target2 = 0.0h;
    gb.target3.w = 0.0h;
}

#endif // A_MODELS_DECAL_ADDITIVE_CGINC
