// Entorno con contraste y saturación ajustables para la UI: el UI/Default de Unity con dos pasos
// más antes de teñir —acercar o alejar cada color de su gris, y separar los tonos alrededor del
// gris medio—. Lo usa el laberinto del Nivel 2, cuyo suelo
// es plano y bajo la luz de atardecer pierde la lectura de piedras y setos (MazeLayout.Contrast).
// Soporta la máscara y el recorte de uGUI igual que el shader por defecto.
Shader "Algoritm/UI Contraste"
{
    Properties
    {
        [PerRendererData] _MainTex ("Textura", 2D) = "white" {}
        _Color ("Tinte", Color) = (1, 1, 1, 1)
        _Contrast ("Contraste (1 = sin cambio)", Range(0.5, 2)) = 1
        _Saturation ("Saturación (1 = sin cambio)", Range(0, 2)) = 1

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }

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
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 world : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            float _Contrast;
            float _Saturation;
            float4 _ClipRect;

            v2f vert(appdata v)
            {
                v2f o;
                o.world = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                float luma = dot(c.rgb, float3(0.299, 0.587, 0.114));
                c.rgb = lerp(luma.xxx, c.rgb, _Saturation);
                c.rgb = saturate((c.rgb - 0.5) * _Contrast + 0.5);
                c *= i.color;
                c.a *= UnityGet2DClipping(i.world.xy, _ClipRect);
                return c;
            }
            ENDCG
        }
    }
}
