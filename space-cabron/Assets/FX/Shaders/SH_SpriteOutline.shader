Shader "Custom/SpriteOutline" {
	
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
	}

		SubShader{
			Tags {
			"Queue" = "Transparent"
			}
			Cull Off
			Blend One OneMinusSrcAlpha

			Pass {

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			fixed4 _Color;

			fixed4 frag(v2f i) : COLOR {
				fixed4 c = tex2D(_MainTex, i.uv);
				c.rgb *= c.a;

				half4 outline_clr = _Color;
				outline_clr.a *= ceil(c.a);
				outline_clr.rgb *= outline_clr.a;

				fixed up = tex2D(_MainTex, i.uv + fixed2(0.0, _MainTex_TexelSize.y)).a;
				fixed down = tex2D(_MainTex, i.uv - fixed2(0.0, _MainTex_TexelSize.y)).a;
				fixed left = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x, 0.0)).a;
				fixed right = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x, 0.0)).a;

				float lt = ceil(up * down * left * right);
				lt = 1.0;
				//lt = step(4.0, up + down + left + right);

				return lerp(outline_clr, c, lt);

				//return c;
			}

		ENDCG
		}
	}
}