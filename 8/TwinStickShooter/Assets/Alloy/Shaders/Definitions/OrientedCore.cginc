// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Oriented.cginc
/// @brief Oriented Blend & Core shader definition.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_DEFINITIONS_ORIENTED_CGINC
#define A_DEFINITIONS_ORIENTED_CGINC

#define A_ORIENTED_TEXTURES_ON
#define A_ORIENTED_TEXTURES_BLEND_OFF

#include "Assets/Alloy/Shaders/Models/Standard.cginc"
#include "Assets/Alloy/Shaders/Lighting/Standard.cginc"
    
void aSurface(
    inout ASurface s)
{    
    aOrientedTextures(s);
    aCutout(s);
}

#endif // A_DEFINITIONS_ORIENTED_CGINC
