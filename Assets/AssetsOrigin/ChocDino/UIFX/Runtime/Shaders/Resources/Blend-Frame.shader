Shader "Hidden/ChocDino/UIFX/Blend-Frame"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[PerRendererData] _ResultTex ("Sprite Texture", 2D) = "white" {}
		[PerRendererData] _SourceTex ("Source Texture", 2D) = "white" {}
		[PerRendererData] _FillTex ("Fill Texture", 2D) = "white" {}

		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_CutoutAlpha ("Cutout Alpha", Float) = 1
		_FillSoft ("Fill Soft", Float) = 1
		_FillColor ("Fill Color", Vector) = (0.2, 0.2, 0.2, 1)
		_BorderColor ("Border Color", Vector) = (0.8, 0.8, 0.8, 1)
		_BorderSizeSoft ("Border Size,Soft", Vector) = (4.0, 1.0, 0.0, 0.0)
		
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		_ColorMask			("Color Mask", Float) = 15
	}

	CGINCLUDE

	#pragma multi_compile_local _ UNITY_UI_CLIP_RECT
	#pragma multi_compile_local _ UNITY_UI_ALPHACLIP
	#pragma multi_compile_local __ SHAPE_CIRCLE SHAPE_ROUNDRECT
	#pragma multi_compile_local __ CUTOUT
	#pragma multi_compile_local __ BORDER
	//#pragma multi_compile_local __ MASK
		
	#include "BlendUtils.cginc"
	#include "CompUtils.cginc"
	#include "ColorUtils.cginc"

	struct v2fFrame
	{
		float4 vertex : SV_POSITION;
		float4 uv : TEXCOORD0;
		float4 color : TEXCOORD1;
		#ifdef UNITY_UI_CLIP_RECT
		float4 mask : TEXCOORD2;
		#endif

		UNITY_VERTEX_OUTPUT_STEREO // VR SUPPORT
	};

	uniform float4 _Rect_ST;
	uniform float _CutoutAlpha;
	uniform float4 _FillColor;
	uniform float _FillSoft;
	uniform float4 _EdgeRounding;
	//uniform sampler2D _FillTex;
	//uniform float4 _FillTex_ST;

	uniform float4 _BorderColor;
	uniform float2 _BorderSizeSoft;

	v2fFrame vertFrame(appdata v)
	{
		v2fFrame o;

		UNITY_SETUP_INSTANCE_ID(v); // VR SUPPORT
		UNITY_INITIALIZE_OUTPUT(v2fFrame, o); // VR SUPPORT
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); // VR SUPPORT

		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv.xy = TRANSFORM_TEX(v.uv, _ResultTex);

		// Scale and offset for texture aspect ratio
		o.uv.zw = TRANSFORM_TEX(v.uv, _Rect);
		o.color = v.color;

		// 2D rect clipping
		#ifdef UNITY_UI_CLIP_RECT
		{
			float2 pixelSize = o.vertex.w;
			pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

			float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
			#ifdef MASK_SOFTNESS_OLD
			half2 maskSoftness = half2(_MaskSoftnessX, _MaskSoftnessY);
			#else
			half2 maskSoftness = half2(_UIMaskSoftnessX, _UIMaskSoftnessY);
			#endif
			o.mask = float4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * maskSoftness + abs(pixelSize.xy)));
		}
		#endif
		
		return o;
	}

	float RoundedRectEdges(in float2 p, in float2 b, in float4 r)
	{
		r.xy = (p.x<0.0)?r.xz : r.yw;
		r.x  = (p.y>0.0)?r.x  : r.y;
		float2 q = abs(p)-b+r.x;
		return min(max(q.x,q.y),0.0) + length(max(q,0.0)) - r.x;
	}

	float Rectangle(float2 p, float2 b)
	{
		float2 d = abs(p)-b;
		return length(max(d,0.0)) + min(max(d.x,d.y),0.0);
	}
	
	float4 fragFrame(v2fFrame i) : SV_Target
	{
		// Note: This is already pre-multiplied alpha
		half4 fg = tex2D(_SourceTex, i.uv.xy);

		#ifdef UNITY_COLORSPACE_GAMMA
		half4 bg = ToPremultiplied(_FillColor);
		#else
		half4 bg = StraightGammaToPremultipliedLinear(_FillColor);
		#endif

		// Note: This is already pre-multiplied alpha
		//bg = tex2D(_ResultTex, i.uv.xy);
		//bg *= ToPremultiplied(tex2D(_FillTex, i.uv.zw));

		const float2 shapeUV = (i.uv.zw - 0.5) * _ResultTex_TexelSize.zw;
		const float borderSize = _BorderSizeSoft.x;

		float fillMask = 0.0;
		float borderMask = 0.0;

		#if SHAPE_CIRCLE
			float radius = (_ResultTex_TexelSize.z - (borderSize * 2.0)) / 2.0;

			float fillEdgeDistance = length(shapeUV) - radius;
			float aaf = saturate(abs(fillEdgeDistance - 0.5)) * _FillSoft;
			fillMask = smoothstep(0.0, aaf, -fillEdgeDistance);

			#if BORDER
				float borderEdgeDistance = fillEdgeDistance - borderSize;
				aaf = saturate(abs(borderEdgeDistance - 0.5)) * _BorderSizeSoft.y;
				borderMask = smoothstep(0.0, aaf, -borderEdgeDistance);
			#endif

		#elif SHAPE_ROUNDRECT
			float2 size = (_ResultTex_TexelSize.zw - (borderSize * 2.0)) / 2.0;

			float fillEdgeDistance = RoundedRectEdges(shapeUV, size, _EdgeRounding);

			float aaf = saturate(abs(fillEdgeDistance - 0.5)) * _FillSoft;
			fillMask = smoothstep(0.0, aaf, -fillEdgeDistance);
			
			#if BORDER
				float borderEdgeDistance = fillEdgeDistance - borderSize;
				aaf = saturate(abs(borderEdgeDistance - 0.5)) * _BorderSizeSoft.y;
				borderMask = smoothstep(0.0, aaf, -borderEdgeDistance);
			#endif
		#else
			float2 size = (_ResultTex_TexelSize.zw - (borderSize * 2.0)) / 2.0;

			float fillEdgeDistance = Rectangle(shapeUV, size);
			float aaf = saturate(abs(fillEdgeDistance - 0.5)) * _FillSoft;
			fillMask = smoothstep(0.0, aaf, -fillEdgeDistance);
			
			#if BORDER
				float borderEdgeDistance = Rectangle(shapeUV, _ResultTex_TexelSize.zw * 0.5);
				aaf = saturate(abs(borderEdgeDistance - 0.5)) * _BorderSizeSoft.y;
				borderMask = smoothstep(0.0, aaf, -borderEdgeDistance);
			#endif
		#endif

		// Composite
		half4 color = AlphaComp_Over(fg, bg);
		#if (CUTOUT)
		{
			color = lerp(color, AlphaComp_Out(bg, fg), _CutoutAlpha);
		}
		#endif

		const half4 backcolor = 0.0;
		
		#if BORDER
			#ifdef UNITY_COLORSPACE_GAMMA
			half4 bordercolor = ToPremultiplied(_BorderColor);
			#else
			half4 bordercolor = StraightGammaToPremultipliedLinear(_BorderColor);
			#endif

			color = lerp(bordercolor * saturate(borderSize), color, fillMask);
			color = lerp(backcolor, color, borderMask);
		#else
			color = lerp(backcolor, color, fillMask);
		#endif


		// Mask with the background
		//#if (MASK)
		//color = ToStraight(color);
		//color.a = min(color.a, bg.a);
		//color = ToPremultiplied(color);
		//#endif

		// 2D rect clipping
		#ifdef UNITY_UI_CLIP_RECT
		color = ApplyClipRect(color, i.mask);
		#endif

		// Alpha clipping
		#ifdef UNITY_UI_ALPHACLIP
		clip (color.a - 0.001);
		#endif
	
		color.rgb *= i.color.a;
		color *= i.color;

		return color;
	}

	ENDCG

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
			"OutputsPremultipliedAlpha"="True"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend One OneMinusSrcAlpha // Premultiplied transparency
		ColorMask [_ColorMask]

		Pass
		{
			Name "Blend-Frame"
			CGPROGRAM
			#pragma vertex vertFrame
			#pragma fragment fragFrame
			ENDCG
		}
	}
}
