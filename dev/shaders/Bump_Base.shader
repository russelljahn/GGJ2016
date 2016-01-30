Shader "ThreeKingdoms/Bump/Base" {
	Properties {
		_Color("Main Color", Color) = (0.5,0.5,0.5,1.0)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normal (RGB)", 2D) = "white" {}
		_CubeLightTex ("Cube Light Map(RGB)",Cube) = "white"{}
		_CubeTex("Cube Map(RGB)",Cube) = "white"{}
		_Offset("Cube Offset", Range(0,1)) = 0.5
		_SpecPower("Spec Power", Range(0,10)) = 2
		_SpecOffset("Spec Offset", Range(0,1)) = 0.5
	}
	
	SubShader {
        Tags { "IgnoreProjector"="True" "Queue" = "Geometry" "RenderType"="Opaque"}
		LOD 200
		Lighting Off
        
        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members worldRefl)
#pragma exclude_renderers d3d11 xbox360
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"

                struct v2f
                {
                    float4  pos : SV_POSITION;
                    float2  uv : TEXCOORD1;
                    float3  normal : TEXCOORD2;
                    float3  tangent : TEXCOORD3;
                    float3	binormal : TEXCOORD4;
                    float3	worldRefl : TEXCOORD5;
                }; 
                uniform float4 _Color;
                uniform sampler2D _MainTex;
               	uniform samplerCUBE _CubeLightTex;
               	uniform samplerCUBE _CubeTex;
               	uniform sampler2D _BumpMap;
               	uniform float _Offset;
               	uniform float _SpecPower;
                uniform float _SpecOffset;
                v2f vert (appdata_tan v)
                {
                    v2f o;
                    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                    o.uv = v.texcoord;
					
					// normal/tangent/binormal
					o.normal = mul(_Object2World, float4(v.normal.x,v.normal.y,v.normal.z,0)).xyz;
					o.normal = normalize(o.normal);
					o.tangent = normalize(cross(o.normal,float3(0,1,0)+0.0001));
					o.tangent = normalize(o.tangent);
					o.binormal = normalize(cross(o.normal,o.tangent));
                    
                    // env Cube
                    o.worldRefl = normalize(mul(_Object2World,v.vertex).xyz-_WorldSpaceCameraPos);
                    
                    return o;
                }

                float4 frag(v2f i) : COLOR
                {
                    half4 bump = tex2D (_BumpMap, i.uv);
                    
                    float3x3 rotation = float3x3(i.tangent,i.binormal,i.normal);
                    float3 n = normalize(mul(bump.rgb*2.0 - 1.0, rotation));
                    //
					half4 c = tex2D(_MainTex,i.uv);
					half4 l = texCUBE (_CubeLightTex, n);
					
					float3 r = reflect(i.worldRefl, n);
					half4 e = texCUBE (_CubeTex, r);
					half4 s = pow(texCUBE (_CubeLightTex, r),_SpecPower);
					
                    half4 oColor = l*c + (e*_Offset + c*s*_SpecOffset)*bump.a;
                    return oColor*_Color*2;
                }
            ENDCG
        }
	} 
	FallBack "Custom/StaticLight"
}
