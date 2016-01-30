Shader "ThreeKingdoms/Water/Base" {
	Properties {
		_Speed ("Speed",Float) = 1.00
		_Color("Main Color", Color) = (0.5,0.5,0.5,1.0)
		_WaterColor ("Water Color", Color) = (0.5,0.5,0.5,1.0)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader {
        Tags { "IgnoreProjector"="True" "Queue" = "Geometry" "RenderType"="Opaque"}
		LOD 200
		Lighting Off
		//Blend one one
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
                uniform float4 _WaterColor;
                uniform sampler2D _MainTex;
                uniform float4 _MainTex_ST;
                uniform float _Speed;
                
                v2f vert (appdata_tan v)
                {
                    v2f o;
                    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                   	o.uv.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex).xy;
                   	o.uv.zw = o.uv.xy*1.1;
                   	o.uv.xy += _Time*_Speed;
                   	o.uv.zw += _Time*_Speed*0.6;
                    return o;
                }

                float4 frag(v2f i) : COLOR
                {	
                	float4 c0 = _Color*tex2D(_MainTex,i.uv.xy)*2;
                	float4 c1 = _Color*tex2D(_MainTex,i.uv.zw)*2;
                    return pow(c0*c1,2)*5 + _WaterColor;
                }
            ENDCG
        }
	} 
	FallBack "Diffuse"
}
