Shader "GOF/Echolocation"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _RingColor("Ring Color", Color) = (1, 1, 1, 1) // �� ����, Inspector���� ����
        _RingColorIntensity("Ring Color Intensity", Float) = 2
        _RingSpeed("Ring Speed", Float) = 1
        _RingWidth("Ring Width", Float) = 0.1
        _RingIntensityScale("Ring Intensity Scale", Float) = 1
        _RingTex("Ring Texture", 2D) = "white" {}
        _OutlineColor("OutlineColor", Color) = (0, 0, 0, 1)
        _OutlineWidth("Outline Width", float) = 0.02

        _RingFadeDuration("Ring Fade Duration", Float) = 2
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200

        // �ĵ� �׸��� �κ�
        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            Stencil
            {
                Ref 1               // ���ٽ� �� 1�� ����
                Comp always         // �׻� �����Ŵ
                Pass replace        // ���ٽ� ���� 1�� ����
            }


            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _MainTex;
            sampler2D _RingTex;

            float4 _BaseColor;
            float4 _RingColor;
            float _RingColorIntensity;
            float _RingSpeed;
            float _RingWidth;
            float _RingIntensityScale;
            float _RingFadeDuration;

            float4 _hitPts[20];
            float _StartTime;
            float _Intensity[20];

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                o.worldPos = TransformObjectToWorld(v.positionOS).xyz;
                return o;
            }

            float4 frag(Varyings i) : SV_Target
            {
                float4 baseColor = tex2D(_MainTex, i.uv) * _BaseColor;
                float3 finalColor = baseColor.rgb;

                float diffFromRingCol = abs(finalColor.r - _RingColor.r) +
                                        abs(finalColor.g - _RingColor.g) +
                                        abs(finalColor.b - _RingColor.b);

                for (int idx = 0; idx < 20; idx++)
                {
                    float3 hitPos = _hitPts[idx].xyz;
                    float hitTime = _hitPts[idx].w;
                    float intensity = _Intensity[idx] * _RingIntensityScale;

                    float dist = distance(hitPos, i.worldPos);
                    float ringStart = (_Time.y - hitTime) * _RingSpeed - _RingWidth;
                    float ringEnd = (_Time.y - hitTime) * _RingSpeed;

                    if (dist > ringStart && dist < ringEnd)
                    {
                        float ringProgress = (dist - ringStart) / _RingWidth;
                        float val = (1 - (dist / intensity)) *
                                    tex2D(_RingTex, float2(1 - ringProgress, 0.5)).r;

                        if (val > 0)
                        {
                            float3 ringColor = _RingColor.rgb * val * _RingColorIntensity;
                            float3 blendedColor = lerp(finalColor, ringColor, val);

                            float newDiffFromRingCol = abs(blendedColor.r - _RingColor.r) +
                                                        abs(blendedColor.g - _RingColor.g) +
                                                        abs(blendedColor.b - _RingColor.b);

                            if (newDiffFromRingCol < diffFromRingCol)
                            {
                                finalColor = blendedColor;
                                diffFromRingCol = newDiffFromRingCol;
                            }
                        }
                    }
                }

                return float4(finalColor, baseColor.a);
            }
            ENDHLSL
        }

        // �ƿ� ���� �׸��� �κ�
        Pass
        {
            Name "OUTLINE"
            Cull Front
            ZWrite On
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

            Stencil
            {
                Ref 1               // ���ٽ� ������ ���� ���� 1�� ����
                Comp always         // �׻� ��(true)
                Pass replace        // ���ٽ� �׽�Ʈ�� ����ϸ� ���ٽ� ���� 1�� ����
                Fail keep           // ���ٽ� �׽�Ʈ�� �����ϸ� ���� ����
                ZFail keep          // ���� �׽�Ʈ ���н� ���ٽ� ���� ����
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0; // ���� ��ǥ �߰�
            };

            float _OutlineWidth;
            float4 _OutlineColor;
            float4 _RingColor; // �� ���� �߰�
            float _RingSpeed;
            float _RingWidth;
            float4 _hitPts[20];
            float _StartTime;
            float _RingFadeDuration;


            v2f vert(appdata v)
            {
                v2f o;

                // if (_OutlineWidth == 0)
                //     _OutlineWidth = 2;

                float3 cameraForwardWorld = mul((float3x3)unity_CameraToWorld, float3(0, 0, 1));
                float3 cameraForwardObject = mul((float3x3)unity_WorldToObject, cameraForwardWorld);

                float3 tangent = cross(cameraForwardObject, v.normal);
                float3 projectedNormal = cross(cameraForwardObject, tangent);

                projectedNormal = normalize(-projectedNormal) * _OutlineWidth;

                v.vertex += float4(projectedNormal, 0);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // ���� ��ǥ ���
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = _OutlineColor;
                col.a = 0; // �⺻ ���İ� 0

                float mostRecentTime = -1.0; // ���� �ֱ� �ĵ��� �ð�
                float3 mostRecentPos = float3(0, 0, 0); // ���� �ֱ� �ĵ��� ��ġ

                for (int idx = 0; idx < 20; idx++)
                {
                    float3 hitPos = _hitPts[idx].xyz;  // �ĵ��� ��ġ
                    float hitTime = _hitPts[idx].w;   // �ĵ��� ���� �ð�

                    float dist = distance(hitPos, i.worldPos);  // ���� �ȼ��� �ĵ��� �Ÿ�
                    float ringStart = (_Time.y - hitTime) * _RingSpeed - _RingWidth;
                    float ringEnd = (_Time.y - hitTime) * _RingSpeed;

                    // ���� �� �ȼ��� ������ ���
                    if (dist < ringStart && hitTime > mostRecentTime)
                    {
                        mostRecentTime = hitTime;   // ���� �ֱ� �ð� ������Ʈ
                        mostRecentPos = hitPos;    // ���� �ֱ� ��ġ ������Ʈ
                    }
                }

                // ���� �ֱٿ� ������ �� �ĵ��� ���� ��� ���̵� ����
                if (mostRecentTime > 0)
                {
                    float fadeTime = _RingFadeDuration;
                    float fadeProgress = 1 - ((_Time.y - mostRecentTime) / fadeTime);
                    fadeProgress = saturate(fadeProgress); // 0~1�� ����
                    col = _RingColor; // �� ���� ����
                    col.a = fadeProgress; // ���İ��� ���̵� ���α׷���
                }

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDHLSL
        }
    }

    FallBack "Diffuse"
}