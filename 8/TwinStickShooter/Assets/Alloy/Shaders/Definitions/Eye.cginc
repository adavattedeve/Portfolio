// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Eye.cginc
/// @brief Eye surface shader definition.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_DEFINITIONS_EYE_CGINC
#define A_DEFINITIONS_EYE_CGINC

#define A_EYE_ON
#define A_DETAIL_MASK_OFF
#define A_DETAIL_COLOR_MAP_OFF

#include "Assets/Alloy/Shaders/Models/Standard.cginc"
#include "Assets/Alloy/Shaders/Lighting/Standard.cginc"

void aSurface(
    inout ASurface s)
{
    aEye(s);
    aDetail(s);

    aNoMask(s);
    aDissolve(s);
    aDecal(s);
    aEmission(s);
    aRim(s);
}

#endif // A_DEFINITIONS_EYE_CGINC
