Shader "Custom/AlienEnergy"
{
    Properties
    {
        _MainColor ("Base Color", Color) = (0, 0.6, 1, 1)
        _GlowStrength ("Glow Strength", Float) = 2
        _FlowSpeed ("Vertical Flow Speed", Float) = 2
        _NoiseScale ("Noise Scale", Float) = 5
        _Brightness ("Brightness", Range(0,100)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha One  
        Cull Off 
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f 
            {
                float4 pos : SV_POSITION;
                float3 world : TEXCOORD0;
            };

            float4 _MainColor;
            float _GlowStrength;
            float _FlowSpeed;
            float _NoiseScale;
            float _Brightness;

            // 噪声
            float hash(float3 p)
            {
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }

            float noise(float3 p)
            {
                return hash(floor(p));
            }

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 world = i.world;

                // 上下流动能量
                float flow = sin(world.y * 5 + _Time.y * _FlowSpeed);
                flow = saturate(flow);

                // 能量噪声亮度
                float n = noise(world * _NoiseScale);

                float glow = (flow + n) * _GlowStrength;

                return float4(_MainColor.rgb * glow, glow);
            }
            ENDCG
        }
    }
}