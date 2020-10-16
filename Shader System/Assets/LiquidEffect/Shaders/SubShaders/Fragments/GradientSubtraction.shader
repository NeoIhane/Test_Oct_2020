Shader "LiquidEffect/GradientSubtraction"
{
    Properties
    {
        _p ("Pressure: _p", 2D) = "white" {}
        _w("Velocity: _w", 2D) = "white" {}
        _dx("dx: _dx", Float) = 0.5
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            uniform sampler2D _p;
            uniform sampler2D _w;
            uniform float _dx;

            fixed4 frag (v2f i) : SV_Target
            {
                float t = tex2D(_p, i.uv + float2(0.0, _dx)).x;
                float b = tex2D(_p, i.uv - float2(0.0, _dx)).x;
                float l = tex2D(_p, i.uv - float2(_dx, 0.0)).x;
                float r = tex2D(_p, i.uv + float2(_dx, 0.0)).x;

                fixed4 col = tex2D(_w, i.uv);
                float half_rdx = 1.0 / (2.0 * _dx);
                col.xy -= half_rdx * float2((r - l), (t - b));
                
                return col;
            }
            ENDCG
        }
    }
}
