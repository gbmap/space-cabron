Shader "Unlit/SH_Upgrade"
{
    Properties
    {
		[PerRendererData] _MainTex("Texture", 2D) = "white" {}
		[PerRendererData] _Spawn("Spawn Float", Range(0.0, 1.0)) = 0.0
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			float _Spawn;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				i.uv += spawnfxuv(i.uv, _Spawn) * _Spawn;
                fixed4 col = tex2D(_MainTex, i.uv);
				col.rgb += spawnfx(i.uv) * _Spawn;
				col.rgb *= col.a;
                return col;
            }

            ENDCG
        }
    }
}
