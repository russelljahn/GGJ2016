Shader "Sense/Photosphere"
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
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
        }

        Cull Front
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf NoLighting vertex:vert keepalpha halfasview noforwardadd nolightmap

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

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            fixed4 color;
            color.rgb = s.Albedo;
            color.a = s.Alpha;
            return color;
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
