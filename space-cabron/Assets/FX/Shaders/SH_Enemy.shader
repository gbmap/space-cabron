Shader "Unlit/SH_Enemy"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		[PerRendererData] _Damage ("Damage Float", Range(0.0, 1.0)) = 0.0
		[PerRendererData] _Spawn ("Spawn Float", Range(0.0, 1.0)) = 0.0
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

			float _Damage;
			float _Spawn;

			float2 spawnfxuv(fixed2 uv)
			{
				float s = pow(_Spawn, 0.25);
				float ofx =	(PerlinNoise2D(30.0*uv+float2(_Time.x*10.0, _Time.x*100.0)))*2.0;
				ofx += (PerlinNoise2D(3.0*uv + float2(_Time.x*10.0, _Time.x*50.0)))*10.0;
				float ofy = 0.0;
				ofy = (PerlinNoise2D(3.0*uv + float2(_Time.x*10.0, _Time.x*50.0))) * 10.0;
				float2 offset = float2(ofx, ofy );
				return offset * s * 0.05;
			}

			fixed3 spawnfx(fixed2 uv) 
			{
				return PerlinNoise2D((uv+_Time)*300.0)*10.0;
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				i.uv += spawnfxuv(i.uv)*_Spawn;
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
				col.rgb += fixed3(1.0, 1.0, 1.0) * _Damage;


				col.rgb += spawnfx(i.uv) * _Spawn;
				col.rgb *= col.a;
                // apply fog
                return col;
            }
            ENDCG
        }
    }
}
