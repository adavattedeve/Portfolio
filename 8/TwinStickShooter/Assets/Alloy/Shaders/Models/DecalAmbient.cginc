// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file DecalAmbient.cginc
/// @brief Ambient-only decal inputs and outputs.
///////////////////////////////////////////////////////////////////////////////

#ifndef A_MODELS_DECAL_AMBIENT_CGINC
#define A_MODELS_DECAL_AMBIENT_CGINC

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
    color.rgb = s.baseColor.rgb;
    color.a = 1.0h;

    // Fog to white to allow underlying surface fog to bleed through.
    UNITY_APPLY_FOG_COLOR(s.fogCoord, color, A_WHITE4);
}

void aFinalGbuffer(
    inout AGbuffer gb,
    ASurface s)
{
    gb.target0.rgb = A_WHITE;
    gb.target0.a = aLuminance(s.baseColor); // Deferred reflections.
    gb.target1 = A_WHITE4;
    gb.target2 = A_ONE4;
    gb.target3.rgb = s.baseColor;
    gb.target3.a = 1.0h;
}

#endif // A_MODELS_DECAL_AMBIENT_CGINC
