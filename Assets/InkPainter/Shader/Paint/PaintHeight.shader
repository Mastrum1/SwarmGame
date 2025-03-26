// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Es/InkPainter/PaintHeight"{
	Properties{
		[HideInInspector]
		_MainTex("MainTex", 2D) = "white"
		[HideInInspector]
		_Brush("Brush", 2D) = "white"
		[HideInInspector]
		_BrushHeight("BrushHeight", 2D) = "white"
		[HideInInspector]
		_BrushScale("BrushScale", FLOAT) = 0.1
		[HideInInspector]
		_BrushRotate("Rotate", FLOAT) = 0
		[HideInInspector]
		_PaintUV("Hit UV Position", VECTOR) = (0,0,0,0)
		[HideInInspector]
		_HeightBlend("HeightBlend", FLOAT) = 1
		[HideInInspector]
		_Color("Color", VECTOR) = (0,0,0,0)
		[HideInInspector]
		[KeywordEnum(USE_BRUSH, ADD, SUB, MIN, MAX, COLOR_RGB_HEIGHT_A)]
		INK_PAINTER_HEIGHT_BLEND("Height Blend Keyword", FLOAT) = 0
	}

	SubShader{
		
		Tags {"RenderPipeline" = "UniversalPipeline" }


			

		

		Pass{
			HLSLPROGRAM
#pragma multi_compile INK_PAINTER_HEIGHT_BLEND_USE_BRUSH INK_PAINTER_HEIGHT_BLEND_ADD INK_PAINTER_HEIGHT_BLEND_SUB INK_PAINTER_HEIGHT_BLEND_MIN INK_PAINTER_HEIGHT_BLEND_MAX INK_PAINTER_HEIGHT_BLEND_COLOR_RGB_HEIGHT_A
#pragma vertex vert
#pragma fragment frag

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "../Lib/InkPainterFoundation.cginc"

			struct Attributes {
				float4 vertex:POSITION;
				float4 uv:TEXCOORD0;
			};

			struct Varyings {
				float4 screen:SV_POSITION;
				float4 uv:TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _Brush;
			sampler2D _BrushHeight;
			CBUFFER_START(UnityPerMaterial)
			float4 _PaintUV;
			float _BrushScale;
			float _BrushRotate;
			float _HeightBlend;
			float4 _Color;
			CBUFFER_END	

			Varyings vert(Attributes i) {
				Varyings o;
				o.screen = TransformObjectToHClip(i.vertex.xyz);
				o.uv = i.uv;
				return o;
			}

			half4 frag(Varyings i) : SV_TARGET {
				half h = _BrushScale;
				half4 base = SampleTexture(_MainTex, i.uv.xy);

				if (IsPaintRange(i.uv.xy, _PaintUV.xy, h, _BrushRotate)) {
					half2 uv = CalcBrushUV(i.uv.xy, _PaintUV.xy, h, _BrushRotate);
					float4 brushColor = SampleTexture(_Brush, uv.xy);

					if (brushColor.a > 0) {
						half2 heightUV = CalcBrushUV(i.uv.xy, _PaintUV.xy, h, _BrushRotate);
						half4 height = SampleTexture(_BrushHeight, heightUV.xy);
#if INK_PAINTER_HEIGHT_BLEND_COLOR_RGB_HEIGHT_A
						height.a = 0.299 * height.r + 0.587 * height.g + 0.114 * height.b;
						height.rgb = _Color.rgb;
						brushColor.a = _Color.a;
#endif
						return INK_PAINTER_HEIGHT_BLEND(base, height, _HeightBlend, brushColor);
					}
				}

				return base;
			}

			ENDHLSL
		}
	}
}