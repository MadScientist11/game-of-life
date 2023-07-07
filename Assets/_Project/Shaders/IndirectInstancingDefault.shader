Shader "Unlit/IndirectInstancingDefault"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #define UNITY_INDIRECT_DRAW_ARGS IndirectDrawIndexedArgs
            #include "UnityIndirect.cginc"
            

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR0;
            };

            uniform StructuredBuffer<float4x4> Transforms;
            float4 _Color;

            v2f vert(appdata_base i, uint instanceID : SV_InstanceID)
            {
                InitIndirectDrawArgs(0);
                v2f o;
                float4 wpos = mul(Transforms[instanceID], i.vertex);
                o.pos = mul(UNITY_MATRIX_VP, wpos);
                o.color = _Color;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}