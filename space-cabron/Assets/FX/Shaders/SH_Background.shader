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

            int _NoteCount0;
            int _NoteCount1;

            float _EngineTime;

            float _NoteTimes0[100];
            float _LastNoteTimes0[100];
            float _NextNoteTimes0[100];

            float _NoteTimes1[100];
            float _LastNoteTimes1[100];
            float _NextNoteTimes1[100];

            const float4 _PlayerLinesColor0 = float4(1., 0., 0., 1.);
            const float4 _PlayerLinesColor1 = float4(0., 1., 0., 1.);

            const int _Background;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = ComputeScreenPos(o.vertex);
                return o;

            }

            float line_time(float time, float2 uv, float line_width=0.02)
            {
                float l = (uv.y-0.25)-max(0,(time-_EngineTime));
                l = smoothstep(-line_width, -line_width+0.01, l) 
                  - smoothstep(line_width-0.01, line_width, l);
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
                else if (_Background == 5)
                    clr = bg06(i, uv);

                fixed3 hsv = rgb_to_hsv_no_clip(clr);
                hsv.g *= 0.725;
                clr.rgb = hsv_to_rgb(hsv);

                clr.rgb *= 1.-line_time(_EngineTime, screen);
                float l = 0.;
                for (int i = 0; i < _NoteCount0; i++)
                {
                    l = max(l, line_time(_NoteTimes0[i], screen, 0.01));
                    l = max(l, line_time(_LastNoteTimes0[i], screen, 0.01));
                    l = max(l, line_time(_NextNoteTimes0[i], screen, 0.01));
                }
                clr.rgb = lerp(clr.rgb, float4(1.,1.,1.,1.)*0.2, l*step(screen.x, 1.0));

                // l = 0.;
                // for (int i = 0; i < _NoteCount1; i++)
                // {
                //     l = max(l, line_time(_NoteTimes1[i], screen, 0.01));
                //     l = max(l, line_time(_LastNoteTimes1[i], screen, 0.01));
                //     l = max(l, line_time(_NextNoteTimes1[i], screen, 0.01));
                // }
                // clr.rgb += float4(0.,1.,0.,1.)*0.1*l*step(1.-screen.x, 0.5);



                return clr;
            }
            ENDCG
        }
    }
}
