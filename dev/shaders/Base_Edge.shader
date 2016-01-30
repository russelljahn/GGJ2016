Shader "ThreeKingdoms/Edge_Base" {
	Properties {
		_Color("Main Color", Color) = (0.5,0.5,0.5,1.0)
		_EdgeColor("Edge Color", Color) = (0.5,0.5,0.5,1.0)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader {
	
		Tags { "IgnoreProjector"="True" "Queue" = "Geometry+1" "RenderType"="Opaque"}
      	LOD 200
        
        Pass
        {   
			ZWrite Off
			ZTest Off
			Lighting Off
			
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
        	ZWrite On
			ZTest On
			Lighting Off
            
            CGPROGRAM
	        #pragma vertex vert	
	        #pragma fragment frag
	        #pragma fragmentoption ARB_precision_hint_fastest
	        #include "UnityCG.cginc"

	        struct v2f
	        {
	            float4  pos : SV_POSITION;
	            float4  uv : TEXCOORD1;
	        }; 
	        uniform float4 _Color;
	        uniform sampler2D _MainTex;
	        uniform float4 _MainTex_ST;
	        
	        v2f vert (appdata_tan v)
	        {
	            v2f o;
	            o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	           	o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex).xy;
	            return o;
	        }

	        float4 frag(v2f i) : COLOR
	        {				
	            return _Color*tex2D(_MainTex,i.uv.xy)*2;
	        }
            ENDCG
        }
	}
	FallBack "Diffuse"
}