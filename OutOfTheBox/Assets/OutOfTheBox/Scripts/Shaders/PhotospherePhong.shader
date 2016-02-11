Shader "Sense/PhotospherePhong"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Geometry" 
            "IgnoreProjector"="True" 
            "RenderType"="Opaque" 
            "PreviewType"="Plane"
        }

        Cull Front
        Lighting On
        ZWrite On
        Blend One OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Custom vertex:vert keepalpha

        sampler2D _MainTex;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
            fixed4 color;
        };
        
        void vert (inout appdata_full v, out Input o)
        {      
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.color = v.color * _Color;
        }

        fixed4 LightingCustom(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
			half NdotL = dot(s.Normal, lightDir);
			half diff = NdotL * 0.5 + 0.5;
			half4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten);
			c.a = s.Alpha;
			return c;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            float2 uvs = float2(1-IN.uv_MainTex.x, IN.uv_MainTex.y);
            fixed4 c = tex2D(_MainTex, uvs) * IN.color;
            o.Albedo = c.rgb * c.a;
            o.Alpha = c.a;
        }
        ENDCG
    }

Fallback "Transparent/VertexLit"
}
