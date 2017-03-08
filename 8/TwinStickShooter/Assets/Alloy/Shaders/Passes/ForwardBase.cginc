// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file ForwardBase.cginc
/// @brief Forward Base lighting vertex & fragment passes.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_PASSES_FORWARD_BASE_CGINC
#define A_PASSES_FORWARD_BASE_CGINC

#define A_INSTANCING_PASS
#define A_STEREO_PASS
#define A_DIRECT_PASS
#define A_INDIRECT_PASS

#include "Assets/Alloy/Shaders/Framework/Semantics.cginc"

#include "AutoLight.cginc"

struct AVertexToFragment {
#ifndef A_LIGHTING_ON
    A_VERTEX_DATA(0, 1, 2, 3, 4, 5, 6)
#else
    A_GI_DATA(0)
    SHADOW_COORDS(1)
    A_VERTEX_DATA(2, 3, 4, 5, 6, 7, 8)
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
    A_TRANSFER_SHADOW(o);
}

half4 aFragmentShader(
    AVertexToFragment i
    A_FACING_TYPE) : SV_Target
{
    ASurface s = aForwardSurface(i, A_FACING_SIGN);
    half3 illum = s.emission;

#ifdef A_LIGHTING_ON
    half shadow = SHADOW_ATTENUATION(i);
    UnityGI gi = aFragmentGi(i, s, shadow);

    illum += aGlobalIllumination(gi, s);

    #ifdef LIGHTMAP_OFF
        illum += aForwardDirect(s, shadow, _WorldSpaceLightPos0, A_ZERO4);
    #endif
#endif

    return aOutputForward(s, illum);
}
            
#endif // A_PASSES_FORWARD_BASE_CGINC
