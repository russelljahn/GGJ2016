Shader "ThreeKingdoms/Metal/Bump" {
	Properties {
		_Color("Main Color", Color) = (0.5,0.5,0.5,1.0)
		_CubeColor("Cube Color", Color) = (0.5,0.5,0.5,1.0)
		_CubeLightTex ("Cube Light Map(RGB)",Cube) = "white"{}
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normal (RGB)", 2D) = "white" {}
	}
	
	SubShader {
 		Tags { "QUEUE"="AlphaTest" "IGNOREPROJECTOR"="true" "RenderType"="TransparentCutout" }
		LOD 200
		Lighting Off
		AlphaToMask On
		//Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members worldRefl)
//#pragma exclude_renderers d3d11 xbox360
                #pragma vertex vert	
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"

                struct v2f
                {
                    float4  pos : SV_POSITION;
                    float4  uv : TEXCOORD1;
                    float3  normal : TEXCOORD2;
                    float3  tangent : TEXCOORD3;
                    float3	binormal : TEXCOORD4;
                    float3  cubenormal : TEXCOORD5;
                }; 
                uniform float4 _Color;
                uniform float4 _CubeColor;
                uniform samplerCUBE _CubeLightTex;
                uniform sampler2D _MainTex;
                uniform sampler2D _BumpMap;
                uniform float4 _MainTex_ST;
                uniform int _RimRange;
                
                v2f vert (appdata_tan v)
                {
                    v2f o;
                    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                    o.uv.zw = v.texcoord.xy;
                    o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex).xy;
					
					// normal/tangent/binormal
					//o.cubenormal = mul (UNITY_MATRIX_MV, float4(v.normal,0));
					vec3 temp01 = v.vertex.xyz - mul(_World2Object , float4(_WorldSpaceCameraPos,1.0)).xyz * unity_Scale.w;
					vec3 temp02 = temp01 - (2.0 * (dot (v.normal, temp01) * v.normal));
					o.cubenormal = mul (_Object2World,vec4(temp02,0)).xyz;
 					
 					// normal/tangent/binormal
					o.normal = mul(UNITY_MATRIX_MV, float4(v.normal.x,v.normal.y,v.normal.z,0)).xyz;
					o.normal = normalize(o.normal);
					o.tangent = normalize(cross(o.normal,float3(0,1,0)+0.0001));
					o.tangent = normalize(o.tangent);
					o.binormal = normalize(cross(o.normal,o.tangent));
                    
                    //o.worldRefl = normalize(mul(_Object2World,v.vertex).xyz-_WorldSpaceCameraPos);
                    
                    return o;
                }

                float4 frag(v2f i) : COLOR
                {
                 	half4 bump = tex2D (_BumpMap, i.uv.xy);
                    
                    float3x3 rotation = float3x3(i.tangent,i.binormal,i.normal);
                    float3 n = normalize(mul(bump.rgb*2.0 - 1.0, rotation));

                    float3 cuben = i.cubenormal;
					
					half4 e = texCUBE (_CubeLightTex, cuben)*_CubeColor*2;
                    
                    half4 c = tex2D(_MainTex,i.uv.xy)*_Color*2;
                    
                    half4 oColor = c + e;
                    return oColor;
                }
            ENDCG
        }
	} 
	FallBack "Diffuse"
}
