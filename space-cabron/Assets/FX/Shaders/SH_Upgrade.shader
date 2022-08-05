Shader "Unlit/SH_Upgrade"
{
    Properties
    {
		[PerRendererData] _MainTex("Texture", 2D) = "white" {}
        // [PerRendererData] _Color("Color", Color) = (1.0, 0.0, 0.0, 0.0)
		[PerRendererData] _Spawn("Spawn Float", Range(0.0, 1.0)) = 0.0
		[PerRendererData] _Upgrade("Upgrade Float", Float) = 0.0

		_NegativeColor("Negative Color", Color) = (1.0, 0.0, 0.0, 0.0)
		_PositiveColor("Positive Color", Color) = (0.0, 1.0, 0.0, 0.0)
    }
    SubShader
    {
		Tags {
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}
		Blend One OneMinusSrcAlpha

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 vertex_color : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			float _Spawn;
			float _Upgrade;

			float4 _NegativeColor;
			float4 _PositiveColor;

            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.vertex_color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				i.uv += spawnfxuv(i.uv, _Spawn) * _Spawn;
                fixed4 col = tex2D(_MainTex, i.uv);

				if (length(col.rgb) < 0.1)
				{
					col.rgb = lerp(
						_NegativeColor.rgb,
						_PositiveColor.rgb,
						_Upgrade
					);
				}

				col.rgb += spawnfx(i.uv) * _Spawn;
				col.rgb *= col.a;
                col.rgb *= i.vertex_color.rgb;
				//col.rgb = fixed3(_Upgrade, _Upgrade, _Upgrade);
                return col;
            }

            ENDCG
        }
    }
}
