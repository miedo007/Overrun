Shader "Mtl/UI/Overlayed Color"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _Saturation ("Saturation", Float) = 1.15

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
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
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "Includes/BlendModes.cginc"

			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
			};
			
			fixed4 _Color;
			half _Saturation;
			
			float4 _ClipRect;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
				#endif
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			
			fixed3 Saturate(fixed3 input, half saturation)
			{
			    fixed luma = dot(input, fixed3(0.2126729, 0.7151522, 0.0721750));
                return luma.xxx + saturation.xxx * (input - luma.xxx);
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = pow(tex2D(_MainTex, IN.texcoord), 0.454545);
				#if UNITY_COLORSPACE_GAMMA
				const half4 gamma = half4(2.2, 2.2, 2.2, 2.2);
				c = pow(c, gamma);
				#endif
				
				c.a *= IN.color.a * UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

				fixed4 color = IN.color;
				
				c.rgb = Overlay(c, color).rgb * c.a;
				#if UNITY_COLORSPACE_GAMMA
				c = pow(c, 1/gamma);
				#endif
				
                c.rgb = Saturate(c.rgb, _Saturation);

				#ifdef UNITY_UI_ALPHACLIP
					clip (c.a - 0.001);
				#endif

				return c;
			}
		ENDCG
        }
    }
}