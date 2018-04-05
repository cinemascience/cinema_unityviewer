// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DistanceAlphaShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MinDistance ("Fully visible Distance", Float) = 0
		_MaxDistance ("Fully invisible Distane", Float) = 0
	}

	SubShader
	{
		Tags {"RenderType" = "Transparent" "Queue" = "Transparent"}
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _MinDistance;
			float _MaxDistance;
			float _MinAlpha;

			struct Vertex
            {
            	float2 uv_MainTex : TEXCOORD0;
                float4 vertex : POSITION;
                float4 color : COLOR0;
            };
     
            struct Fragment
            {
            	float2 uv_MainTex : TEXCOORD0;
                float4 vertex : POSITION;
                float4 color: COLOR0;
                float4 posWorld : TEXCOORD1;
            };


			Fragment vert (Vertex v)
			{
				Fragment o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv_MainTex = TRANSFORM_TEX(v.uv_MainTex, _MainTex);
				o.color = v.color;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);

				return o;

			}
			
			float4 frag (Fragment IN) : COLOR
			{
				float4 c = tex2D(_MainTex, IN.uv_MainTex);

				//distance falloff
				//float3 viewDirW = _WorldSpaceCameraPos - mul((half4x4)_Object2World, IN.vertex);
        		//float viewDist = length(viewDirW);
        		float viewDist = length(IN.posWorld.xyz - _WorldSpaceCameraPos.xyz);
        		float falloff = saturate((viewDist - _MinDistance) / (_MaxDistance - _MinDistance));
        		c.a *= (1.0f - falloff);

				return c;

				// sample the texture
				fixed4 col = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
				return col;
			}
			ENDCG
		}
	}
}
