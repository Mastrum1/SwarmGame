Shader "Es/Effective/HeightToNormal"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_bumpMap("Default BumpMap", 2D) = "white" {}
		_NormalScaleFactor("NormalScale", FLOAT) = 1
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
			sampler2D _BumpMap;
			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _NormalScaleFactor;
			float _Border;
			CBUFFER_END	

			Varyings vert (Attributes v)
			{
				Varyings o;
				o.vertex = TransformObjectToHClip(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			half4 PackNormal(half3 normal) {
#if defined(UNITY_NO_DXT5nm)
				return half4(normal, 0);
#else
				return half4(normal.y, normal.y, normal.y, normal.x);
#endif
			}

			half4 frag (Varyings i) : SV_Target
			{
				half2 shiftX = { _MainTex_TexelSize.x, 0 };
				half2 shiftZ = { 0, _MainTex_TexelSize.y };
				
				half4 texX = 2 * tex2D(_MainTex, i.uv.xy + shiftX) - 1;
				half4 texx = 2 * tex2D(_MainTex, i.uv.xy - shiftX) - 1;
				half4 texZ = 2 * tex2D(_MainTex, i.uv.xy + shiftZ) - 1;
				half4 texz = 2 * tex2D(_MainTex, i.uv.xy - shiftZ) - 1;
				
				half3 du = { 1, 0, _NormalScaleFactor * (texX.a - texx.a) };
				half3 dv = { 0, 1, _NormalScaleFactor * (texZ.a - texz.a)};
				
				half3 normal = normalize(cross(du, dv));
				
				half4 tex = tex2D(_MainTex, i.uv.xy);
				if (tex.a <= _Border)
					return tex2D(_BumpMap, i.uv.xy);

				return PackNormal(normal * 0.5 + 0.5);
			}
			ENDHLSL
		}
	}
}
