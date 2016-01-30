Shader "ThreeKingdoms/Overlay/Blend" {
	Properties {
		_Color01("Main Color", Color) = (0.5,0.5,0.5,1.0)
		_Color02("Temp Color", Color) = (0.1,0.0,0.0,1.0)
		_BackColor("Back Color", Color) = (0.1,0.0,0.0,1.0)
		_FrontTex ("Front (RGB)", 2D) = "white" {}
		_BackTex ("Back (RGB)", 2D) = "white" {}
		_MaskTex ("Mask (RGB)", 2D) = "white" {}
		_Offset01 ("Offset",Range (0.00,1.00)) = 1.00
		_Offset02 ("Offset",Range (0.00,1.00)) = 1.00
	}
	
	SubShader {
        Tags { "IgnoreProjector"="True" "Queue" = "Overlay" "RenderType"="Transparent"}
		LOD 200
		ZWrite Off
		ZTest Off
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
                }; 
                
                uniform float4 _Color01;
                uniform float4 _Color02;
                uniform float4 _BackColor;
                uniform sampler2D _FrontTex;
                uniform sampler2D _BackTex;
                uniform sampler2D _MaskTex;
                uniform float _Offset01;
                uniform float _Offset02;
                v2f vert (appdata_tan v)
                {
                    v2f o;
                    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                    o.uv.xy = v.texcoord.xy;
                    o.uv.zw = float2(v.texcoord.x + _Offset01,v.texcoord.x + _Offset02);
                    return o;
                }

                float4 frag(v2f i) : COLOR
                {	
                	half4 f = tex2D(_FrontTex,i.uv.xy);
                	half4 b1 = tex2D(_BackTex,i.uv.zy)*_Color01*2;
                	half4 b2 = tex2D(_BackTex,i.uv.wy)*_Color02*2;
                	half4 t = lerp(b2,b1,b1.a);
                	half4 c = lerp(t*t.a,f,f.a);
                	c = lerp(_BackColor,c,c.a);
                	c.a = tex2D(_MaskTex,i.uv.xy).a;
                    return c;
                }
            ENDCG
        }
	} 
	FallBack "Diffuse"
}