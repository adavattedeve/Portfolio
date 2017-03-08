// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file Decal.cginc
/// @brief Decal model type uber-header.
///////////////////////////////////////////////////////////////////////////////

#ifndef A_FRAMEWORK_DECAL_CGINC
#define A_FRAMEWORK_DECAL_CGINC

#ifdef A_PROJECTIVE_DECAL_ON
    #define A_TEX_UV_OFF
    #define A_VERTEX_COLOR_IS_DATA
#endif

#include "Assets/Alloy/Shaders/Framework/Model.cginc"

#endif // A_FRAMEWORK_DECAL_CGINC
