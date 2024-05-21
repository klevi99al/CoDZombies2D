Shader "Custom/LineSlider"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _NumSegments ("Number of Segments", Range(1, 20)) = 10
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _NumSegments;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float segmentWidth = 1.0 / _NumSegments;
                float segmentIndex = floor(i.uv.x * _NumSegments);
                float segmentStart = segmentIndex * segmentWidth;
                float segmentEnd = (segmentIndex + 1) * segmentWidth;
                float segmentCenter = (segmentStart + segmentEnd) / 2.0;
                float lineAlpha = smoothstep(segmentCenter - 0.01, segmentCenter + 0.01, i.uv.x);
                fixed4 col = tex2D(_MainTex, i.uv);
                return col * lineAlpha;
            }
            ENDCG
        }
    }
}
