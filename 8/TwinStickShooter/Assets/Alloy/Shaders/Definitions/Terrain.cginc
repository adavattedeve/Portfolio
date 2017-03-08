// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Terrain.cginc
/// @brief Terrain surface shader definition.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_DEFINITIONS_TERRAIN_CGINC
#define A_DEFINITIONS_TERRAIN_CGINC

#define A_TERRAIN_ON
#define A_DETAIL_MASK_OFF

#include "Assets/Alloy/Shaders/Models/Terrain.cginc"
#include "Assets/Alloy/Shaders/Lighting/Standard.cginc"

void aSurface(
    inout ASurface s)
{
    aTerrain(s);
    aDetail(s);
}

#endif // A_DEFINITIONS_TERRAIN_CGINC
