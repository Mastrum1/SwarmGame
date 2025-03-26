Shader "Es/Effective/HeightDrip"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ScaleFactor ("ScaleFactor", FLOAT) = 1
		_Viscosity("Viscosity", FLOAT) = 0.1
		_FlowDirection("Flow Direction", VECTOR) = (0, 0, 0, 0)
		_NormalMap("Normal Map", 2D) = "white" {}
		_HorizontalSpread("HorizontalSpread", Float) = 0.1
		_FixedColor("InkColor", COLOR) = (0, 0, 0, 0)
		[KeywordEnum(ADD, OVERWRITE)]
		COLOR_SYNTHESIS("Color synthesis algorithm", FLOAT) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile COLOR_SYNTHESIS_ADD COLOR_SYNTHESIS_OVERWRITE
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
			sampler2D _NormalMap;
			CBUFFER_START(UnityPerMaterial)
			half4 _MainTex_ST;
			half4 _MainTex_TexelSize;
			half _ScaleFactor;
			half _Viscosity;
			half4 _FlowDirection;
			half _HorizontalSpread;
			half4 _FixedColor;
			CBUFFER_END	

			half rand(half3 seed)
			{
				return frac(sin(dot(seed.xyz, half3(12.9898, 78.233, 56.787))) * 43758.5453);
			}

			Varyings vert (Attributes v)
			{
				Varyings o;
				o.vertex = TransformObjectToHClip(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			half4 frag (Varyings i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv);
				
				half3 normal = normalize(UnpackNormal(tex2D(_NormalMap, i.uv)).xyz);
				half VITIATE_Z = pow(normal.b, 2) - normal.y * 0.2;
				half VITIATE_X = _HorizontalSpread * rand(half3(i.uv.xy, i.uv.x + i.uv.y)) * (1 + normal.b * 30);
				
				half2 shiftZ = half2(_FlowDirection.x * _MainTex_TexelSize.x, _FlowDirection.y * _MainTex_TexelSize.y) * _ScaleFactor * _Viscosity * VITIATE_Z;
				half2 shiftX = half2(_MainTex_TexelSize.x * _FlowDirection.y, _MainTex_TexelSize.y * _FlowDirection.x) * _ScaleFactor * _Viscosity * VITIATE_X;
				half2 shiftx = -shiftX;
				
				half4 texZ = tex2D(_MainTex, clamp(i.uv.xy + shiftZ, 0, 1));
				half4 texx = tex2D(_MainTex, clamp(i.uv.xy + shiftx + shiftZ, 0, 1));
				half4 texX = tex2D(_MainTex, clamp(i.uv.xy + shiftX + shiftZ, 0, 1));
				
				half amountUp = (texZ.a + texx.a + texX.a) * 0.3333;

				if (amountUp > (1 - _Viscosity)) {
					half resultAmount = (col.a + amountUp) * 0.5;
#ifdef COLOR_SYNTHESIS_ADD
					half3 maxRGB = max(col.rgb, max(texZ.rgb, max(texx.rgb, texX.rgb)));
					half3 resultRGB = lerp(maxRGB, texZ.rgb, clamp(amountUp - _Viscosity, 0, 1));
					return half4(resultRGB, resultAmount);
#else
					return half4(_FixedColor.rgb, resultAmount);
#endif

				}

				return col;
			}
			ENDHLSL
		}
	}
}
