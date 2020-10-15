Shader "LiquidEffect/Force"
{
    Properties
    {
        _v("Velocity Field: _v", 2D) = "white" {}
        _impulse_pos("Impulse_pos: _impulse_pos", Vector) = (0.5,0.5,0.0)
        _force("Force: _force", Vector) = (0.5,0.5,0.0)
        _dt("dt: _dt", Float) = 0.5
        _rho("rho: _rho", Float) = 0.025
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
            uniform float3 _impulse_pos;
            uniform float3 _force;
            uniform float _dt;
            uniform float _rho;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_v, i.uv);
                float2 delta = i.uv - float2(_impulse_pos.x,_impulse_pos.y);
                float scale = _dt * exp(-(pow(delta.x, 2.0) + pow(delta.y, 2.0)) / _rho);

                col.xy += scale * _force;

                return col;
            }
            ENDCG
        }
    }
}
