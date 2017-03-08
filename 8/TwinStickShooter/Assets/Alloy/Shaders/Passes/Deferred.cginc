// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Deferred.cginc
/// @brief Deferred g-buffer vertex & fragment passes.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_PASSES_DEFERRED_CGINC
#define A_PASSES_DEFERRED_CGINC

#define A_INSTANCING_PASS
#define A_STEREO_PASS
#define A_INDIRECT_PASS

#include "Assets/Alloy/Shaders/Framework/Semantics.cginc"

struct AVertexToFragment {
#ifndef A_LIGHTING_ON
    A_VERTEX_DATA(0, 1, 2, 3, 4, 5, 6)
#else
    A_GI_DATA(0)
    A_VERTEX_DATA(1, 2, 3, 4, 5, 6, 7)
#endif
};

#include "Assets/Alloy/Shaders/Framework/Pass.cginc"

void aVertexShader(
    AVertex v,
    out AVertexToFragment o,
    out float4 opos : SV_POSITION)
{
    aTransferVertex(v, o, opos);
    aVertexGi(o, v);
}

AGbuffer aFragmentShader(
    AVertexToFragment i
    A_FACING_TYPE)
{
    return aOutputDeferred(i, A_FACING_SIGN);
}					
            
#endif // A_PASSES_DEFERRED_CGINC
