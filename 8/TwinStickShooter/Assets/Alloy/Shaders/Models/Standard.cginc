// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Standard.cginc
/// @brief Standard model inputs and outputs.
///////////////////////////////////////////////////////////////////////////////

#ifndef A_MODELS_STANDARD_CGINC
#define A_MODELS_STANDARD_CGINC

#include "Assets/Alloy/Shaders/Framework/Model.cginc"

void aVertex(
    inout AVertex v)
{
    aDeGammaVertexColor(v);
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

#endif // A_MODELS_STANDARD_CGINC
