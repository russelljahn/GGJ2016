﻿Shader "ThreeKingdoms/Actor/Edge Rim1" {
	Properties {
		_Color("Main Color", Color) = (0.5,0.5,0.5,1.0)
		_RimColor("Rim Color", Color) = (0.0,0.0,0.0,1.0)
		_EdgeColor("Edge Color", Color) = (0,0,0,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_CubeLightTex ("Cube Light Map(RGB)",Cube) = "white"{}
		_RimRange("Rim Offset", Range(0,16)) = 4
	}
	
	SubShader {
        Tags { "IgnoreProjector"="True" "Queue" = "Geometry+10" "RenderType"="Opaque"}
		Lighting Off
		LOD 200
        
        Pass
        {   
			ZWrite Off
			ZTest Off
			Fog {Mode Off}
            CGPROGRAM
	        #pragma vertex vert	
	        #pragma fragment frag
	        #pragma fragmentoption ARB_precision_hint_fastest
	        #include "UnityCG.cginc"

	        struct v2f
	        {
	            float4  pos : SV_POSITION;
	        }; 
	        uniform float4 _EdgeColor;
	        
	        v2f vert (appdata_tan v)
	        {
	            v2f o;
	            float4 pos = v.vertex;
	            pos.xyz += v.normal*0.05;
	            o.pos = mul(UNITY_MATRIX_MVP, pos);
	            return o;
	        }

	        float4 frag(v2f i) : COLOR
	        {				
	            return _EdgeColor;
	        }
            ENDCG
        }
        
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
                uniform float4 _RimColor;
                uniform sampler2D _MainTex;
                uniform samplerCUBE _CubeLightTex;
                uniform float4 _MainTex_ST;
                uniform int _RimRange;
                
                v2f vert (appdata_tan v)
                {
                    v2f o;
                    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                    o.uv.zw = v.texcoord.xy;
                    o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex).xy;
					
					// normal/tangent/binormal
					o.cubenormal = mul (UNITY_MATRIX_MV, float4(v.normal,0));
					o.normal = mul(_Object2World, float4(v.normal.x,v.normal.y,v.normal.z,0)).xyz;
 					o.worldRefl = normalize(mul(_Object2World,v.vertex).xyz-_WorldSpaceCameraPos);
                    return o;
                }

                float4 frag(v2f i) : COLOR
                {
					half4 c = tex2D(_MainTex,i.uv.xy);
					half3 r = reflect(i.worldRefl, i.normal);
					half4 e = texCUBE (_CubeLightTex, i.cubenormal);
                    
                    half4 oColor = c*_Color*2;
                    half rim =  1.0 - saturate(dot(-i.worldRefl, i.normal));
                    rim = pow(rim*2,_RimRange);
                    oColor = oColor * (e + 0.5) + _RimColor*rim;
                    return oColor;
                }
            ENDCG
        }
	} 
	FallBack "ThreeKingdoms/Base"
}