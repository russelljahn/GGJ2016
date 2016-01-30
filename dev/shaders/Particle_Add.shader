Shader "ThreeKingdoms/Particle/Add" {
	Properties
	{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,1.0)
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_Alpha("Alpha",Float) = 1.0
		_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	}
	
	 Category {
		Tags { "IgnoreProjector"="True" "Queue" = "Overlay" "RenderType"="Opaque"}
		Blend One One
		FOG { Mode off}
		ColorMask RGB
		ZWrite Off ZTest Off Offset -1, -1
		Cull Off Lighting Off ZWrite Off FOG { Mode off}
		BindChannels {
	         Bind "Color", color
	         Bind "Vertex", vertex
	         Bind "TexCoord", texcoord
	     }
		
		SubShader{
			Pass{
	             CGPROGRAM
	             #pragma vertex vert
	             #pragma fragment frag
	             #pragma fragmentoption ARB_precision_hint_fastest
	             #pragma multi_compile_particles
	 
	             #include "UnityCG.cginc"
	             
	             sampler2D _MainTex;
	             fixed4 _TintColor;
		
				struct appdata_t
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};
		
				struct v2f
				{
					float4 vertex : POSITION;
					half2 texcoord : TEXCOORD0;
					fixed4 color : COLOR;
	                #ifdef SOFTPARTICLES_ON
	                float4 projPos : TEXCOORD1;
	                #endif
				};
	
				float4 _MainTex_ST;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					#ifdef SOFTPARTICLES_ON
	                o.projPos = ComputeScreenPos (o.vertex);
	                COMPUTE_EYEDEPTH(o.projPos.z);
	                #endif
	                o.color = v.color;
					o.texcoord.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex).xy;
					return o;
				}
				
				sampler2D _CameraDepthTexture;
				float _InvFade;
				
				fixed4 frag (v2f i) : COLOR
				{
					#ifdef SOFTPARTICLES_ON
	                float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
	                float partZ = i.projPos.z;
	                float fade = saturate (_InvFade * (sceneZ-partZ));
	                i.color.a *= fade;
	                #endif
					return tex2D(_MainTex, i.texcoord) * i.color * _TintColor * 2;
				}
				ENDCG
			}
		}
		// ---- Dual texture cards
	    SubShader {
	         Pass {
	             SetTexture [_MainTex] {
	                 constantColor [_TintColor]
	                 combine constant * primary
	             }
	             SetTexture [_MainTex] {
	                 combine texture * previous DOUBLE
	             }
	         }
	    }
	     
	    // ---- Single texture cards (does not do color tint)
	    SubShader {
	         Pass {
	             SetTexture [_MainTex] {
	                 combine texture * primary
	             }
	         }
	    }
	}
}