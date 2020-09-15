// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Nullspace/OverdrawReplacement"
{
	Properties
	{
		_OverdrawUnit("OverdrawUnit", Range(0.01, 0.1)) = 0.01
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			Fog { Mode Off }
			Blend One One
			ZTest Always
			ZWrite Off
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				half _OverdrawUnit;
				half4 vert(half4 vertex : POSITION) : SV_POSITION
				{
					return UnityObjectToClipPos(vertex);
				}

				fixed4 frag(half4 vertex : SV_POSITION) : COLOR
				{
					return _OverdrawUnit;
				}
			ENDCG
		}
	}

}