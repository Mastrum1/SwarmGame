// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Es/Effective/GrabArea"{
	Properties{
		[HideInInspector]
		_ClipTex("Clipping Texture", 2D) = "white"
		[HideInInspector]
		_TargetTex("Target Texture", 2D) = "white"
		[HideInInspector]
		_ClipScale("Clipping Scale", FLOAT) = 0.1
		[HideInInspector]
		_ClipUV("Target UV Position", VECTOR) = (0,0,0,0)
		[HideInInspector]
		_Rotate("Rotate", FLOAT) = 0
		[KeywordEnum(CLAMP, REPEAT, CLIP)]
		WRAP_MODE("Color Blend Keyword", FLOAT) = 0
		[KeywordEnum(REPLACE, NOT_REPLACE)]
		ALPHA("Clipping texture replaces alpha", FLOAT) = 0
	}

	SubShader{
		
		Tags {"RenderPipeline" = "UniversalPipeline" }		

		Pass{
			HLSLPROGRAM
#pragma multi_compile WRAP_MODE_CLAMP WRAP_MODE_REPEAT WRAP_MODE_CLIP
#pragma multi_compile ALPHA_REPLACE ALPHA_NOT_REPLACE
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

			sampler2D _TargetTex;
			sampler2D _ClipTex;

			CBUFFER_START(UnityPerMaterial)
			half4 _ClipUV;
			half _ClipScale;
			half _Rotate;
			CBUFFER_END	


			Varyings vert(Attributes i) {
				Varyings o;
				o.screen = TransformObjectToHClip(i.vertex);
				o.uv = i.uv;
				return o;
			}

			half4 frag(Varyings i) : SV_TARGET {
				half angle = _Rotate;
#if !UNITY_UV_STARTS_AT_TOP
				angle = 180 - angle;
#endif

				half2 uv = Rotate(i.uv.xy - 0.5, angle) + 0.5;
				half uv_x = (uv.x - 0.5) * _ClipScale * 2 + _ClipUV.x;
				half uv_y = (uv.y - 0.5) * _ClipScale * 2 + _ClipUV.y;

#if WRAP_MODE_CLAMP
				//Clamp UV
				uv_x = clamp(uv_x, 0, 1);
				uv_y = clamp(uv_y, 0, 1);
#elif WRAP_MODE_REPEAT
				//Repeat UV
				uv_x = fmod(abs(uv_x), 1);
				uv_y = fmod(abs(uv_y), 1);
#elif WRAP_MODE_CLIP
				//Clip UV
				clip(uv_x);
				clip(uv_y);
				clip(trunc(uv_x) * -1);
				clip(trunc(uv_y) * -1);
#endif

				half4 base = tex2D(_TargetTex, half2(uv_x, uv_y));
#if ALPHA_REPLACE
				half alpha = tex2D(_ClipTex, uv.xy).a;
				base.a = alpha;
#endif
				return base;
			}

			ENDHLSL
		}
	}
}
