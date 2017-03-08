// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file TriPlanar.cginc
/// @brief TriPlanar shader definition.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_DEFINITIONS_TRIPLANAR_CGINC
#define A_DEFINITIONS_TRIPLANAR_CGINC

#define A_TRIPLANAR_ON
#define A_RIM_EFFECTS_MAP_OFF

#include "Assets/Alloy/Shaders/Models/Standard.cginc"
#include "Assets/Alloy/Shaders/Lighting/Standard.cginc"

void aSurface(
    inout ASurface s)
{
    aTriPlanar(s);
    aRim(s);
}

#endif // A_DEFINITIONS_TRIPLANAR_CGINC
