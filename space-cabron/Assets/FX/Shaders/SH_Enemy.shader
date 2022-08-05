Shader "Unlit/SH_Enemy"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		[PerRendererData] _Damage ("Damage Float", Range(0.0, 1.0)) = 0.0
		[PerRendererData] _Spawn ("Spawn Float", Range(0.0, 1.0)) = 0.0
        [PerRendererData] _IsResistant ("Is Resistant", Range(0.0, 1.0)) = 0.0
        _ResistantHueOffset ("Resistant Hue Offset", Float) = 0.0
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

            v2f vert (appdata v)
            {
                v2f o;
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
				i.uv += spawnfxuv(i.uv, _Spawn)*_Spawn;
                fixed4 col = tex2D(_MainTex, i.uv);
				col.rgb += fixed3(1.0, 1.0, 1.0) * damage_factor();
				col.rgb += spawnfx(i.uv) * _Spawn;
				col.rgb *= col.a;

                float3 hsv = rgb_to_hsv_no_clip(col.rgb);
                hsv.r += _IsResistant*3.14159268*_ResistantHueOffset;
                col.rgb = hsv_to_rgb(hsv);

                col.rgb *= i.color.rgb;
                return col;
            }
            ENDCG
        }
    }
}
