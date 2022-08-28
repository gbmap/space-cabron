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

            struct fragmentData
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _NextNoteTime;

            int _NoteCount;
            float _EngineTime;
            float _NoteTimes[100];
            float _LastNoteTimes[100];
            float _NextNoteTimes[100];
            const int _Background;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = ComputeScreenPos(o.vertex);
                return o;

            }

            float line_time(float time, float2 uv)
            {
                float l = (uv.y-0.25)-max(0,(time-_EngineTime));
                l = smoothstep(-0.02, 0.00, l) - smoothstep(0.00, 0.02, l);
                return l;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed2 uv = i.uv;
                fixed2 screen = i.worldPos.xy/i.worldPos.w;
                screen.y*=2.;

                fixed4 clr = fixed4(0., 0., 0., 1.);
                if (_Background == 0)
                    clr = bg01(i, uv);
                else if (_Background == 1)
                    clr = bg02(i, uv);
                else if (_Background == 2)
                    clr = bg03(i, uv);
                else if (_Background == 3)
                    clr = bg04(i, uv);
                else if (_Background == 4)
                    clr = bg05(i, uv);


                fixed3 hsv = rgb_to_hsv_no_clip(clr);
                hsv.g *= 0.725;
                clr.rgb = hsv_to_rgb(hsv);

                clr.rgb *= 1.-line_time(_EngineTime, screen);
                float l = 0.;
                for (int i = 0; i < _NoteCount; i++)
                {
                    l = max(l, line_time(_NoteTimes[i], screen));
                    // l = max(l, line_time(_LastNoteTimes[i], screen));
                    // l = max(l, line_time(_NextNoteTimes[i], screen));
                }


                clr.rgb += fixed4(1.,1.,1.,1.)*l*.25;


                return clr;
            }
            ENDCG
        }
    }
}
