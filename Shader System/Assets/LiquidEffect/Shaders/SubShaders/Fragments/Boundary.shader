Shader "LiquidEffect/Boundary"
{
    Properties
    {
        _x("State field: _x", 2D) = "white" {}
        _dx("Boundary offset: _dx", Float) = 0.5
        _scale("Scale: _Scale", Float) = 0.5
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
            uniform float _dx;
            uniform float _scale;

            fixed4 frag(v2f i) : SV_Target
            {
                float2 _offset = float2(0.0, 0.0);
                if (i.uv.x < _dx) _offset = float2(_dx, 0.0);
                else if (1.0 - i.uv.x < _dx) _offset = float2(-_dx, 0.0);
                else if (i.uv.y < _dx) _offset = float2(0.0, _dx);
                else if (1.0 - i.uv.y < _dx) _offset = float2(0.0, -_dx);
                else return tex2D(_x, i.uv);

                float2 col = _scale * tex2D(_x, i.uv + _offset).xy;
                return float4(col, 0.0, 1.0);
            }
            ENDCG
        }
    }
}
