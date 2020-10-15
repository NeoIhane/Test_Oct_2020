Shader "LiquidEffect/Dye"
{
    Properties
    {
        _colorField("Color Field: _colorField", 2D) = "white" {}
        _rho("rho: _rho", Float) = 0.025
        //_dt("dt: _dt", Float) = 0.5
        _color("Color: _color", Color) = (1.0,1.0,1.0,1.0)
        _impulse_pos("Impulse Position: _impulse_pos", Vector) = (0.5,0.5,0.0)
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

            uniform sampler2D _colorField;
            //uniform float _dt;
            uniform float _rho;
            uniform float4 _color;
            uniform float3 _impulse_pos;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_colorField, i.uv);

                float eps = _rho;
                float2 delta = i.uv - float2(_impulse_pos.x, _impulse_pos.y);
                if (length(delta) < eps)
                {
                    col.xyz = _color;
                }
                return col;
            }

            ENDCG
        }
    }
}
