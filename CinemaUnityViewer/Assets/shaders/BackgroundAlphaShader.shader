// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BackgroundAlphaShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_AlphaColor("Alpha Color", Color) = (0.0,0.0,0.0,1)
		_AlphaThreshold("Alpha Threshold", Float) = 0.0
		_AlphaSoftness("Alpha Softness", Float) = 0 
	}

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha 

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
  
            sampler2D _MainTex;
            float4 _AlphaColor;
			float _AlphaThreshold;
			float _AlphaSoftness;
 
            struct Vertex
            {
                float4 vertex : POSITION;
                float2 uv_MainTex : TEXCOORD0;
                float4 color : COLOR0;
            };
     
            struct Fragment
            {
            	float2 uv_MainTex : TEXCOORD0;
                float4 vertex : POSITION;
                float4 color: COLOR0;
            };
  
            Fragment vert(Vertex v)
            {
                Fragment o;
     
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv_MainTex = v.uv_MainTex;
                o.color = v.color;
     
                return o;
            }
                                                     
            float4 frag(Fragment IN) : COLOR
            {
                float4 c = tex2D(_MainTex, IN.uv_MainTex);

                float4 alp = _AlphaColor;
                float diff = (abs(alp.r-c.r)+abs(alp.g-c.g)+abs(alp.b-c.b));
                if (diff <= _AlphaThreshold) {
                	c.a = 0.0f;
                }
                else if (diff < _AlphaThreshold + _AlphaSoftness) {
                	c.a = (diff - _AlphaThreshold) / _AlphaSoftness;
                }
                return c;
            }
 
            ENDCG
        }
    }
 }
