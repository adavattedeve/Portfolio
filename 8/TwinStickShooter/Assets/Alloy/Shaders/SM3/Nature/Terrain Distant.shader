// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

Shader "Hidden/Alloy/Nature/Terrain/Distant" {
Properties {
    // Distant Terrain
    _DistantSpecularity ("'Specularity' {Min:0, Max:1}", Float) = 0.5
    _DistantSpecularTint ("'Specular Tint' {Min:0, Max:1}", Float) = 0
    _DistantRoughness ("'Roughness' {Min:0, Max:1}", Float) = 0.5
    
    // Detail
    [Toggle(_DETAIL_MULX2)] 
    _DetailT ("'Detail' {Feature:{Color:7}}", Float) = 0
    [Enum(Mul, 0, MulX2, 1)] 
    _DetailMode ("'Color Mode' {Dropdown:{Mul:{}, MulX2:{}}}", Float) = 0
    _DetailAlbedoMap ("'Color(RGB)' {Visualize:{RGB}}", 2D) = "white" {}
    _DetailNormalMap ("'Normals' {Visualize:{NRM}, Parent:_DetailAlbedoMap}", 2D) = "bump" {}
    _DetailWeight ("'Weight' {Min:0, Max:1}", Float) = 1
    _DetailNormalMapScale ("'Normal Strength' {}", Float) = 1
    
    // set by terrain engine
    _MainTex ("Base (RGB) Smoothness (A)", 2D) = "white" {}
    _MetallicTex ("Metallic (R)", 2D) = "white" {}
    _Smoothness ("Smoothness", Range(0.0, 1.0)) = 0.0
}

CGINCLUDE
    #define A_TERRAIN_DISTANT
ENDCG

SubShader {
    Tags {
        "RenderType" = "Opaque"
        "Queue" = "Geometry-100"
    }
    LOD 200

    Pass {
        Name "FORWARD" 
        Tags { "LightMode" = "ForwardBase" }

        CGPROGRAM
        #pragma target 3.0
        #pragma exclude_renderers gles
        
        #pragma shader_feature _DETAIL_MULX2
        
        //#pragma multi_compile __ VTRANSPARENCY_ON
        #pragma multi_compile_fwdbase
        #pragma multi_compile_fog
            
        #pragma vertex aVertexShader
        #pragma fragment aFragmentShader
        
        #define UNITY_PASS_FORWARDBASE
        
        #include "Assets/Alloy/Shaders/Definitions/Terrain.cginc"
        #include "Assets/Alloy/Shaders/Passes/ForwardBase.cginc"

        ENDCG
    }
    
    Pass {
        Name "FORWARD_DELTA"
        Tags { "LightMode" = "ForwardAdd" }
        
        Blend One One
        ZWrite Off

        CGPROGRAM
        #pragma target 3.0
        #pragma exclude_renderers gles
        
        #pragma shader_feature _DETAIL_MULX2
        
        //#pragma multi_compile __ VTRANSPARENCY_ON
        #pragma multi_compile_fwdadd_fullshadows
        #pragma multi_compile_fog
        
        #pragma vertex aVertexShader
        #pragma fragment aFragmentShader

        #define UNITY_PASS_FORWARDADD

        #include "Assets/Alloy/Shaders/Definitions/Terrain.cginc"
        #include "Assets/Alloy/Shaders/Passes/ForwardAdd.cginc"

        ENDCG
    }
    
    Pass {
        Name "SHADOWCASTER"
        Tags { "LightMode" = "ShadowCaster" }
        
        CGPROGRAM
        #pragma target 3.0
        #pragma exclude_renderers gles
        
        #pragma multi_compile_shadowcaster

        #pragma vertex aVertexShader
        #pragma fragment aFragmentShader
        
        #define UNITY_PASS_SHADOWCASTER
        
        #include "Assets/Alloy/Shaders/Definitions/Terrain.cginc"
        #include "Assets/Alloy/Shaders/Passes/Shadow.cginc"

        ENDCG
    }
    
    Pass {
        Name "DEFERRED"
        Tags { "LightMode" = "Deferred" }

        CGPROGRAM
        #pragma target 3.0
        #pragma exclude_renderers nomrt gles
        
        #pragma shader_feature _DETAIL_MULX2
        
        #pragma multi_compile_prepassfinal
        #pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
        
        #pragma vertex aVertexShader
        #pragma fragment aFragmentShader
        
        #define UNITY_PASS_DEFERRED
        
        #include "Assets/Alloy/Shaders/Definitions/Terrain.cginc"
        #include "Assets/Alloy/Shaders/Passes/Deferred.cginc"

        ENDCG
    }
    
    Pass {
        Name "Meta"
        Tags { "LightMode" = "Meta" }

        Cull Off

        CGPROGRAM
        #pragma target 3.0
        #pragma exclude_renderers nomrt gles
        
        #pragma shader_feature _DETAIL_MULX2
        
        #pragma vertex aVertexShader
        #pragma fragment aFragmentShader
        
        #define UNITY_PASS_META
        
        #include "Assets/Alloy/Shaders/Definitions/Terrain.cginc"
        #include "Assets/Alloy/Shaders/Passes/Meta.cginc"

        ENDCG
    }
}

FallBack "Diffuse"
}
