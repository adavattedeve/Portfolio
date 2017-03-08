Shader "MyCustom/Splatter"
{
	Properties
	{
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_MainTex("Main texture", 2D) = "white" {}
		_NoiseTex("Noise texture", 2D) = "white" {}
		_Warp("Warp", Range(0,1)) = 0
		_WarpIntensity("Warp intensity", Range(1, 20)) = 2
	}
		SubShader
	{
		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
	}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float4 color : COLOR;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float4 color : COLOR;
		float2 uv : TEXCOORD0;
		float2 world : TEXCOORD1;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.color = v.color;
		o.uv = v.uv;
		o.world = mul(unity_ObjectToWorld, v.vertex).xz;
		return o;
	}

	sampler2D _MainTex;
	sampler2D _NoiseTex;
	float _Warp;
	float4 _Color;
	uniform float _WarpIntensity;

	fixed4 frag(v2f i) : SV_Target
	{
		fixed2 warp = (((tex2D(_NoiseTex, i.world).rg) * _WarpIntensity) - 1) * _Warp;
	fixed4 col = tex2D(_MainTex, i.uv + warp);
	col *= i.color * _Color;
	return col;
	}
		ENDCG
	}
	}
}