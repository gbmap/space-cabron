Shader "Unlit/SH_BulletTrail"
{
    Properties
    {
        [PerRendererData] _ColorIndex ("Color Index", Int) = 0
    }
    SubShader
    {
        Tags { 
			"RenderType"="Transparent" 
			"Queue"="Transparent" 
		}
		Blend One OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work

            #include "UnityCG.cginc"
			#include "noise.cginc"
			#include "fx_utils.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            const float _Pink = 0.0;
            const float _Yellow = 0.08;
            const float _Green = 0.16;
            const float _Blue = 0.24;
            int _ColorIndex = 0;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                const fixed4 basecolor = fixed4(230.0/255., 0., 84./255.,1.);

                fixed4 col = basecolor;
                float a = col.a;

                const float _Pink = 0.0;
                const float _Yellow = 0.065;
                const float _Green = 0.165;
                const float _Blue = 0.218;
                const float hueOffsets[4] = {0., _Yellow, _Green, _Blue};

                float3 hsv = rgb_to_hsv_no_clip(col.rgb);
                hsv.r += 3.14159268
                        * step(length(col.rgb-basecolor.rgb), 0.5)
                        * hueOffsets[_ColorIndex];
                        // * 0.0685
                        // *_ColorIndex;

                col.rgb = hsv_to_rgb(hsv);
				col.rgb *= a;
                col.a = i.color.a;
                return col;
            }
            ENDCG
        }
    }
}
