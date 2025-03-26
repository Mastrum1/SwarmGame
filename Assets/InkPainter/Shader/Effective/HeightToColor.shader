Shader "Es/Effective/HeightToColor"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ColorMap("ColorMap", 2D) = "white" {}
		_BaseColor("BaseColor", 2D) = "white" {}
		_Alpha("Alpha", Float) = 1
		_Border("Border", Float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			struct Attributes
			{
				half4 vertex : POSITION;
				half2 uv : TEXCOORD0;
			};

			struct Varyings
			{
				half2 uv : TEXCOORD0;
				half4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _ColorMap;
			sampler2D _BaseColor;
			CBUFFER_START(UnityPerMaterial)
			half4 _MainTex_ST;
			half _Alpha;
			half _Border;
			CBUFFER_END	

			Varyings vert (Attributes v)
			{
				Varyings o;
				o.vertex = TransformObjectToHClip(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			half4 frag (Varyings i) : SV_Target
			{
				half4 mainCol = tex2D(_MainTex, i.uv);

				if (mainCol.a > _Border) {
					return lerp(tex2D(_BaseColor, i.uv), half4(mainCol.rgb, 1), _Alpha);
				}

				return tex2D(_ColorMap, i.uv);
			}
			ENDHLSL
		}
	}
}
