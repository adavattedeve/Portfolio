// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file CarPaint.cginc
/// @brief Car Paint surface shader definition.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_DEFINITIONS_CAR_PAINT_CGINC
#define A_DEFINITIONS_CAR_PAINT_CGINC

#define A_MAIN_TEXTURES_ON
#define A_CAR_PAINT_ON

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
    
    aOpacityMask(s);
    aCarPaint(s);

    aNoMask(s);
    aAo2(s);
    aDecal(s);
    aWetness(s);
    aEmission(s);
    aRim(s);
}

#endif // A_DEFINITIONS_CAR_PAINT_CGINC
