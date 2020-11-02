Shader "Custom/TerrainShader" {
    Properties{
        _GTex("grass texture", 2D) = "white" {}
        _RTex("rock texture", 2D) = "white" {}
        _HTex("heightMap texture", 2D) = "white" {}
    }

    SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 2048

            CGPROGRAM
            #pragma surface surf Lambert
            sampler2D _GrassTex;
            sampler2D _RockTex;
            sampler2D _HeightTex;
            struct Input {
                float2 uv_GTex;
                float2 uv_RTex;
                float2 uv_HTex;
            };

            void surf(Input IN, inout SurfaceOutput o) {
                float4 grass = tex2D(_GTex, IN.uv_GTex);
                float4 rock = tex2D(_RTex, IN.uv_RTex);
                float4 height = tex2D(_HTex, IN.uv_HTex);

                o.Albedo = lerp(grass, rock, height);
            }
            ENDCG
    }
        FallBack "Diffuse"
}