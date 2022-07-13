Shader "WooArt/WooArt_AlphaDissolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha ("Alpha Boost", Range (0.0, 5.0)) = 1.0
        _Scroll ("Scroll Speed", Range (0.0, 2.0)) = 1.0
        _Density ("Noise Density", Range (1.0, 5.0)) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            Lighting Off
            ZWrite Off
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float3 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Alpha;
            float _Scroll;
            float _Density;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.uv.z = v.uv.z;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float agePercent = i.uv.z;
                
                fixed4 noise_1 = tex2D(_MainTex, i.uv * _Density - float2(_Time.x * 2, _Time.x * 3) * _Scroll);
                fixed4 noise_2 = tex2D(_MainTex, i.uv * _Density - float2(_Time.x * -2, _Time.x * 4) * _Scroll);
                fixed4 base = tex2D(_MainTex, i.uv);
                
                fixed4 c = (base.r * i.color);

                c.a = saturate(base.r - agePercent) * _Alpha * noise_1.g * noise_2.b;

                return c;
            }
            ENDCG
        }
    }
}
