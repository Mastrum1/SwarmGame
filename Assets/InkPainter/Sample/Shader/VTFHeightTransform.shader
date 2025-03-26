Shader "Es/Sample/VTFHeightTransform_URP"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {}
        _ParallaxMap("Parallax Map", 2D) = "white" {}
        _ParallaxScale("Parallax Scale", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // Include the URP core functions and transformation helpers
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // Declare our textures and parameters
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_ParallaxMap);
            SAMPLER(sampler_ParallaxMap);
            float _ParallaxScale;

            // Vertex input structure
            struct Attributes
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            // Interpolated data passed from vertex to fragment shader
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Vertex shader: displaces vertex based on the parallax map's red channel
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                // Sample the parallax map at the given UV
                float parallax = SAMPLE_TEXTURE2D(_ParallaxMap, sampler_ParallaxMap, IN.uv).r;
                // Modify the y position by the parallax value scaled by _ParallaxScale
                float4 pos = IN.position;
                pos.y += parallax * _ParallaxScale;
                // Transform the modified position from object space to clip space
                OUT.positionCS = TransformObjectToHClip(pos);
                OUT.uv = IN.uv;
                return OUT;
            }

            // Fragment shader: samples the base texture for color
            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                return color;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Forward"
}
