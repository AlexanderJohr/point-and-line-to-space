// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "PaLtS/Waves"
{
	Properties{
				_Color("Color", Color) = (1,1,1,1)

   _Tint("Colour Tint", Color) = (1,1,1,1)
   _Freq("Frequency", Range(0,12.5663706144)) = 3
   _Speed("Speed",Range(0,100000)) = 10
   _Amp("Amplitude",Range(0,10)) = 0.5
		  _Test("Test",Range(-100,100)) = 0.5

	}
		SubShader{
			Cull Off
		 Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
	   Blend SrcAlpha OneMinusSrcAlpha

	   ZWrite Off
		
	   CGPROGRAM
		  #pragma surface surf Lambert vertex:vert  alpha:fade

		  struct Input {
			  float3 vertColor;
		  };

		  float4 _Tint;
		  float _Freq;
		  float _Speed;
		  float _Amp;
		  float _Test;
		  fixed4 _Color;

		  struct appdata {
			  float4 vertex: POSITION;
			  float3 normal: NORMAL;
			  float4 texcoord: TEXCOORD0;
			  float4 texcoord1: TEXCOORD1;
			  float4 texcoord2: TEXCOORD2;
		  };

		  void vert(inout appdata v, out Input o) {
			  UNITY_INITIALIZE_OUTPUT(Input,o);
			  float4 NewCoord = mul(unity_ObjectToWorld, v.vertex);

			  float t = _Time * _Speed;
			  float waveHeight = sin(t + v.vertex.z * _Freq) * _Amp +
							sin(t * 2 + v.vertex.z * _Freq * 2) * _Amp;
			  v.vertex.x = v.vertex.x + waveHeight *  ((_Test + v.vertex.z) * (_Test - v.vertex.z));
		  }

		  void surf(Input IN, inout SurfaceOutput o) {
			  fixed4 c =  _Color;
			  o.Albedo = c.rgb;
			  o.Alpha = c.a;
		  }
		  ENDCG

   }
	   Fallback "Diffuse"
}
