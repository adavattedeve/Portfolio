// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file SpeedTree.cginc
/// @brief SpeedTree surface shader definition.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_DEFINITIONS_SPEED_TREE_CGINC
#define A_DEFINITIONS_SPEED_TREE_CGINC

#define A_SPECULAR_TINT_ON
#define A_SPEED_TREE_ON
#define A_TRANSMISSION_ON
#define A_TRANSMISSION_ALPHA

#include "Assets/Alloy/Shaders/Models/SpeedTree.cginc"

#ifdef A_TWO_SIDED_ON
    #include "Assets/Alloy/Shaders/Lighting/Transmission.cginc"
#else
    #include "Assets/Alloy/Shaders/Lighting/Standard.cginc"
#endif

void aSurface(
    inout ASurface s)
{
    aParallax(s);
    aDissolve(s);
    aSpeedTree(s);

#ifdef A_TWO_SIDED_ON
    aTransmission(s);
#endif

    s.specularity = _Specularity;
    s.specularTint = _SpecularTint;
    s.roughness = _Roughness;

    aTeamColor(s);
    aDecal(s);
    aWetness(s);
    aRim(s);
    aEmission(s);
}

#endif // A_DEFINITIONS_SPEED_TREE_CGINC
