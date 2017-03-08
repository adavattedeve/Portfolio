// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file FeatureImpl.cginc
/// @brief Feature method implementations to allow disabling of features.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_FRAMEWORK_FEATURE_IMPL_CGINC
#define A_FRAMEWORK_FEATURE_IMPL_CGINC

#include "Assets/Alloy/Shaders/Framework/Feature.cginc"
#include "Assets/Alloy/Shaders/Framework/SplatImpl.cginc"

void aTwoSided(
    inout ASurface s)
{
#ifdef A_TWO_SIDED_ON
    s.normalTangent.xy = s.facingSign > 0.0h || _TransInvertBackNormal < 0.5h ? s.normalTangent.xy : -s.normalTangent.xy;
    aSetNormalTangent(s, s.normalTangent);
#endif
}

void aOpacityMask(
    inout ASurface s)
{
    s.mask = s.opacity;
}

void aNoMask(
    inout ASurface s)
{
    s.mask = 1.0h;
}

void aInvertMask(
    inout ASurface s)
{
    s.mask = 1.0h - s.mask;
}

void aChannelPickerMask(
    inout ASurface s,
    half4 masks,
    half4 channels)
{
    s.mask = aDotClamp(masks, channels);
}

void aBlendRangeMask(
    inout ASurface s,
    half mask,
    half weight,
    half cutoff,
    half blendRange,
    half vertexTint)
{
    cutoff = lerp(cutoff, 1.0h, s.vertexColor.a * vertexTint);
    mask = 1.0h - saturate((mask - cutoff) / blendRange);
    s.mask *= weight * mask;
}

void aSetDefaultBaseUv(
    inout ASurface s)
{
    A_SET_BASE_UV_SCROLL(s, _MainTex);
}

void aSetBaseUv(
    inout ASurface s,
    float2 baseUv)
{
    s.baseUv = baseUv;

#ifdef _VIRTUALTEXTURING_ON
    s.baseVirtualCoord = VTUpdateVirtualCoord(s.baseVirtualCoord, s.baseUv);
#endif
}

void aCutout(
    ASurface s)
{
#ifdef _ALPHATEST_ON
    clip(s.opacity - _Cutoff);
#endif
}

half4 aSampleBase(
    ASurface s) 
{
    half4 result = 0.0h;

#ifdef _VIRTUALTEXTURING_ON
    result = VTSampleBase(s.baseVirtualCoord);
#else
    result = tex2D(_MainTex, s.baseUv);
#endif
    
    return result;
}

half4 aSampleMaterial(
    ASurface s) 
{
    half4 result = 0.0h;

#ifdef A_EXPANDED_MATERIAL_MAPS
    half3 channels;

    // Assuming sRGB texture sampling, undo it for all but AO.
    channels.x = tex2D(_MetallicMap, s.baseUv).g;
    channels.y = tex2D(_SpecularityMap, s.baseUv).g;
    channels.z = tex2D(_RoughnessMap, s.baseUv).g;
    channels = LinearToGammaSpace(channels);

    result.A_METALLIC_CHANNEL = channels.x;
    result.A_AO_CHANNEL = tex2D(_AoMap, s.baseUv).g;
    result.A_SPECULARITY_CHANNEL = channels.y;
    result.A_ROUGHNESS_CHANNEL = channels.z;
#else
    #ifdef _VIRTUALTEXTURING_ON
        result = VTSampleSpecular(s.baseVirtualCoord);
    #else
        result = tex2D(_SpecTex, s.baseUv);
    #endif

    // Converts AO from gamma to linear
    result.A_AO_CHANNEL = aGammaToLinear(result.A_AO_CHANNEL);
#endif

    return result;
}

half3 aSampleBumpScale(
    ASurface s,
    half scale)
{
    half4 result = 0.0h;

#ifdef _VIRTUALTEXTURING_ON
    result = VTSampleNormal(s.baseVirtualCoord);
#else
    result = tex2D(_BumpMap, s.baseUv);
#endif

    return UnpackScaleNormal(result, scale);
}

half3 aSampleBump(
    ASurface s) 
{
    return aSampleBumpScale(s, _BumpScale);
}

half3 aSampleBumpBias(
    ASurface s,
    float bias) 
{
    half4 result = 0.0h;

#ifdef _VIRTUALTEXTURING_ON
    result = VTSampleNormal(VTComputeVirtualCoord(s.baseUv, bias));
#else
    result = tex2Dbias(_BumpMap, float4(s.baseUv, 0.0h, bias));
#endif

    return UnpackScaleNormal(result, _BumpScale);  
}

half aSampleHeight(
    ASurface s)
{
    half result = 0.0h;

#ifdef _VIRTUALTEXTURING_ON
    result = VTSampleNormal(s.baseVirtualCoord).b;
#else
    result = tex2D(_ParallaxMap, s.baseUv).y;
#endif

    return result;
}

half3 aVertexColorTint(
    ASurface s,
    half strength)
{
#ifdef A_VERTEX_COLOR_IS_DATA
    return A_WHITE;
#else
    return aLerpWhiteTo(s.vertexColor.rgb, strength);
#endif
}

half3 aBaseVertexColorTint(
    ASurface s)
{
    return aVertexColorTint(s, _BaseColorVertexTint);
}

half4 aBaseTint(
    ASurface s)
{
    half4 result = _Color;

#ifndef A_VERTEX_COLOR_IS_DATA
    result.rgb *= aBaseVertexColorTint(s);
#endif

    return result;
}

half4 aBaseColor(
    ASurface s)
{
    return aBaseTint(s) * aSampleBase(s);
}

half aMetallic(
    half4 material)
{
    return _Metal * material.A_METALLIC_CHANNEL;
}

half aAmbientOcclusion(
    half4 material)
{
    return aLerpOneTo(material.A_AO_CHANNEL, _Occlusion);
}

half aSpecularity(
    half4 material)
{
    return _Specularity * material.A_SPECULARITY_CHANNEL;
}

half aRoughness(
    half4 material)
{
    return _Roughness * material.A_ROUGHNESS_CHANNEL;
}

void aParallaxOffset(
    inout ASurface s,
    float2 offset)
{
    offset *= s.mask;
    
    // To apply the parallax offset to secondary textures without causing swimming,
    // we must normalize it by removing the implicitly multiplied base map tiling. 
    s.uv01 += (offset / s.baseTiling).xyxy;
    aSetBaseUv(s, s.baseUv + offset);
}

void aOffsetBumpMapping(
    inout ASurface s)
{
    // NOTE: Prevents NaN compiler errors in DX9 mode for shadow pass.
#if defined(A_TANGENT_TO_WORLD_ON) && defined(A_VIEW_VECTOR_TANGENT_ON)
    half h = aSampleHeight(s) * _Parallax - _Parallax * 0.5h;
    half3 v = s.viewDirTangent;

    v.z += 0.42h;
    aParallaxOffset(s, h * (v.xy / v.z));
#endif
}

void aParallaxOcclusionMapping(
    inout ASurface s,
    float minSamples,
    float maxSamples)
{
    // NOTE: Prevents NaN compiler errors in DX9 mode for shadow pass.
#if defined(A_TANGENT_TO_WORLD_ON) && defined(A_VIEW_VECTOR_TANGENT_ON)
    // Parallax Occlusion Mapping
    // Subject to GameDev.net Open License
    // cf http://www.gamedev.net/page/resources/_/technical/graphics-programming-and-theory/a-closer-look-at-parallax-occlusion-mapping-r3262
    float2 offset = float2(0.0f, 0.0f);

    // Calculate the parallax offset vector max length.
    // This is equivalent to the tangent of the angle between the
    // viewer position and the fragment location.
    float parallaxLimit = -length(s.viewDirTangent.xy) / s.viewDirTangent.z;

    // Scale the parallax limit according to heightmap scale.
    parallaxLimit *= _Parallax;

    // Calculate the parallax offset vector direction and maximum offset.
    float2 offsetDirTangent = normalize(s.viewDirTangent.xy);
    float2 maxOffset = offsetDirTangent * parallaxLimit;
    
    // Calculate how many samples should be taken along the view ray
    // to find the surface intersection.  This is based on the angle
    // between the surface normal and the view vector.
    int numSamples = (int)lerp(maxSamples, minSamples, s.NdotV);
    int currentSample = 0;
    
    // Specify the view ray step size.  Each sample will shift the current
    // view ray by this amount.
    float stepSize = 1.0f / (float)numSamples;

    // Initialize the starting view ray height and the texture offsets.
    float currentRayHeight = 1.0f;	
    float2 lastOffset = float2(0.0f, 0.0f);
    
    float lastSampledHeight = 1.0f;
    float currentSampledHeight = 1.0f;

    #ifdef _VIRTUALTEXTURING_ON
        VirtualCoord vcoord = s.baseVirtualCoord;
    #else
        // Calculate the texture coordinate partial derivatives in screen
        // space for the tex2Dgrad texture sampling instruction.
        float2 dx = ddx(s.baseUv);
        float2 dy = ddy(s.baseUv);
    #endif

    while (currentSample < numSamples) {
        #ifdef _VIRTUALTEXTURING_ON
            vcoord = VTUpdateVirtualCoord(vcoord, s.baseUv + offset);
            currentSampledHeight = VTSampleNormal(vcoord).b;
        #else
            // Sample the heightmap at the current texcoord offset.
            currentSampledHeight = tex2Dgrad(_ParallaxMap, s.baseUv + offset, dx, dy).y;
        #endif

        // Test if the view ray has intersected the surface.
        UNITY_BRANCH
        if (currentSampledHeight > currentRayHeight) {
            // Find the relative height delta before and after the intersection.
            // This provides a measure of how close the intersection is to 
            // the final sample location.
            float delta1 = currentSampledHeight - currentRayHeight;
            float delta2 = (currentRayHeight + stepSize) - lastSampledHeight;
            float ratio = delta1 / (delta1 + delta2);

            // Interpolate between the final two segments to 
            // find the true intersection point offset.
            offset = lerp(offset, lastOffset, ratio);
            
            // Force the exit of the while loop
            currentSample = numSamples + 1;	
        }
        else {
            // The intersection was not found.  Now set up the loop for the next
            // iteration by incrementing the sample count,
            currentSample++;

            // take the next view ray height step,
            currentRayHeight -= stepSize;
            
            // save the current texture coordinate offset and increment
            // to the next sample location, 
            lastOffset = offset;
            offset += stepSize * maxOffset;

            // and finally save the current heightmap height.
            lastSampledHeight = currentSampledHeight;
        }
    }

    aParallaxOffset(s, offset);
#endif
}

#endif // A_FRAMEWORK_FEATURE_IMPL_CGINC
