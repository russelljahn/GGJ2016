Shader "ThreeKingdoms/Illumin/Base" {
	Properties {
		_Color("Main Color", Color) = (0.5,0.5,0.5,1.0)
		_MainTex ("Main (RGB)", 2D) = "white" {}
		_IllColor("Ill Color", Color) = (0.5,0.5,0.5,1.0)
		_IllTex("Illumin (A)", 2D) = "white" {}
	}
	
	SubShader {
        Tags { "IgnoreProjector"="True" "Queue" = "Geometry" "RenderType"="Opaque"}
		LOD 200
		Lighting Off
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
                }; 
                uniform float4 _Color;
                uniform sampler2D _MainTex;
                uniform float4 _MainTex_ST;
                
                uniform float4 _IllColor;
                uniform sampler2D _IllTex;
                uniform float4 _IllTex_ST;
                v2f vert (appdata_tan v)
                {
                    v2f o;
                    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                   	o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex).xy;
                   	o.uv.zw = TRANSFORM_TEX(v.texcoord.xy,_IllTex).xy;
                    return o;
                }

                float4 frag(v2f i) : COLOR
                {	
                	half4 c = _Color*tex2D(_MainTex,i.uv.xy)*2;
                	half4 l = _IllColor*tex2D(_IllTex,i.uv.zw).a*2;
                    return c+l;
                }
            ENDCG
        }
	} 
	FallBack "Diffuse"
}