// Capa de oscuridad del Nivel 1 (docs/Camara_Narrativa_N1.md §4): un charco de luz radial sobre
// un fondo atenuado, multiplicado sobre lo que ya está pintado (Blend DstColor Zero). Sin luces
// ni materiales por objeto: un solo Image a pantalla completa con este material.
// uv = fracciones del rect del Image; el centro y el radio se pasan ya en esas fracciones y
// _Aspect corrige el ancho/alto para que el charco sea redondo.
Shader "Algoritm/Oscuridad"
{
    Properties
    {
        // Sin uso, pero obligatoria: uGUI asigna material.mainTexture a todo Image y sin ella
        // registra un error por cuadro que tumba cualquier prueba (LogAssert).
        [PerRendererData] _MainTex ("Textura (sin uso)", 2D) = "white" {}
        _Center ("Centro (uv)", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Radio (altos)", Float) = 0
        _Ambient ("Fondo", Range(0, 1)) = 1
        _Tint ("Tinte", Color) = (1, 1, 1, 1)
        _Edge ("Borde (fracción del radio)", Range(0, 1)) = 0.5
        _Aspect ("Ancho / alto", Float) = 1
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" "PreviewType" = "Plane" }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend DstColor Zero

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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Center;
            float _Radius;
            float _Ambient;
            fixed4 _Tint;
            float _Edge;
            float _Aspect;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 d = float2((i.uv.x - _Center.x) * _Aspect, i.uv.y - _Center.y);
                float dist = length(d);
                float light = _Radius > 0 ? 1 - smoothstep(_Radius * (1 - _Edge), _Radius, dist) : 0;
                float brightness = _Ambient + (1 - _Ambient) * light;
                return fixed4(_Tint.rgb * brightness, 1);
            }
            ENDCG
        }
    }
}
