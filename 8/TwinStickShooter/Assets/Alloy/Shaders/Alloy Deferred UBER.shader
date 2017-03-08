// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

Shader "Hidden/Alloy/Deferred Shading UBER" {
Properties {
    _LightTexture0 ("", any) = "" {}
    _LightTextureB0 ("", 2D) = "" {}
    _ShadowMapTexture ("", any) = "" {}
    _SrcBlend ("", Float) = 1
    _DstBlend ("", Float) = 1
}

CGINCLUDE
    // UBER - Standard Shader Ultra integration
    // https://www.assetstore.unity3d.com/en/#!/content/39959

    // When using both features check UBER_StandardConfig.cginc to configure Gbuffer channels
    #define UBER_TRANSLUCENCY_DEFERRED
    #define UBER_POM_SELF_SHADOWS_DEFERRED
    //
    // you can gently turn it up (like 0.3, 0.5) if you find front facing geometry overbrighten (esp. for point lights),
    // but suppresion can negate albedo for high translucency values (they can become badly black)
    #define TRANSLUCENCY_SUPPRESS_DIFFUSECOLOR 0.0
ENDCG

SubShader {

// Pass 1: Lighting pass
//  LDR case - Lighting encoded into a subtractive ARGB8 buffer
//  HDR case - Lighting additively blended into floating point buffer
Pass {
    ZWrite Off
    Blend [_SrcBlend] [_DstBlend]

CGPROGRAM
#pragma target 3.0

#pragma vertex vert_deferred
#pragma fragment frag
#pragma multi_compile_lightpass
#pragma multi_compile ___ UNITY_HDR_ON

#pragma exclude_renderers nomrt

#include "Assets/Alloy/Shaders/Lighting/Standard.cginc"
#include "Assets/Alloy/Shaders/Framework/Deferred.cginc"

// UBER - POM self-shadowing (for one realtime light)
#if defined(UBER_POM_SELF_SHADOWS_DEFERRED)
	float4 _WorldSpaceLightPosCustom;
#endif

// UBER - Translucency, POM self-shadowing and other values encoded
#if defined(UBER_POM_SELF_SHADOWS_DEFERRED) || defined(UBER_TRANSLUCENCY_DEFERRED)
	sampler2D _UBERPropsBuffer;
#endif

// UBER - Translucency
#if defined(UBER_TRANSLUCENCY_DEFERRED)
	sampler2D _UBERTranslucencySetup;
	struct TranslucencyParams {
		half3 _TranslucencyColor;
		half _TranslucencyStrength;
		half _TranslucencyConstant;
		half _TranslucencyNormalOffset;
		half _TranslucencyExponent;
		half _TranslucencyPointLightDirectionality;
		half _TranslucencySuppressRealtimeShadows;
		half _TranslucencyNDotL;
	};

    half Translucency(half3 normalWorld, ADirect d, half3 eyeVec, TranslucencyParams translucencyParams) {
        #ifdef USING_DIRECTIONAL_LIGHT
            half tLitDot = saturate(dot((d.direction + normalWorld * translucencyParams._TranslucencyNormalOffset), eyeVec));
        #else
            float3 lightDirectional = normalize(_LightPos.xyz - _WorldSpaceCameraPos.xyz);
            half3 light_dir = normalize(lerp(d.direction, lightDirectional, translucencyParams._TranslucencyPointLightDirectionality));
            half tLitDot = saturate(dot((light_dir + normalWorld * translucencyParams._TranslucencyNormalOffset), eyeVec));
        #endif
        
        tLitDot = exp2(-translucencyParams._TranslucencyExponent * (1 - tLitDot)) * translucencyParams._TranslucencyStrength;
		float NDotL = abs(dot(d.direction, normalWorld));
		tLitDot *= lerp(1, NDotL, translucencyParams._TranslucencyNDotL);

		half translucencyAtten = (tLitDot + translucencyParams._TranslucencyConstant*(NDotL + 0.1));


        return translucencyAtten;
    }
#endif
        
half4 CalculateLight (unity_v2f_deferred i)
{
    ASurface s = aDeferredSurface(i);
    ADirect d = aDeferredDirect(s);
    
    half4 color = 0.0h;
    
#if defined(UBER_POM_SELF_SHADOWS_DEFERRED) || defined(UBER_TRANSLUCENCY_DEFERRED)
	// buffer decoded from _CameraGBufferTexture3.a in command buffer
	half Wetness = 0;
	half SS = 1;
	half translucencySetupIndex = 0;
	half translucency_thickness = 0;
	float encoded = tex2D(_UBERPropsBuffer, s.screenUv).r;
	if (encoded < 0) {
		encoded = -encoded;

		// wetness (not used currently so below line should get compiled out)
		encoded /= 8.0; // 3 bits
		Wetness = frac(encoded) * (8.0 / 7.0); // to 0..1 range
		encoded = floor(encoded);

		// self shadowing
		encoded /= 4.0; // 2 bits
		SS = 1 - frac(encoded) * (4.0 / 3.0); // to 0..1 range
		encoded = floor(encoded);

		// translucency color index
		encoded /= 4.0; // 2 bits
		translucencySetupIndex = frac(encoded); // directly decoded as U coord in lookup texture
		encoded = floor(encoded);

		// translucency thickness
		encoded /= 15.0; // 4 bits (divide by 15 instead of 16 to bring it immediately to 0..1 range)
		translucency_thickness = encoded;
	} // else - no prop used for this pixel (no translucency, self-shadowing and surface is considered to be dry)
#endif

#if defined(UBER_POM_SELF_SHADOWS_DEFERRED)
    d.shadow = (abs(dot((_LightDir.xyz + _WorldSpaceLightPosCustom.xyz), float3(1, 1, 1))) < 0.01) ? min(d.shadow, SS) : d.shadow;
#endif	
    
#if defined(UBER_TRANSLUCENCY_DEFERRED)	
	half setupIndex = translucencySetupIndex; // [0..1] to [0..1) range

	half4 val;
	val = tex2D(_UBERTranslucencySetup, float2(setupIndex, 0));
	TranslucencyParams translucencyParams;
	translucencyParams._TranslucencyColor = val.rgb;
	translucencyParams._TranslucencyStrength = val.a;
	val = tex2D(_UBERTranslucencySetup, float2(setupIndex, 0.4));
	translucencyParams._TranslucencyPointLightDirectionality = val.r;
	translucencyParams._TranslucencyConstant = val.g;
	translucencyParams._TranslucencyNormalOffset = val.b;
	translucencyParams._TranslucencyExponent = val.a;
	val = tex2D(_UBERTranslucencySetup, float2(setupIndex, 0.8));
	translucencyParams._TranslucencySuppressRealtimeShadows = val.r;
	translucencyParams._TranslucencyNDotL = val.g;
    
    half3 TL = Translucency(s.normalWorld, d, normalize(s.positionWorld - _WorldSpaceCameraPos), translucencyParams)*translucencyParams._TranslucencyColor;
    
    TL *= s.albedo;
	TL *= translucency_thickness;
    s.albedo *= saturate(1 - max(max(TL.r, TL.g), TL.b) * TRANSLUCENCY_SUPPRESS_DIFFUSECOLOR);
    d.shadow = lerp(d.shadow, 1, saturate(dot(TL, 1) * translucencyParams._TranslucencySuppressRealtimeShadows));

    color.rgb += d.shadow * TL * d.color.rgb;
#endif
    
    color.rgb += aDirect(d, s);
    return aHdrClamp(color);
}

#ifdef UNITY_HDR_ON
half4
#else
fixed4
#endif
frag (unity_v2f_deferred i) : SV_Target
{
    half4 c = CalculateLight(i);
    #ifdef UNITY_HDR_ON
    return c;
    #else
    return exp2(-c);
    #endif
}

ENDCG
}


// Pass 2: Final decode pass.
// Used only with HDR off, to decode the logarithmic buffer into the main RT
Pass {
    ZTest Always Cull Off ZWrite Off
    Stencil {
        ref [_StencilNonBackground]
        readmask [_StencilNonBackground]
        // Normally just comp would be sufficient, but there's a bug and only front face stencil state is set (case 583207)
        compback equal
        compfront equal
    }

CGPROGRAM
#pragma target 3.0
#pragma vertex vert
#pragma fragment frag
#pragma exclude_renderers nomrt

#include "UnityCG.cginc"

sampler2D _LightBuffer;
struct v2f {
    float4 vertex : SV_POSITION;
    float2 texcoord : TEXCOORD0;
};

v2f vert (float4 vertex : POSITION, float2 texcoord : TEXCOORD0)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(vertex);
    o.texcoord = texcoord.xy;
#ifdef UNITY_SINGLE_PASS_STEREO
    o.texcoord = TransformStereoScreenSpaceTex(o.texcoord, 1.0f);
#endif
    return o;
}

fixed4 frag (v2f i) : SV_Target
{
    return -log2(tex2D(_LightBuffer, i.texcoord));
}
ENDCG 
}

}
Fallback Off
}
