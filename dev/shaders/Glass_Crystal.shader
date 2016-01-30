Shader "ThreeKingdoms/Glass/Crystal" {
	Properties {
		_Color("Main Color", Color) = (0.5,0.5,0.5,1.0)
		_CubeColor("Cube Color", Color) = (0.5,0.5,0.5,1.0)
		_CubeLightTex ("Cube Light Map(RGB)",Cube) = "white"{}
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_RimRange("Cube Offset", Range(0,16)) = 4
	}
	
	SubShader {
        Tags { "IgnoreProjector"="True" "Queue" = "Geometry" "RenderType"="Opaque"}
		LOD 200
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha
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
                    float3  cubenormal : TEXCOORD3;
                    float3	worldRefl : TEXCOORD5;
                }; 
                uniform float4 _Color;
                uniform float4 _CubeColor;
                uniform samplerCUBE _CubeLightTex;
                uniform sampler2D _MainTex;
                uniform float4 _MainTex_ST;
                uniform int _RimRange;
                
                v2f vert (appdata_tan v)
                {
                    v2f o;
                    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                    o.uv.zw = v.texcoord.xy;
                    o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex).xy;
					
					// normal/tangent/binormal
					// o.cubenormal = mul (UNITY_MATRIX_MV, float4(v.normal,0));
					float3 temp01 = v.vertex.xyz - mul(_World2Object , float4(_WorldSpaceCameraPos,1.0)).xyz * unity_Scale.w;
					float3 temp02 = temp01 - (2.0 * (dot (v.normal, temp01) * v.normal));
					o.cubenormal = mul (_Object2World,float4(temp02,0)).xyz;
					
					o.normal = mul(_Object2World, float4(v.normal.x,v.normal.z,v.normal.y,0)).xyz;
 					o.worldRefl = normalize(mul(_Object2World,v.vertex).xyz-_WorldSpaceCameraPos);
                    return o;
                }

                float4 frag(v2f i) : COLOR
                {
                    float3 n = i.normal;
                    float3 cuben = i.cubenormal;
					
					float3 r = reflect(i.worldRefl, n);
					half rim =  1.0 - saturate(dot(-i.worldRefl, i.normal));
                    rim = pow(rim*2,_RimRange);
					
					half4 e = texCUBE (_CubeLightTex, cuben) *_CubeColor*2;
                    half4 c = tex2D(_MainTex,i.uv.xy) *_Color*2;
                    
                    return c + e;
                }
            ENDCG
        }
	} 
	FallBack "Diffuse"
}
