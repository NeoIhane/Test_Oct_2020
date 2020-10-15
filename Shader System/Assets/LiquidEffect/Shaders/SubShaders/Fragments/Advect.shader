Shader "LiquidEffect/Layers/Advect"
{
    Properties
    {
        _v("VecField: _v", 2D) = "white" {}
        _x("ColorField: _x", 2D) = "white" {}
        _dx("1/gridscale: _dx", Float) = 0.5
        _dt("Timestep: _dt", Float) = 0.5
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
            uniform float _dx;
            uniform float _dt;
            uniform sampler2D _v;
            uniform sampler2D _x;
            fixed4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 u = tex2D(_v, i.uv);
                fixed2 pastCoord = i.uv - (_dx * _dt * u);
            
            return tex2D(_x, pastCoord);
            }
            ENDCG
        }
    }
}
