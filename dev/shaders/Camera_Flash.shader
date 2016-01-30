Shader "ThreeKingdoms/Sky/Camera_Flash" {
	Properties {
		_Color("Main Color", Color) = (0.5,0.5,0.5,1.0)
	}
	
	SubShader {
        Tags { "IgnoreProjector"="True" "Queue" = "Overlay+10" "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off ZTest Off
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
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
                }; 
                uniform float4 _Color;
                
                v2f vert (appdata_tan v)
                {
                    v2f o;
                    o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                    return o;
                }

                float4 frag(v2f i) : COLOR
                {				
                    return _Color*2;
                }
            ENDCG
        }
	} 
	FallBack "Diffuse"
}
