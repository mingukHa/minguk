Shader "mg"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {} 
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1) // ��� �ܰ���
        _OutlineWidth ("Outline Width", Float) = 0.05 // �ν����� ����
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        // �н� 1: �ܰ��� ������
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "UniversalForward" } // URP�� �´� LightMode ����
            Cull Front // �ո� ����, �޸鸸 ������
            ZWrite Off
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag 
            #include "UnityCG.cginc"

            float4 _OutlineColor;
            float _OutlineWidth;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal); // ��� ���͸� ����ȭ
                v.vertex.xyz += norm * _OutlineWidth; // �ƿ����� �ܰ� ��ġ
                o.pos = UnityObjectToClipPos(v.vertex); // ������ ����
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return _OutlineColor; // �ܰ��� ��
            }
            ENDHLSL
        }

        // �н� 2: ��ü ������
        Pass
        {
            Name "MainObject"
            Tags { "LightMode" = "UniversalForward" } // URP�� �´� LightMode ����
            Cull Back // �ܰ��� �ø�
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv); // ���� �ؽ�ó �� ��ȯ
            }
            ENDHLSL
        }
    }
    FallBack "UniversalForward" // URP�� fallback ����
}

