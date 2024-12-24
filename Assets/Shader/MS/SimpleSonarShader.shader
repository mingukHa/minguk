Shader "MadeByProfessorOakie/URP_SimpleSonarShader"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _RingColor("Ring Color", Color) = (1, 1, 1, 1) // 링 색상, Inspector에서 설정
        _RingColorIntensity("Ring Color Intensity", Float) = 2
        _RingSpeed("Ring Speed", Float) = 1
        _RingWidth("Ring Width", Float) = 0.1
        _RingIntensityScale("Ring Intensity Scale", Float) = 1
        _RingTex("Ring Texture", 2D) = "white" {}
        _OutlineColor("OutlineColor", Color) = (0, 0, 0, 1)
        _OutlineWidth("Outline Width", float) = 0.02
        _DistanceFactor("Distance Factor", Float) = 0.1
        _OutlineAlpha("Outline Alpha", Float) = 1.0


        _RingFadeDuration("Ring Fade Duration", Float) = 2
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200

        // 파동 그리는 부분
        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

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

            float4 _hitPts[100];
            float _StartTime;
            float _Intensity[100];

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

                for (int idx = 0; idx < 100; idx++)
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

        // 아웃 라인 그리는 부분
        Pass
        {
            Name "OUTLINE"
            Cull Front
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha

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
                float3 worldPos : TEXCOORD0; // 월드 좌표 추가
                float3 originalWorldPos : TEXCOORD1; // 원래 위치의 월드 좌표 추가
            };

            float _OutlineWidth;
            float4 _OutlineColor;
            float4 _RingColor; // 링 색상 추가
            float _RingSpeed;
            float _RingWidth;
            float4 _hitPts[100];
            float _StartTime;
            float _RingFadeDuration;
            float _OutlineAlpha;
            float _DistanceFactor; // 거리 기반 스케일링을 위한 팩터


            v2f vert(appdata v)
            {
                v2f o;

                // 월드 공간에서 정점 위치 계산
                o.originalWorldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // 오브젝트 중심 계산 (local space에서 원점 기준)
                float3 objectCenterWorld = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;

                // 중심에서 정점으로 향하는 벡터
                float3 outlineDirection = normalize(o.originalWorldPos - objectCenterWorld);

                // 카메라와의 거리 비례한 스케일링
                float distanceFromCamera = distance(o.originalWorldPos, _WorldSpaceCameraPos);
                float outlineScale = _OutlineWidth * (1.0 + _DistanceFactor * distanceFromCamera);

                // 아웃라인을 중심 방향으로 확장
                float3 projectedNormal = outlineDirection * outlineScale;

                // 정점 변위
                v.vertex.xyz += projectedNormal;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // 변위된 월드 좌표 계산
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = _OutlineColor;
                col.a = _OutlineAlpha;; // 기본 알파값 1



                float mostRecentTime = -1.0; // 가장 최근 파동의 시간
                float3 mostRecentPos = float3(0, 0, 0); // 가장 최근 파동의 위치

                for (int idx = 0; idx < 100; idx++)
                {
                    float3 hitPos = _hitPts[idx].xyz;  // 파동의 위치
                    float hitTime = _hitPts[idx].w;   // 파동의 시작 시간

                    float dist = distance(hitPos, i.originalWorldPos);  // 현재 픽셀과 파동의 거리
                    float ringStart = (_Time.y - hitTime) * _RingSpeed - _RingWidth;
                    float ringEnd = (_Time.y - hitTime) * _RingSpeed;

                    // 링이 이 픽셀을 지나간 경우
                    if (dist < ringStart && hitTime > mostRecentTime)
                    {
                        mostRecentTime = hitTime;   // 가장 최근 시간 업데이트
                        mostRecentPos = hitPos;    // 가장 최근 위치 업데이트
                    }
                }

                // 가장 최근에 영향을 준 파동이 있을 경우 페이드 적용
                if (mostRecentTime > 0)
                {
                    float fadeTime = _RingFadeDuration;
                    float fadeProgress = 1 - ((_Time.y - mostRecentTime) / fadeTime);
                    fadeProgress = saturate(fadeProgress); // 0~1로 제한
                    //col = _RingColor; // 링 색상 적용
                    col.a = fadeProgress; // 알파값은 페이드 프로그래스
                }

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDHLSL
        }
    }

    FallBack "Diffuse"
}