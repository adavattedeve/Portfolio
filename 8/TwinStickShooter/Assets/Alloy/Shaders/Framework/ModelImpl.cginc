// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

///////////////////////////////////////////////////////////////////////////////
/// @file ModelImpl.cginc
/// @brief Model method implementations to allow disabling of features.
///////////////////////////////////////////////////////////////////////////////

#ifndef A_FRAMEWORK_MODEL_IMPL_CGINC
#define A_FRAMEWORK_MODEL_IMPL_CGINC

#include "Assets/Alloy/Shaders/Framework/Model.cginc"

void aDeGammaVertexColor(
    inout AVertex v)
{
#ifndef A_VERTEX_COLOR_IS_DATA
    v.color.rgb = aGammaToLinear(v.color.rgb);
#endif
}

#endif // A_FRAMEWORK_MODEL_IMPL_CGINC
