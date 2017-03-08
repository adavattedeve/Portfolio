// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file DecalAmbient.cginc
/// @brief Ambient deferred decal surface shader definition.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_DEFINITIONS_DECAL_AMBIENT_CGINC
#define A_DEFINITIONS_DECAL_AMBIENT_CGINC

#include "Assets/Alloy/Shaders/Models/DecalAmbient.cginc"

void aSurface(
    inout ASurface s)
{
    half4 base = aBaseColor(s);

    s.baseColor = aLerpWhiteTo(base.rgb, base.a);
    aTeamColor(s);
}

#endif // A_DEFINITIONS_DECAL_AMBIENT_CGINC
