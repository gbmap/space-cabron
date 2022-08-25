Shader "Unlit/Background"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Background ("Background", Int) = 0 
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work

            #include "UnityCG.cginc"
			#include "backgrounds.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            const int _Background;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed2 uv = i.uv;

                fixed4 clr = fixed4(0., 0., 0., 1.);
                if (_Background == 0)
                    clr = bg01(i, uv);
                else if (_Background == 1)
                    clr = bg02(i, uv);
                else if (_Background == 2)
                    clr = bg03(i, uv);
                else if (_Background == 3)
                    clr = bg04(i, uv);

                fixed3 hsv = rgb_to_hsv_no_clip(clr);
                hsv.g = 0.625;
                hsv.b = .125*beat_curve();
                clr.rgb = hsv_to_rgb(hsv);

                return clr;
            }
            ENDCG
        }
    }
}
