Shader "Blackjack/UI Shine"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _ShineColor ("Shine Color", Color) = (1,0.95,0.75,0.8)
        _ShineWidth ("Shine Width", Range(0.01, 0.5)) = 0.12
        _ShineSpeed ("Shine Speed", Float) = 0.6
        _ShinePause ("Pause Between Sweeps", Float) = 2.5
        _ShineAngle ("Shine Slant", Range(-1, 1)) = 0.35

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _ShineColor;
            float _ShineWidth;
            float _ShineSpeed;
            float _ShinePause;
            float _ShineAngle;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                float cycle = 1.0 / max(_ShineSpeed, 0.01) + _ShinePause;
                float t = fmod(_Time.y, cycle) * _ShineSpeed;
                float center = lerp(-_ShineWidth - 0.5, 1.5 + _ShineWidth, saturate(t));

                float x = i.uv.x + (i.uv.y - 0.5) * _ShineAngle;
                float band = 1.0 - saturate(abs(x - center) / _ShineWidth);
                band = band * band;

                col.rgb += _ShineColor.rgb * _ShineColor.a * band * col.a;
                return col;
            }
            ENDCG
        }
    }
}
