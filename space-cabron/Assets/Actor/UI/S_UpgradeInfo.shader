Shader "Unlit/S_UpgradeInfo"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [PerRendererData] _Float ("Float", Range(0.0,1.0)) = 1.0 
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

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
            float _Float;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = -(_Float*2.-1.) * 3.14159264;

                // float ff = 3.14159264/.2;

                // to polar
                float2 uv = i.uv*2.-1.;
                float r = length(uv);
                float theta = atan2(-uv.x, -uv.y);

                float a = 1. - step(theta, t);

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= col.a;
                col.rgb*=lerp(.5, 1., a);
                col.rgb*=i.color.rgb;
                // apply fog
                return col;
            }
            ENDCG
        }
    }
}
