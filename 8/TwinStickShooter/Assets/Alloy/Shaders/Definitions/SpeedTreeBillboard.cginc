// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file SpeedTreeBillboard.cginc
/// @brief SpeedTree Billboard surface shader definition.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_DEFINITIONS_SPEED_TREE_BILLBOARD_CGINC
#define A_DEFINITIONS_SPEED_TREE_BILLBOARD_CGINC

#define A_SPEED_TREE_ON

#include "Assets/Alloy/Shaders/Models/SpeedTreeBillboard.cginc"
#include "Assets/Alloy/Shaders/Lighting/Standard.cginc"

void aSurface(
    inout ASurface s)
{
    aSpeedTree(s);

    s.roughness = 1.0h;
}

#endif // A_DEFINITIONS_SPEED_TREE_BILLBOARD_CGINC
