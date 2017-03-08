// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file OrientedBlend.cginc
/// @brief Oriented Blend shader definition.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_DEFINITIONS_ORIENTED_BLEND_CGINC
#define A_DEFINITIONS_ORIENTED_BLEND_CGINC

#define A_MAIN_TEXTURES_ON
#define A_MAIN_TEXTURES_CUTOUT_OFF
#define A_DIRECTIONAL_BLEND_ON
#define A_DIRECTIONAL_BLEND_MODE_OFF
#define A_ORIENTED_TEXTURES_ON

#include "Assets/Alloy/Shaders/Models/Standard.cginc"
#include "Assets/Alloy/Shaders/Lighting/Standard.cginc"
    
void aSurface(
    inout ASurface s)
{
    aParallax(s);
    aDissolve(s);
    aMainTextures(s);
    aDetail(s);
    aTeamColor(s);
    aDecal(s);
    aEmission(s);

    aDirectionalBlend(s);
    aOrientedTextures(s);
    aCutout(s);

    aNoMask(s);
    aAo2(s);
    aWetness(s);
    aRim(s);
}

#endif // A_DEFINITIONS_ORIENTED_BLEND_CGINC
