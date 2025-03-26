// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Es/InkPainter/PaintMain"{
	Properties{
		[HideInInspector]
		_MainTex("MainTex", 2D) = "white"
		[HideInInspector]
		_Brush("Brush", 2D) = "white"
		[HideInInspector]
		_BrushScale("BrushScale", FLOAT) = 0.1
		[HideInInspector]
		_BrushRotate("Rotate", FLOAT) = 0
		[HideInInspector]
		_ControlColor("ControlColor", VECTOR) = (0,0,0,0)
		[HideInInspector]
		_PaintUV("Hit UV Position", VECTOR) = (0,0,0,0)
		[HideInInspector]
		[KeywordEnum(USE_CONTROL, USE_BRUSH, NEUTRAL, ALPHA_ONLY)]
		INK_PAINTER_COLOR_BLEND("Color Blend Keyword", FLOAT) = 0
	}

	SubShader{

		Tags {"RenderPipeline" = "UniversalPipeline" }			

		Pass{
			HLSLPROGRAM


#pragma multi_compile INK_PAINTER_COLOR_BLEND_USE_CONTROL INK_PAINTER_COLOR_BLEND_USE_BRUSH INK_PAINTER_COLOR_BLEND_NEUTRAL INK_PAINTER_COLOR_BLEND_ALPHA_ONLY
#pragma vertex vert
#pragma fragment frag

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "../Lib/InkPainterFoundation.cginc"

			struct Attributes {
				half4 vertex:POSITION;
				half4 uv:TEXCOORD0;
			};

			struct Varyings {
				half4 uv:TEXCOORD0;
				half4 screen:SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _Brush;
			CBUFFER_START(UnityPerMaterial)
			half4 _PaintUV;
			half _BrushScale;
			half _BrushRotate;
			half4 _ControlColor;
			CBUFFER_END	

			Varyings vert(Attributes i) {
				Varyings o;
				o.screen = TransformObjectToHClip(i.vertex);
				o.uv = i.uv;
				return o;
			}

			float4 frag(Varyings i) : SV_TARGET {
				float h = _BrushScale;
				float4 base = SampleTexture(_MainTex, i.uv.xy);
				float4 brushColor = half4(1, 1, 1, 1);

				if (IsPaintRange(i.uv, _PaintUV, h, _BrushRotate)) {
					half2 uv = CalcBrushUV(i.uv, _PaintUV, h, _BrushRotate);
					brushColor = SampleTexture(_Brush, uv.xy);

					return INK_PAINTER_COLOR_BLEND(base, brushColor, _ControlColor);
				}
				return base;
			}

			ENDHLSL
		}
	}
}