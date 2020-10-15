Shader "LiquidEffect/Jacobi"
{
    Properties
    {
        _x ("x vector: _x", 2D) = "white" {}
        _b("b vector: _b", 2D) = "white" {}
        _dx("dx: _dx", Float) = 0.5
        _alpha("alpha: _alpha", Float) = 0.5
        _rbeta("reciprocal beta: _rbeta", Float) = 0.5
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

            uniform sampler2D _x;
            uniform sampler2D _b;
            uniform float _dx;
            uniform float _alpha;
            uniform float _rbeta;

            fixed4 frag(v2f i) : SV_Target
            {
                float2 t = tex2D(_x, i.uv + float2(0.0, _dx)).xy;
                float2 b = tex2D(_x, i.uv - float2(0.0, _dx)).xy;
                float2 l = tex2D(_x, i.uv - float2(_dx, 0.0)).xy;
                float2 r = tex2D(_x, i.uv + float2(_dx, 0.0)).xy;

                float2 bCenter = tex2D(_b, i.uv).xy;

                fixed4 col = fixed4(_rbeta * (l + r + t + b + _alpha * bCenter), 0.0, 1.0);

                return col;
            }
            ENDCG
        }
    }
}
