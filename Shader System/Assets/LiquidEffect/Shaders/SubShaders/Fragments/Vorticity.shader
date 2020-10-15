Shader "LiquidEffect/Vorticity"
{
    Properties
    {
        _v("Texture: _v", 2D) = "white" {}
        _dx("dx: _dx", Float) = 0.3
        _dt("dt: _dt", Float) = 0.5
        _vorticity("Vorticity: _vorticity", Float) = 0.5
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

            uniform sampler2D _v;
            uniform float _dx;
            uniform float _dt;
            uniform float _vorticity;

            float curl(float x, float y, sampler2D v)
            {
                float t = tex2D(v, float2(x, y + _dx)).x;
                float b = tex2D(v, float2(x, y - _dx)).x;
                float l = tex2D(v, float2(x - _dx, y)).y;
                float r = tex2D(v, float2(x + _dx, y)).y;

                return 0.5 * (t - b + l - r);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float x = i.uv.x;
                float y = i.uv.y;
             
                float dx = abs(curl(x, y - _dx, _v)) - abs(curl(x, y + _dx, _v));
                float dy = abs(curl(x + _dx, y, _v)) - abs(curl(x - _dx, y, _v));
                float2 d = float2(0.5 * dx, 0.5 * dy);
                float len = length(d) + 1e-9;
                d = _vorticity / len * d;

                fixed4 col = tex2D(_v, i.uv);
                col.rgb = 1 - col.rgb;
                return  tex2D(_v, i.uv) + _dt * curl(i.uv.x, i.uv.y, _v) * float4(d, 0, 0);
            }
            ENDCG
        }
    }
}
