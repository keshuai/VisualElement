// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UIViewClip"
{
	Properties
	{
		_MainTex0 ("Texture 0", 2D) = "white" {}
		_MainTex1 ("Texture 1", 2D) = "white" {}
		_MainTex2 ("Texture 2", 2D) = "white" {}
		_MainTex3 ("Texture 3", 2D) = "white" {}
	}
	
	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			// 深度偏移
			Offset -1, -1
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex0;
			sampler2D _MainTex1;
			sampler2D _MainTex2;
			sampler2D _MainTex3;

			uniform float4 _Clip;
	
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				float2 worldPos : TEXCOORD1;
			};
	
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				o.worldPos = v.vertex.xy;
				return o;
			}
				
			fixed4 frag (v2f IN) : SV_Target
			{
				float2 worldPos = IN.worldPos;

				if (worldPos.x < _Clip.x || worldPos.x > _Clip.y|| worldPos.y < _Clip.z || worldPos.y > _Clip.w)
				{
					discard;
				}

				float2 uv = IN.texcoord;

				// only color
				if (uv.x < -1)
				{
					return IN.color;
				}

				// texcoord.x 表示纹理索引 {0, 1, 2, 3...}
				// texcoord.y 表示元素类型 {label, image, ...}

				if (uv.x <= 1)
				{
					if (uv.y < 2)
					{
						return tex2D(_MainTex0, uv) * IN.color;
					}
					else
					{
					    if (uv.x < 0) return fixed4(0, 0, 0, 0);
						uv.y -= 2;
						return tex2D(_MainTex0, uv).a * IN.color;
					}
				}

				uv.x -= 2;
				if (uv.x <= 1)
				{
					if (uv.y < 2)
					{
						return tex2D(_MainTex1, uv) * IN.color;
					}
					else
					{
					    if (uv.x < 0) return fixed4(0, 0, 0, 0);
						uv.y -= 2;
						return tex2D(_MainTex1, uv).a * IN.color;
					}
				}

				uv.x -= 2;
				if (uv.x <= 1)
				{
					if (uv.y < 2)
					{
						return tex2D(_MainTex2, uv) * IN.color;
					}
					else
					{
					    if (uv.x < 0) return fixed4(0, 0, 0, 0);
						uv.y -= 2;
						return tex2D(_MainTex2, uv).a * IN.color;
					}
				}
			
				uv.x -= 2;
				if (uv.y < 2)
				{
					return tex2D(_MainTex3, uv) * IN.color;
				}
				else
				{
				    if (uv.x < 0) return fixed4(0, 0, 0, 0);
					uv.y -= 2;
					return tex2D(_MainTex3, uv).a * IN.color;
				}
			}
			ENDCG
		}
	}

	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex0]
			{
				Combine Texture * Primary
			}
		}
	}
}
