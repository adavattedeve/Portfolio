// Alloy Physical Shader Framework
// Copyright 2013-2016 RUST LLC.
// http://www.alloy.rustltd.com/

Shader "Hidden/Alloy/Transmission Blit" {
Properties {
    _MainTex ("Render Input", 2D) = "white" {}
}
SubShader {
    ZTest Always 
    Cull Off 
    ZWrite Off 
    Fog { Mode Off }

    Pass {
        CGPROGRAM
        #pragma target 3.0
        #pragma exclude_renderers gles
        #pragma vertex vert_img
        #pragma fragment frag

        #include "UnityCG.cginc"
            
        sampler2D _MainTex;
            
        float4 frag(v2f_img IN) : COLOR {
            return tex2Dlod(_MainTex, float4(IN.uv, 0.0f, 0.0f));
        }
        ENDCG
    }
}
}