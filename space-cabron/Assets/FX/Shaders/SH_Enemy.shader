Shader "Unlit/SH_Enemy"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		[PerRendererData] _Damage ("Damage Float", Range(0.0, 1.0)) = 0.0
		[PerRendererData] _Spawn ("Spawn Float", Range(0.0, 1.0)) = 0.0
        [PerRendererData] _IsResistant ("Is Resistant", Range(0.0, 1.0)) = 0.0
        [PerRendererData] _Direction ("Direction", Vector) = (0.0, 0.0, 0.0)
        _DirectionScale ("Direction Scale", Float) = 0.35
        _ResistantHueOffset ("Resistant Hue Offset", Float) = 0.0
        [PerRendererData] _ColorIndex ("Color Index", Int) = 0
    }
    SubShader
    {
        Tags { 
			"RenderType"="Transparent" 
			"Queue"="Transparent" 
		}
		Blend One OneMinusSrcAlpha

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

			float _Damage;
			float _Spawn;
            float _IsResistant;
            float _ResistantHueOffset;
            fixed2 _Direction;
            float _DirectionScale;
            float _LastShotTime;

            const float _Pink = 0.0;
            const float _Yellow = 0.08;
            const float _Green = 0.16;
            const float _Blue = 0.24;
            int _ColorIndex = 0;
            float _EngineTime;

            float last_shot_factor() 
            {
                return 1. - saturate(_EngineTime - _LastShotTime);
            }

            v2f vert (appdata v)
            {
                v2f o;

                float z = v.vertex.z;

                float xscale = 1 + dot(v.vertex.xy, float2(0., _Direction.y))*-_DirectionScale;
                float yscale = 1 + dot(v.vertex.xy, float2(_Direction.x, 0.))*-_DirectionScale;
                v.vertex.xy *= xscale*yscale;

                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            float damage_factor()
            {
                return _Damage;
                // return saturate(_Time.x - _Damage);
            }

            fixed4 frag (v2f i) : SV_Target
            {

                const fixed4 basecolor = fixed4(230.0/255., 0., 84./255.,1.);

				i.uv += spawnfxuv(i.uv, _Spawn)*_Spawn;
                fixed4 col = tex2D(_MainTex, i.uv);
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
				col.rgb += fixed3(1.0, 1.0, 1.0) * damage_factor();
				col.rgb += spawnfx(i.uv) * _Spawn;
                col.rgb *= i.color.rgb;
                col.rgb += fixed4(1.0, 1.0, 1.0, a) * last_shot_factor();
				col.rgb *= a;
                return col;
            }
            ENDCG
        }
    }
}
