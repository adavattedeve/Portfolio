// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file DecalAdditive.cginc
/// @brief Additive deferred decal surface shader definition.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_DEFINITIONS_DECAL_ADDITIVE_CGINC
#define A_DEFINITIONS_DECAL_ADDITIVE_CGINC

#define A_MAIN_TEXTURES_ON
#define A_MAIN_TEXTURES_MATERIAL_MAP_OFF

#include "Assets/Alloy/Shaders/Models/DecalAdditive.cginc"

void aSurface(
    inout ASurface s)
{
    aParallax(s);
    aDissolve(s);
    aMainTextures(s);
    aDetail(s);
    aTeamColor(s);
    aDecal(s);
    aRim(s);
    aEmission(s);

    s.emission += s.baseColor;
}

#endif // A_DEFINITIONS_DECAL_ADDITIVE_CGINC
