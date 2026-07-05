Shader "Lu/lunaria_ui_image"
{
    Properties
    {
        [HideInInspector] [Enum(UnityEngine.Rendering.BlendMode)] _SrcMode ("SrcMode", int) = 5
        [Enum(Alpha Blend, 10, Additive, 1)] _DstMode ("Blend Mode", int) = 10
        [Enum(2Side, 0, Front, 2, Back, 1)] _Culling ("Show Mesh", int) = 0
        [Enum(Off, 0, On, 1)] _Zwrite ("ZWrite", int) = 0

        [Toggle(_USE_POLAR)] _UsePolar ("Use Polar Coordinate", Float) = 0

        _MainTex ("Main Texture", 2D) = "white" {}
        [KeywordEnum(None, Repeat, Clamp)] _MtexWrapMode ("Main Texture WrapMode", float) = 0
        _MtexSpdX ("   - MTex Flow Speed X", Range (-10,10) ) = 0.0
        _MtexSpdY ("   - MTex Flow Speed Y", Range (-10,10) ) = 0.0
        [HDR] _MtexC ("   - MTex Color", Color) = (1,1,1,1)
        _MtexUV ("   - MTex UV Rext", Vector) = (1,1,1,1)

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

        [Toggle(_USE_D_DS_TEX)] _UseDDsTex ("Use Dissolve Distortion", Float) = 0
        _DsDi("   - DSTex Intensity", Range (0,1) ) = 0.0
        [Toggle(_USE_DS_A_TEX)] _UseDsATex ("Use Dissolve Alpha Texture", Float) = 0
        _DsAtex ("Dissolve Alpha Texture", 2D) = "white" {}
        _DsAi("   - Dissolve Alpha Intensity", Range (0,1) ) = 0.0

        _Fade ("Fade", Range (0, 1) ) = 1.0

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
        _ClipRect ("Clip Rect", Vector) = (-32767, -32767, 32767, 32767)

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        _AClip ("Alpha Clip Range", Range (0,1) ) = 0.01
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
            #pragma shader_feature _USE_DS_A_TEX
            #pragma shader_feature _USE_D_DS_TEX
            #pragma shader_feature _USE_POLAR
            #pragma shader_feature _MTEXWRAPMODE_NONE _MTEXWRAPMODE_REPEAT _MTEXWRAPMODE_CLAMP
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

           
           

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata
            {
                float4 col : COLOR;
                float4 vtx : POSITION;
                float4 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexToFlag
            {
                float4 col : COLOR;
                float4 vtx : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 uv2 : TEXCOORD2;
                float4 worldPosition : TEXCOORD3;
                half4 mask : TEXCOORD4;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _ClipRect;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;
            float _AClip;

            sampler2D _MainTex;
            sampler2D _SubTex;
            sampler2D _Atex;
            sampler2D _Dtex;
            sampler2D _Dstex;
            sampler2D _DsAtex;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
                UNITY_DEFINE_INSTANCED_PROP(float4, _MtexC)
                UNITY_DEFINE_INSTANCED_PROP(float, _MtexSpdX)
                UNITY_DEFINE_INSTANCED_PROP(float, _MtexSpdY)

                #ifndef _MTEXWRAPMODE_NONE
                    UNITY_DEFINE_INSTANCED_PROP(float4, _MtexUV)
                #endif

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
                #ifdef _USE_DS_A_TEX
                    UNITY_DEFINE_INSTANCED_PROP(float4, _DsAtex_ST)
                    UNITY_DEFINE_INSTANCED_PROP(float, _DsAi)
                #endif

                #ifdef USE_D_DS_TEX
                    UNITY_DEFINE_INSTANCED_PROP(float, _DsDi)
                #endif

                UNITY_DEFINE_INSTANCED_PROP(float4, _Dstex_ST)
                UNITY_DEFINE_INSTANCED_PROP(float, _Dsi)
                UNITY_DEFINE_INSTANCED_PROP(float, _DsEdgeRange)
                UNITY_DEFINE_INSTANCED_PROP(float4, _DsEdgeC)
                UNITY_DEFINE_INSTANCED_PROP(float, _DsEdgeMul)
                UNITY_DEFINE_INSTANCED_PROP(float, _DstexSpdX)
                UNITY_DEFINE_INSTANCED_PROP(float, _DstexSpdY)

                UNITY_DEFINE_INSTANCED_PROP(float, _Fade)
            UNITY_INSTANCING_BUFFER_END(Props)

            #define M_TEX_ST            UNITY_ACCESS_INSTANCED_PROP(Props, _MainTex_ST)
            #define M_TEX_SPEED_X       UNITY_ACCESS_INSTANCED_PROP(Props, _MtexSpdX)
            #define M_TEX_SPEED_Y       UNITY_ACCESS_INSTANCED_PROP(Props, _MtexSpdY)
            #define SB_TEX_ST           UNITY_ACCESS_INSTANCED_PROP(Props, _SubTex_ST)
            #define SB_TEX_SPEED_X      UNITY_ACCESS_INSTANCED_PROP(Props, _SBtexSpdX)
            #define SB_TEX_SPEED_Y      UNITY_ACCESS_INSTANCED_PROP(Props, _SBtexSpdY)
            #define A_TEX_ST            UNITY_ACCESS_INSTANCED_PROP(Props, _Atex_ST)
            #define A_TEX_SPEED_X       UNITY_ACCESS_INSTANCED_PROP(Props, _AtexSpdX)
            #define A_TEX_SPEED_Y       UNITY_ACCESS_INSTANCED_PROP(Props, _AtexSpdY)
            #define D_TEX_ST            UNITY_ACCESS_INSTANCED_PROP(Props, _Dtex_ST)
            #define D_TEX_SPEED_X       UNITY_ACCESS_INSTANCED_PROP(Props, _DtexSpdX)
            #define D_TEX_SPEED_Y       UNITY_ACCESS_INSTANCED_PROP(Props, _DtexSpdY)
            #define DS_TEX_ST           UNITY_ACCESS_INSTANCED_PROP(Props, _Dstex_ST)
            #define DS_TEX_SPEED_X      UNITY_ACCESS_INSTANCED_PROP(Props, _DstexSpdX)
            #define DS_TEX_SPEED_Y      UNITY_ACCESS_INSTANCED_PROP(Props, _DstexSpdY)

            #define M_TEX_UV            uv.xy
            #define SB_TEX_UV           uv.zw
            #define D_TEX_UV            uv1.xy
            #define A_TEX_UV            uv1.zw
            #define DS_TEX_UV           uv2.xy
            #define DS_ATEX_UV          uv2.zw


            VertexToFlag vert(appdata v)
            {
                VertexToFlag o;
                o.worldPosition = v.vtx;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vtx = UnityObjectToClipPos(v.vtx);

                float2 pixelSize = o.vtx.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                float2 maskUV = (v.vtx.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);
                o.mask = half4(v.vtx.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));
                o.col = v.col;

                #if _USE_POLAR
                    o.M_TEX_UV = ((v.M_TEX_UV - 0.5) * 2);
                #else
                    o.M_TEX_UV = v.M_TEX_UV * M_TEX_ST.xy + M_TEX_ST.zw + (float2(M_TEX_SPEED_X, M_TEX_SPEED_Y) * _Time.g);
                #endif

                o.SB_TEX_UV = v.M_TEX_UV * SB_TEX_ST.xy + SB_TEX_ST.zw + (float2(SB_TEX_SPEED_X, SB_TEX_SPEED_Y) * _Time.g);
                o.A_TEX_UV = v.M_TEX_UV * A_TEX_ST.xy + A_TEX_ST.zw + (float2(A_TEX_SPEED_X, A_TEX_SPEED_Y) * _Time.g);
                o.D_TEX_UV = v.M_TEX_UV * D_TEX_ST.xy + D_TEX_ST.zw + (float2(D_TEX_SPEED_X, D_TEX_SPEED_Y) * _Time.g);
                o.DS_TEX_UV = v.M_TEX_UV * DS_TEX_ST.xy + DS_TEX_ST.zw + (float2(DS_TEX_SPEED_X, DS_TEX_SPEED_Y) * _Time.g);

                #ifdef _USE_DS_A_TEX
                    o.DS_ATEX_UV = v.M_TEX_UV * UNITY_ACCESS_INSTANCED_PROP(Props, _DsAtex_ST).xy + UNITY_ACCESS_INSTANCED_PROP(Props, _DsAtex_ST).zw;
                #else
                    o.DS_ATEX_UV = v.M_TEX_UV;
                #endif

                return o;
            }


            float4 frag(VertexToFlag i) : SV_Target
            {
                //디스토션 계산 (한번에 메인 텍스쳐와 서브 텍스쳐 모두 적용)
               float2 dTexUVWrapped = frac(i.D_TEX_UV);
               float4 dc = tex2D(_Dtex, dTexUVWrapped);
                float2 dval = float2(dc.r - 0.5, dc.g - 0.5) * UNITY_ACCESS_INSTANCED_PROP(Props, _Di); // _Di weight 설정

                #ifdef _MTEXWRAPMODE_NONE // if _MTEXWRAPMODE_NONE
                    i.M_TEX_UV += dval * UNITY_ACCESS_INSTANCED_PROP(Props, _Di);
                    i.SB_TEX_UV += (dval * UNITY_ACCESS_INSTANCED_PROP(Props, _SBDi));
                #endif
                #ifndef _MTEXWRAPMODE_NONE // else
                    i.M_TEX_UV += dval * UNITY_ACCESS_INSTANCED_PROP(Props, _Di) * (UNITY_ACCESS_INSTANCED_PROP(Props, _MtexUV).zw - UNITY_ACCESS_INSTANCED_PROP(Props, _MtexUV).xy);
                    i.SB_TEX_UV += dval * UNITY_ACCESS_INSTANCED_PROP(Props, _SBDi);
                #endif

                //극좌표
                #if _USE_POLAR
                    i.M_TEX_UV = toPolar(i.M_TEX_UV);
                    i.M_TEX_UV = float2(frac(i.uv.x), i.uv.y) * M_TEX_ST.xy + M_TEX_ST.zw + (float2(M_TEX_SPEED_X, M_TEX_SPEED_Y) * _Time.g);
                #endif

                //디졸브 디스토션
                float dsdval = 0;
                #ifdef USE_D_DS_TEX
                     float dsdi = UNITY_ACCESS_INSTANCED_PROP(Props, _DsDi);
                     float4 dsdc = tex2D(_Dtex, i.D_TEX_UV);
                     dsdval = float2((dsdc.r - 0.5) * dsdi, (dsdc.g - 0.5) * dsdi);
                #endif

                #ifdef _MTEXWRAPMODE_REPEAT // if _MTEXWRAPMODE_REPEAT
                    const float4 vectorMainTexUV = UNITY_ACCESS_INSTANCED_PROP(Props, _MtexUV);
                    float2 length = vectorMainTexUV.zw - vectorMainTexUV.xy;
                    float2 t = frac((i.M_TEX_UV - vectorMainTexUV.xy) / length) * length + vectorMainTexUV.xy;
                    i.M_TEX_UV = t;
                    
                #endif
                #ifdef _MTEXWRAPMODE_CLAMP // else if _MTEXWRAPMODE_CLAMP
                    const float4 vectorMainTexUV = UNITY_ACCESS_INSTANCED_PROP(Props, _MtexUV);
                    i.M_TEX_UV = clamp(i.M_TEX_UV, vectorMainTexUV.xy, vectorMainTexUV.zw);
                #endif

                //메인 텍스쳐
                float4 mc = tex2D(_MainTex, i.M_TEX_UV) * i.col;

                //메인 텍스쳐에 서브 텍스쳐 섞기
                float4 sbc = tex2D(_SubTex, i.SB_TEX_UV) * UNITY_ACCESS_INSTANCED_PROP(Props, _SBtexC);
                mc.rgb = lerp(mc.rgb, sbc.rgb, sbc.a * UNITY_ACCESS_INSTANCED_PROP(Props, _SBMix));

#ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(i.mask.xy)) * i.mask.zw);
                mc.a *= m.x * m.y;
#endif

                //알파 텍스쳐
                float4 ac = tex2D(_Atex, i.A_TEX_UV);
                mc.a *= lerp(mc.a, ac.r, UNITY_ACCESS_INSTANCED_PROP(Props, _Ai));

                //디졸브
                float dsClip = tex2D(_Dstex, i.DS_TEX_UV + dsdval).r;

                //디졸브 알파 텍스쳐
                #ifdef _USE_DS_A_TEX
                    float DsAc = tex2D(_DsAtex, i.DS_ATEX_UV).r;
                    dsClip = lerp(dsClip, dsClip * (DsAc * UNITY_ACCESS_INSTANCED_PROP(Props, _DsAi)), UNITY_ACCESS_INSTANCED_PROP(Props, _DsAi));
                #endif

                dsClip -= UNITY_ACCESS_INSTANCED_PROP(Props, _Dsi);
                float edgeRamp = max(0, UNITY_ACCESS_INSTANCED_PROP(Props, _DsEdgeRange) - dsClip);
                clip(dsClip);
                mc.rgb = lerp(mc.rgb, -UNITY_ACCESS_INSTANCED_PROP(Props, _DsEdgeC).rgb, min(1, edgeRamp * UNITY_ACCESS_INSTANCED_PROP(Props, _DsEdgeMul)));

                //메인 텍스쳐 컬러 최종 적용
                mc *= UNITY_ACCESS_INSTANCED_PROP(Props, _MtexC);
                mc.a *= UNITY_ACCESS_INSTANCED_PROP(Props, _Fade);

                mc.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
#ifdef UNITY_UI_ALPHACLIP
				clip (mc.a - _AClip);
#endif

                return mc;
            }
            ENDCG
        }
    }
}
