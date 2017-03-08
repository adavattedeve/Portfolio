// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

/////////////////////////////////////////////////////////////////////////////////
/// @file Config.cginc
/// @brief User configuration options.
/////////////////////////////////////////////////////////////////////////////////

#ifndef A_CONFIG_CGINC
#define A_CONFIG_CGINC

#include "Assets/Alloy/Shaders/Framework/Keywords.cginc"

/// Flag provided for third-party integration.
#define A_VERSION 3.44

/// Enables clamping of all shader outputs to prevent blending and bloom errors.
#define A_USE_HDR_CLAMP 1

/// Max HDR intensity for lighting and emission.
#define A_HDR_CLAMP_MAX_INTENSITY 100.0

/// Enables capping tessellation quality via the global _MinEdgeLength property.
#define A_USE_TESSELLATION_MIN_EDGE_LENGTH 0

/// Enables tube area lights. Can be disabled to improve sphere light performance.
#define A_USE_TUBE_LIGHTS 1

/// Enables the Unity behavior for light cookies.
#define A_USE_UNITY_LIGHT_COOKIES 0

/// Enables the Unity behavior for attenuation.
#define A_USE_UNITY_ATTENUATION 0

/// Packed map metallic channel.
#define A_METALLIC_CHANNEL r

/// Packed map ao channel.
#define A_AO_CHANNEL g

/// Packed map spcularity channel.
#define A_SPECULARITY_CHANNEL b

/// Packed map roughness channel.
#define A_ROUGHNESS_CHANNEL a


// THIRD PARTY
//#include "Assets/Plugins/HxVolumetricLighting/BuiltIn-Replacement/HxVolumetricCore.cginc"

#endif // A_CONFIG_CGINC
