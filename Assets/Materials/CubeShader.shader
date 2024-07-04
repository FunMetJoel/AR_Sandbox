Shader "Custom/CubeShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Scale ("Scale", Int) = 100
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard
        #pragma target 3.0

        struct Input
        {
            float3 worldPos;
        };

    UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        int _Scale;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = fixed4(IN.worldPos.x / _Scale, IN.worldPos.y / _Scale, IN.worldPos.z / _Scale,0 );
            o.Albedo = c.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
