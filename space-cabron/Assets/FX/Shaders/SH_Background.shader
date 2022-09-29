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

            float _EnemyKillTimes[10];
            float3 _EnemyKillPositions[10];

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

            float sqr(float2 uv, float2 sz, float2 pos, float angle) {
                float2 sqrUv = uv;
                sqrUv = mul(float2x2(
                    cos(angle), -sin(angle), sin(angle), cos(angle)
                ), sqrUv);

                pos = mul(float2x2(
                    cos(angle), -sin(angle), sin(angle), cos(angle)
                ), pos);

                float2 sqrSz = sz;
                fixed2 sqr = step(pos-sqrSz*.5, sqrUv) - step(pos+sqrSz*.5, sqrUv);
                return sqr.x*sqr.y;
            }

            float kill_animation(float2 uv, float2 kill_pos_screen, float kill_time, float t) {

                float animation_t = saturate((t-kill_time)*2.5);
                animation_t = pow(animation_t, 0.25);
                float2 bigger_square_sz = lerp(float2(0.15, 0.15), float2(0.25, 0.25), animation_t);
                float2 smaller_square_sz = lerp(
                    float2(0.00, 0.00),
                    bigger_square_sz,
                    animation_t
                );

                float angle = radians(45. +(noise(float2(kill_time, kill_time))-1.)*60.);
                float hitsquare = sqr(uv, bigger_square_sz, kill_pos_screen, angle) 
                                - sqr(uv, smaller_square_sz, kill_pos_screen, angle);
                return saturate(hitsquare);
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed2 uv = i.uv;
                fixed2 screen = i.worldPos.xy/i.worldPos.w;
                screen.y*=2.;

                uv.xy += _WorldSpaceCameraPos.xy*.1;

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
                else if (_Background == 6)
                    clr = bg07(i, uv);

                fixed3 hsv = rgb_to_hsv_no_clip(clr);
                hsv.g *= 0.725;
                clr.rgb = hsv_to_rgb(hsv);

                float expt = 0.125*step(0.5, frac(screen.y*1.5+_Time.y*0.5));
                clr.rgb *= 0.5;
                clr.rgb = pow(clr.rgb, 1.0+expt);

                clr.rgb *= 1.-line_time(_EngineTime, screen);
                float l = 0.;
                for (int i = 0; i < _NoteCount0; i++) {
                    l = max(l, line_time(_NoteTimes0[i], screen, 0.01));
                }
                clr.rgb = lerp(clr.rgb, float4(1.,1.,1.,1.)*0.5, l/(screen.y*5.));
                clr.rgb += hash(screen.xy*100.+_Time.xy)*0.1;

                float hitsquare = 0.;
                float2 hituv = screen;
                hituv.y /= 2.;
                for (int i = 0; i < 10; i++) 
                {
                    hitsquare += kill_animation(hituv, _EnemyKillPositions[i], _EnemyKillTimes[i], _EngineTime);
                }

                hsv = rgb_to_hsv_no_clip(fixed3(1.0,0.0,0.0));
                hsv.r = frac(hsv.r + _Time.y + screen.x+screen.y);
                clr.rgb = lerp(clr.rgb, hsv_to_rgb(hsv),hitsquare*0.1);

                // clr.rgb += saturate(hitsquare);


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
