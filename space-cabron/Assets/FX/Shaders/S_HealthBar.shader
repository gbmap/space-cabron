Shader "Unlit/S_HealthBar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NumberOfColors ("Number Of Colors", Int) = 4
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
// // Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
// #pragma exclude_renderers d3d11 gles
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
			#include "fx_utils.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            int _NumberOfColors;
            float _ColorIndexes[5];

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                const fixed4 basecolor = fixed4(230.0/255., 0., 84./255.,1.);
                const float hue_offset = 0.0685 * 3.14159268;

                fixed3 hsv = rgb_to_hsv_no_clip(basecolor.rgb);

                int index = (int)floor((1.-i.uv.y*.99f) * _NumberOfColors);
                hsv.r += hue_offset * _ColorIndexes[index];

                fixed3 rgb = hsv_to_rgb(hsv);

                return fixed4(rgb.x, rgb.y, rgb.z, 1.0);

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                return basecolor;
            }
            ENDCG
        }
    }
}
