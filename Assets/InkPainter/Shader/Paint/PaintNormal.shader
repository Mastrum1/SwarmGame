// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Es/InkPainter/PaintNormal"{
	Properties{
		[HideInInspector]
		_MainTex("MainTex", 2D) = "white"
		[HideInInspector]
		_Brush("Brush", 2D) = "white"
		[HideInInspector]
		_BrushNormal("BrushNormal", 2D) = "white"
		[HideInInspector]
		_BrushScale("BrushScale", FLOAT) = 0.1
		[HideInInspector]
		_BrushRotate("Rotate", FLOAT) = 0
		[HideInInspector]
		_PaintUV("Hit UV Position", VECTOR) = (0,0,0,0)
		[HideInInspector]
		_NormalBlend("NormalBlend", FLOAT) = 1
		[HideInInspector]
		[KeywordEnum(USE_BRUSH, ADD, SUB MIN, MAX)]
		INK_PAINTER_NORMAL_BLEND("Normal Blend Keyword", FLOAT) = 0
		[KeywordEnum(USE, UNUSE)]
		DXT5NM_COMPRESS("use DXT5nm compressed", FLOAT) = 0
	}

	SubShader{
		
		Tags {"RenderPipeline" = "UniversalPipeline" }



		Pass{
			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "../Lib/InkPainterFoundation.cginc"

			struct Attributes {
				half4 vertex:POSITION;
				half4 uv:TEXCOORD0;
			};

			struct Varyings {
				half4 screen:SV_POSITION;
				half4 uv:TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _Brush;
			sampler2D _BrushNormal;
			CBUFFER_START(UnityPerMaterial)
			half4 _PaintUV;
			half _BrushScale;
			half _BrushRotate;
			half _NormalBlend;
			CBUFFER_END	
		

#pragma multi_compile INK_PAINTER_NORMAL_BLEND_USE_BRUSH INK_PAINTER_NORMAL_BLEND_ADD INK_PAINTER_NORMAL_BLEND_SUB INK_PAINTER_NORMAL_BLEND_MIN INK_PAINTER_NORMAL_BLEND_MAX
#pragma multi_compile DXT5NM_COMPRESS_USE DXT5NM_COMPRESS_UNUSE
#pragma vertex vert
#pragma fragment frag

			Varyings vert(Attributes i) {
				Varyings o;
				o.screen = TransformObjectToHClip(i.vertex);
				o.uv = i.uv;
				return o;
			}

			float4 frag(Varyings i) : SV_TARGET {
				half h = _BrushScale;
				half4 base = SampleTexture(_MainTex, i.uv.xy);

				if (IsPaintRange(i.uv, _PaintUV, h, _BrushRotate)) {
					half2 uv = CalcBrushUV(i.uv, _PaintUV, h, _BrushRotate);
					half4 brushColor = SampleTexture(_Brush, uv.xy);

					if (brushColor.a > 0) {
						half2 normalUV = CalcBrushUV(i.uv, _PaintUV, h, _BrushRotate);
						half4 normal = SampleTexture(_BrushNormal, normalUV.xy);
						return INK_PAINTER_NORMAL_BLEND(base, normal, _NormalBlend, brushColor.a);
					}
				}

				return base;
			}

			ENDHLSL
		}
	}
}