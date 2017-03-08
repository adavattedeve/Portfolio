// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file ForwardAdd.cginc
/// @brief Forward Add lighting vertex & fragment passes.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_PASSES_FORWARD_ADD_CGINC
#define A_PASSES_FORWARD_ADD_CGINC

#define A_DIRECT_PASS

#include "Assets/Alloy/Shaders/Framework/Semantics.cginc"

#include "AutoLight.cginc"

struct AVertexToFragment {
    float4 lightVectorRange : TEXCOORD0;
    unityShadowCoord4 lightCoord : TEXCOORD1;
    SHADOW_COORDS(2)
    A_VERTEX_DATA(3, 4, 5, 6, 7, 8, 9)
};

#include "Assets/Alloy/Shaders/Framework/Pass.cginc"

void aVertexShader(
    AVertex v,
    out AVertexToFragment o,
    out float4 opos : SV_POSITION)
{
    aTransferVertex(v, o, opos);
    aLightVectorRangeCoord(o.positionWorldAndViewDepth.xyz, o.lightVectorRange, o.lightCoord);
    A_TRANSFER_SHADOW(o)
}

half4 aFragmentShader(
    AVertexToFragment i
    A_FACING_TYPE) : SV_Target
{
    ASurface s = aForwardSurface(i, A_FACING_SIGN);
    half3 illum = aForwardDirect(s, SHADOW_ATTENUATION(i), i.lightVectorRange, i.lightCoord);
    
    return aOutputForward(s, illum);
}			
            
#endif // A_PASSES_FORWARD_ADD_CGINC
