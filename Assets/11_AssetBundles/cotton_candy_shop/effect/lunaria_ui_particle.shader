Shader "Lu/lunaria_ui_particle"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcMode ("SrcMode", int) = 5
        [Enum(Alpha Blend, 10, Additive, 1)] _DstMode ("Blend Mode", int) = 10
        [Enum(2Side, 0, Front, 2, Back, 1)] _Culling ("Show Mesh", int) = 0
        [Enum(Off, 0, On, 1)] _Zwrite ("ZWrite", int) = 0

        _MainTex ("Main Texture", 2D) = "white" {}
        _MtexSpdX ("   - MTex Flow Speed X", Range (-10,10) ) = 0.0
        _MtexSpdY ("   - MTex Flow Speed Y", Range (-10,10) ) = 0.0
        [HDR] _MtexC ("   - MTex Color", Color) = (1,1,1,1)

        _SubTex ("Sub Texture", 2D) = "white" {}
        _SBMix("   - SBTex Mix", Range (0,1) ) = 0.0
        _SBtexSpdX ("   - SBTex Flow Speed X", Range (-10,10) ) = 0.0
        _SBtexSpdY ("   - SBTex Flow Speed Y", Range (-10,10) ) = 0.0
        [HDR] _SBtexC ("   - SBTex Color", Color) = (1,1,1,1)

        _Atex ("Alphamask Texture", 2D) = "white" {}
        _Ai("   - ATex Intensity", Range (0,1) ) = 1.0
        _AtexSpdX ("   - ATex Flow Speed X", Range (-10,10) ) = 0.0
        _AtexSpdY ("   - ATex Flow Speed Y", Range (-10,10) ) = 0.0

        _Dtex ("Distortion Texture", 2D) = "white" {}
        _Di("   - DTex Intensity", Range (0,1) ) = 0.0
        _SBDi("   - DTex Sub Intensity", Range (0,1) ) = 0.0
        _DtexSpdX ("   - DTex Flow Speed X", Range (-10,10) ) = 0.0
        _DtexSpdY ("   - DTex Flow Speed Y", Range (-10,10) ) = 0.0

        _Dstex ("Dissolve Texture", 2D) = "white" {}
        _Dsi("   - DsTex Intensity", Range (0,1) ) = 0.0
        _DsEdgeRange("   - DsTex Edge Range", Range (0,1) ) = 0.0
        _DsEdgeMul ("   - DsTex Edge Multiplier", Range (-10,10) ) = 0.0
        [HDR] _DsEdgeC("   - DsTex Color", Color) = (1,1,1,1)
        _DstexSpdX ("   - DsTex Flow Speed X", Range (-10,10) ) = 0.0
        _DstexSpdY ("   - DsTex Flow Speed Y", Range (-10,10) ) = 0.0

        _Fade ("Fade", Range (0, 1) ) = 1.0

        [Toggle(_USE_UNSCALED_TIME)] _UseUnscaledTime ("Use Unscaled time", Float) = 0
        _UnscaledTime ("Unscaled time", Vector) = (0, 0, 0, 0)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
        _ClipRect ("Clip Rect", Vector) = (-32767, -32767, 32767, 32767)
    }
    SubShader
    {
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull [_Culling]
        Lighting Off
        ZWrite [_Zwrite]
        ZTest [unity_GUIZTestMode]
        ColorMask [_ColorMask]
        Blend [_SrcMode] [_DstMode]

        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "PerformanceChecks"="False"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_instancing
            #pragma shader_feature _USE_UNSCALED_TIME
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata
            {
                float4 col : COLOR;
                float4 vtx : POSITION;
                float2 MtexUV : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexToFlag
            {
                float4 col : COLOR;
                float4 vtx : SV_POSITION;
                float2 MtexUV : TEXCOORD0;
                float2 SBtexUV : TEXCOORD1;
                float2 AtexUV : TEXCOORD2;
                float2 DtexUV : TEXCOORD3;
                float2 DstexUV : TEXCOORD4;
                float4 worldPosition : TEXCOORD5;
                half4 mask : TEXCOORD6;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _UnscaledTime;
            float4 _ClipRect;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;

            sampler2D _MainTex;
            sampler2D _SubTex;
            sampler2D _Atex;
            sampler2D _Dtex;
            sampler2D _Dstex;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
            UNITY_DEFINE_INSTANCED_PROP(float4, _MtexC)
            UNITY_DEFINE_INSTANCED_PROP(float, _MtexSpdX)
            UNITY_DEFINE_INSTANCED_PROP(float, _MtexSpdY)

            UNITY_DEFINE_INSTANCED_PROP(float4, _SubTex_ST)
            UNITY_DEFINE_INSTANCED_PROP(float4, _SBtexC)
            UNITY_DEFINE_INSTANCED_PROP(float, _SBMix)
            UNITY_DEFINE_INSTANCED_PROP(float, _SBtexSpdX)
            UNITY_DEFINE_INSTANCED_PROP(float, _SBtexSpdY)

            UNITY_DEFINE_INSTANCED_PROP(float4, _Atex_ST)
            UNITY_DEFINE_INSTANCED_PROP(float, _Ai)
            UNITY_DEFINE_INSTANCED_PROP(float, _AtexSpdX)
            UNITY_DEFINE_INSTANCED_PROP(float, _AtexSpdY)

            UNITY_DEFINE_INSTANCED_PROP(float4, _Dtex_ST)
            UNITY_DEFINE_INSTANCED_PROP(float, _Di)
            UNITY_DEFINE_INSTANCED_PROP(float, _SBDi)
            UNITY_DEFINE_INSTANCED_PROP(float, _DtexSpdX)
            UNITY_DEFINE_INSTANCED_PROP(float, _DtexSpdY)

            UNITY_DEFINE_INSTANCED_PROP(float4, _Dstex_ST)
            UNITY_DEFINE_INSTANCED_PROP(float, _Dsi)
            UNITY_DEFINE_INSTANCED_PROP(float, _DsEdgeRange)
            UNITY_DEFINE_INSTANCED_PROP(float4, _DsEdgeC)
            UNITY_DEFINE_INSTANCED_PROP(float, _DsEdgeMul)
            UNITY_DEFINE_INSTANCED_PROP(float, _DstexSpdX)
            UNITY_DEFINE_INSTANCED_PROP(float, _DstexSpdY)

            UNITY_DEFINE_INSTANCED_PROP(float, _Fade)

            UNITY_INSTANCING_BUFFER_END(Props)


            VertexToFlag vert(appdata v)
            {
                VertexToFlag o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.worldPosition = v.vtx;
                o.vtx = UnityObjectToClipPos(v.vtx);

                float2 pixelSize = o.vtx.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vtx.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                o.mask = half4(v.vtx.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

                o.col = v.col;

#if _USE_UNSCALED_TIME
                float time = _UnscaledTime.g;
#else
                float time = _Time.g;
#endif

                o.MtexUV = v.MtexUV * UNITY_ACCESS_INSTANCED_PROP(Props, _MainTex_ST) + UNITY_ACCESS_INSTANCED_PROP(Props, _MainTex_ST).zw + (float2(
                    UNITY_ACCESS_INSTANCED_PROP(Props, _MtexSpdX), UNITY_ACCESS_INSTANCED_PROP(Props, _MtexSpdY)) * time);
                o.SBtexUV = v.MtexUV * UNITY_ACCESS_INSTANCED_PROP(Props, _SubTex_ST) + UNITY_ACCESS_INSTANCED_PROP(Props, _SubTex_ST).zw + (float2(
                    UNITY_ACCESS_INSTANCED_PROP(Props, _SBtexSpdX), UNITY_ACCESS_INSTANCED_PROP(Props, _SBtexSpdY)) * time);
                o.AtexUV = v.MtexUV * UNITY_ACCESS_INSTANCED_PROP(Props, _Atex_ST) + UNITY_ACCESS_INSTANCED_PROP(Props, _Atex_ST).zw + (float2(
                    UNITY_ACCESS_INSTANCED_PROP(Props, _AtexSpdX), UNITY_ACCESS_INSTANCED_PROP(Props, _AtexSpdY)) * time);
                o.DtexUV = v.MtexUV * UNITY_ACCESS_INSTANCED_PROP(Props, _Dtex_ST) + UNITY_ACCESS_INSTANCED_PROP(Props, _Dtex_ST).zw + (float2(
                    UNITY_ACCESS_INSTANCED_PROP(Props, _DtexSpdX), UNITY_ACCESS_INSTANCED_PROP(Props, _DtexSpdY)) * time);
                o.DstexUV = v.MtexUV * UNITY_ACCESS_INSTANCED_PROP(Props, _Dstex_ST) + UNITY_ACCESS_INSTANCED_PROP(Props, _Dstex_ST).zw + (float2(
                    UNITY_ACCESS_INSTANCED_PROP(Props, _DstexSpdX), UNITY_ACCESS_INSTANCED_PROP(Props, _DstexSpdY)) * time);
                return o;
            }


            float4 frag(VertexToFlag i) : SV_Target
            {

                //디스토션 텍스쳐 컬러 받기
                float4 dc = tex2D(_Dtex, i.DtexUV);

                //디스토션 계산 (메인 텍스쳐와 서브 텍스쳐 모두 적용)
                float di = UNITY_ACCESS_INSTANCED_PROP(Props, _Di);
                float2 dval = float2((dc.r * di) - (0.5 * di), (dc.g * di) - (0.5 * di));
                i.MtexUV += (dval * UNITY_ACCESS_INSTANCED_PROP(Props, _Di));
                i.SBtexUV += (dval * UNITY_ACCESS_INSTANCED_PROP(Props, _SBDi));

                //메인 텍스쳐 컬러 계산
                float4 mc = tex2D(_MainTex, i.MtexUV) * i.col;

                //메인 텍스쳐에 서브 텍스쳐 섞기
                float4 sbc = tex2D(_SubTex, i.SBtexUV) * UNITY_ACCESS_INSTANCED_PROP(Props, _SBtexC);
                mc.rgb = lerp(mc.rgb, sbc.rgb, sbc.a * UNITY_ACCESS_INSTANCED_PROP(Props, _SBMix));

#ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(i.mask.xy)) * i.mask.zw);
                mc.a *= m.x * m.y;
#endif

                //알파 마스크
                float4 ac = tex2D(_Atex, i.AtexUV);
                mc.a *= lerp(mc.a, ac.r, UNITY_ACCESS_INSTANCED_PROP(Props, _Ai));

                //디졸브 적용
                float dsClip = tex2D(_Dstex, i.DstexUV).r - UNITY_ACCESS_INSTANCED_PROP(Props, _Dsi);
                float edgeRamp = max(0, UNITY_ACCESS_INSTANCED_PROP(Props, _DsEdgeRange) - dsClip);
                clip(dsClip);
                mc.rgb = lerp(mc.rgb, -UNITY_ACCESS_INSTANCED_PROP(Props, _DsEdgeC).rgb, min(1, edgeRamp * UNITY_ACCESS_INSTANCED_PROP(Props, _DsEdgeMul)));

                /* clip()을 사용하지 않은 연산으로 바꾸다가 색이 제대로 안나오게 되었습니다. wine빌드 끝나고 다시 만져보려고 합니다.
                float dsTexA = tex2D(_Dstex, i.DstexUV).r - UNITY_ACCESS_INSTANCED_PROP(Props, _Dsi);
                float dsProgress = ((UNITY_ACCESS_INSTANCED_PROP(Props, _DsEdgeRange) - dsTexA) * UNITY_ACCESS_INSTANCED_PROP(Props, _DsEdgeMul));
                float edgeRamp = max(0, UNITY_ACCESS_INSTANCED_PROP(Props, _DsEdgeRange) - dsTexA);
                mc.rgb = lerp(mc.rgb, UNITY_ACCESS_INSTANCED_PROP(Props, _DsEdgeC).rgb, min(1, edgeRamp * UNITY_ACCESS_INSTANCED_PROP(Props, _DsEdgeMul)));
                mc.a *= clamp(lerp(dsTexA, -UNITY_ACCESS_INSTANCED_PROP(Props, _DsEdgeC).a, dsProgress-1), 0, 1);
                */

                //메인 텍스쳐 컬러 최종 적용
                mc *= UNITY_ACCESS_INSTANCED_PROP(Props, _MtexC);
                mc.a *= UNITY_ACCESS_INSTANCED_PROP(Props, _Fade);
                mc.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
#ifdef UNITY_UI_ALPHACLIP
				clip (mc.a - 0.01);
#endif

                return mc;
            }
            ENDCG
        }
    }
}
