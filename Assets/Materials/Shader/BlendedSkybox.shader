Shader "Custom/BlendedSkybox"
{
    Properties
    {
        _Skybox1("Skybox Day", Cube) = "" {}
        _Skybox2("Skybox Night", Cube) = "" {}
        _Blend("Blend", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Background" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            samplerCUBE _Skybox1;
            samplerCUBE _Skybox2;
            float _Blend;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            v2f vert (float3 pos : POSITION)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(pos);
                o.dir = pos;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col1 = texCUBE(_Skybox1, i.dir);
                half4 col2 = texCUBE(_Skybox2, i.dir);
                return lerp(col1, col2, _Blend);
            }
            ENDCG
        }
    }
}
