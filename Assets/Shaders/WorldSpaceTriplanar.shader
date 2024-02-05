Shader "GreenThings/WorldSpaceTriplanar"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Overlay Tex", 2D) = "white" {}
        _RepeatingAmount ("RepeatingAmount", float) = 1
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        #pragma vertex vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        
        struct Input
        {
            float3 worldPos;
            float4 vert;
            float3 normal;
        };

        void vert (inout appdata_full v, out Input o)
        {
            o.worldPos= mul(unity_ObjectToWorld,v.vertex).xyz;
            o.vert = v.vertex;
            o.normal = mul(unity_ObjectToWorld, float4(v.normal, 0)).xyz;
        }

        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        half _RepeatingAmount;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
    
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Taken from here
            // https://github.com/keijiro/StandardTriplanar/blob/master/Assets/Triplanar/Shaders/StandardTriplanar.shader
            
            float3 tripPlanarBlendingFactor = normalize(abs(IN.normal));
            tripPlanarBlendingFactor /= dot(tripPlanarBlendingFactor, (float3)1);

            float2 tx = IN.worldPos.yz * _RepeatingAmount;
            float2 ty = IN.worldPos.zx * _RepeatingAmount;
            float2 tz = IN.worldPos.xy * _RepeatingAmount;
            half4 cx = tex2D(_MainTex, tx) * tripPlanarBlendingFactor.x;
            half4 cy = tex2D(_MainTex, ty) * tripPlanarBlendingFactor.y;
            half4 cz = tex2D(_MainTex, tz) * tripPlanarBlendingFactor.z;
            half4 overlayColor = cx + cy + cz;
            
            fixed4 lighter = overlayColor;
            lighter = saturate(lighter - 0.5);
            fixed4 darker = saturate(1 - overlayColor - 0.5);
            
            o.Albedo = ((fixed4)1 + lighter - darker) * _Color;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = _Color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
