Shader "Unlit/WebcamDisplay"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			

			fixed3 YUVtoRGB(fixed3 c)
			{
				fixed3 rgb;
				rgb.r = c.x + c.z * 1.13983;
				rgb.g = c.x + dot(fixed2(-0.39465, -0.58060), c.yz);
				rgb.b = c.x + c.y * 2.03211;
				return rgb;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 yuv = tex2D(_MainTex, i.uv);
				fixed4 rgb = fixed4(YUVtoRGB(yuv.rgb), yuv.a);
				return yuv;
			}

			//fixed4 frag (v2f i) : SV_Target
			//{
			//	// sample the texture
			//	fixed4 col = tex2D(_MainTex, i.uv);
			//	fixed Y = col.x;
			//	fixed U = col.y;
			//	fixed V = col.z;
			//	col.x = Y + 1.402 * (V - 0.5);
			//	col.y = Y - 0.34414 * (V - 0.5) - 0.71414 * (U - 0.5);
			//	col.z = Y + 1.772 * (U - 0.5);
			//	return col;
			//}
			ENDCG
		}
	}
}
