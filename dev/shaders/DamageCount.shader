Shader "ThreeKingdoms/DamageCount" {
	Properties
	{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1.0)
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_Alpha("Alpha", Float) = 0.5
	}
	
	SubShader
	{
		LOD 100

//		Tags
//		{
//			"Queue" = "Transparent"
//			"IgnoreProjector" = "True"
//			"RenderType" = "Transparent"
//		}

		Tags { "IgnoreProjector"="True" "Queue" = "Overlay" "RenderType"="Opaque"}
		
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
				
			#include "UnityCG.cginc"
	
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _Alpha;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex).xy;
				return o;
			}
				
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 col = tex2D(_MainTex, i.texcoord) * _Color;
				col.a*=_Alpha;
				return col;
			}
			ENDCG
		}
	}
}