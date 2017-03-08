// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file VertexBlend.cginc
/// @brief Vertex Blend shader definition.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_DEFINITIONS_VERTEX_BLEND_CGINC
#define A_DEFINITIONS_VERTEX_BLEND_CGINC

#define A_VERTEX_BLEND_ON

#include "Assets/Alloy/Shaders/Models/Standard.cginc"
#include "Assets/Alloy/Shaders/Lighting/Standard.cginc"

void aSurface(
    inout ASurface s)
{
    aVertexBlend(s);
    aDetail(s);
    aAo2(s);
    aDecal(s);
    aWetness(s);
}

#endif // A_DEFINITIONS_VERTEX_BLEND_CGINC
