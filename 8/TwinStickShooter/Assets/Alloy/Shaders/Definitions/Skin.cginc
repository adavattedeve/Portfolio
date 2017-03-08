// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Skin.cginc
/// @brief Skin surface shader definition.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_DEFINITIONS_SKIN_CGINC
#define A_DEFINITIONS_SKIN_CGINC

#define A_MAIN_TEXTURES_ON
#define A_MAIN_TEXTURES_CUTOUT_OFF

#include "Assets/Alloy/Shaders/Models/Standard.cginc"
#include "Assets/Alloy/Shaders/Lighting/Skin.cginc"

void aSurface(
    inout ASurface s)
{
    aParallax(s);
    aDissolve(s);
    aMainTextures(s);

    s.transmission = s.opacity;
    s.opacity = 1.0h;
    s.blurredNormalTangent = aSampleBumpBias(s, A_SKIN_BUMP_BLUR_BIAS);
    
    aDetail(s);
    aTeamColor(s);
    aDecal(s);
    aWetness(s);	
    aRim(s);
    aEmission(s);
}

#endif // A_DEFINITIONS_SKIN_CGINC
