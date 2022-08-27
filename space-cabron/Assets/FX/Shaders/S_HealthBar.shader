Shader "Unlit/S_HealthBar"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NumberOfColors ("Number Of Colors", Int) = 4
    }
    SubShader
    {
            Blend One OneMinusSrcAlpha
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
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
            float _ColorIndexes[20];
            int _CurrentHealth;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float sdf_square(float empty_width, float2 uv)
            {
                fixed2 frame = fixed2(empty_width, empty_width);

                fixed2 ff = step(uv, fixed2(1.0,1.0)-empty_width) 
                          * step(empty_width, uv);
                float a = min(ff.x,ff.y);
                a = ff.x*ff.y;
                return a;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                const fixed4 basecolor = fixed4(230.0/255., 0., 84./255.,1.);
                const float hue_offset = 0.0685 * 3.14159268;

                fixed3 hsv = rgb_to_hsv_no_clip(basecolor.rgb);

                int index = (int)floor((1.-i.uv.y*.999f) * _NumberOfColors);
                hsv.r += hue_offset * _ColorIndexes[(_NumberOfColors-(index+1))];

                fixed3 rgb = hsv_to_rgb(hsv);

                float x = i.uv.x;
                float y = frac(i.uv.y * _NumberOfColors);
                fixed2 uv_frame = fixed2(x, y);

                float frame = sdf_square(0.00, uv_frame) - sdf_square(0.2, uv_frame);
                // rgb.rgb *= (1.-frame)*0.9;
                rgb.rgb = lerp(rgb.rgb, rgb.rgb*0.35, ceil(frame));

                float a = sdf_square(0.1, uv_frame);
                a *= step(i.uv.y, ((float)_CurrentHealth)/_NumberOfColors);
                rgb.rgb *= a;

                return fixed4(rgb.x, rgb.y, rgb.z, a);
            }
            ENDCG
        }
    }
}
