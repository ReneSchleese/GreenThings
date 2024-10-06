Shader "Custom/DepthShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DepthFactor ("Depth Factor", float) = 1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Tags { "Queue" = "Transparent" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert // compile function vert as vertex shader
            #pragma fragment frag // compile function frag as fragment shader

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float invLerp(float from, float to, float value){
  return (value - from) / (to - from);
}

            float remap(float origFrom, float origTo, float targetFrom, float targetTo, float value){
  float rel = invLerp(origFrom, origTo, value);
  return lerp(targetFrom, targetTo, rel);
}

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float _DepthFactor;

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = tex2D(_CameraDepthTexture, i.uv).r;
                depth = LinearEyeDepth(depth);
                //depth = depth * _ProjectionParams.z;
                depth = remap(418, 450, 0, 1, depth);
                return depth;
            }
            ENDCG
        }
    }
}