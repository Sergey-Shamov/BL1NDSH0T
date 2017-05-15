Shader "Custom/PaintShaders/WhiteSpaceShader" {
	Properties{
		[HideInInspector]_DrawingTex("Drawing texture", 2D) = "" {}

		//_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_NormalTex("Normal Map", 2D) = "bump" {}
		//_Glossiness("Smoothness", Range(0,1)) = 0.5
		//_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

	sampler2D _DrawingTex;
	sampler2D _MainTex;
	sampler2D _NormalTex;

	struct Input {
		float2 uv_MainTex;
		float2 uv2_DrawingTex;
	};

	//half _Glossiness;
	//half _Metallic;
	//fixed4 _Color;

	void surf(Input IN, inout SurfaceOutputStandard o) {
		float4 drawData = tex2D(_DrawingTex, IN.uv2_DrawingTex);
		float4 mainData = tex2D(_MainTex, IN.uv_MainTex);// *_Color;
		fixed4 c = lerp(mainData, drawData, drawData.a);
		c.a = drawData.a + mainData.a;
		o.Albedo = c.rgb;

		fixed3 normalMap = UnpackNormal(tex2D(_NormalTex, IN.uv_MainTex));
		normalMap.x *= drawData.a; // Normal is amplified under the paint
		normalMap.y *= drawData.a;
		o.Normal = normalize(normalMap);	

		o.Metallic = 0;// _Metallic;
		o.Smoothness = 0;//_Glossiness;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
