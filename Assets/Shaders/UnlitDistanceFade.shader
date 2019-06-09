Shader "Unlit/UnlitDistanceFade"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DepthLevel("Depth Level", Range(1, 3)) = 2
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform sampler2D_float _CameraDepthTexture;
			uniform fixed _DepthLevel;
			uniform half4 _MainTex_TexelSize;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 projPos : TEXCOORD1;
			};

			
			v2f vert (appdata v)
			{
				v2f o;
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = mul(UNITY_MATRIX_VP, float4(worldPos, 1.));
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.projPos = ComputeScreenPos(o.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				float depth = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r);

				// If depth < n, 100% brightness
				// If depth > p, 0% brightness
				// slerp between

				// TODO: Uniform these bad boys
				float falloffStart = 0.00;
				float falloffEnd = 0.08;

				float falloffDist = falloffEnd - falloffStart;
				float m = -1.0 / falloffDist;
				float c = 1.0 - (falloffStart * m);
				float brightness = m * depth + c;
				brightness = clamp(brightness, 0.0, 1.0);

				col.r *= brightness;
				col.g *= brightness;
				col.b *= brightness;
				return col;
			}
			ENDCG
		}
	}
}
