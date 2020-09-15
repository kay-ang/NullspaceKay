Shader "Nullspace/Text Shader Clip" {
    Properties {
        _MainTex ("Font Texture", 2D) = "white" {}
        _Color ("Text Color", Color) = (1,1,1,1)

        _ClipRect("Clip Rect", Vector) = (1,1,1,1)
        _UseClipRect("UseClipRect", float) = 0
    }

    SubShader {

        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }
        Lighting Off Cull Off ZTest Always ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                // float2 texcoord : TEXCOORD0;
			    float4 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform fixed4 _Color;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _Color;
                // o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                o.texcoord.xy = TRANSFORM_TEX(v.texcoord.xy,_MainTex).xy;
                o.texcoord.zw = mul(unity_ObjectToWorld, v.vertex).xy;
                return o;
            }

            float4 _ClipRect;
			float _UseClipRect;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = i.color;
                col.a *= tex2D(_MainTex, i.texcoord).a;

                 // out of clip area alpha=0
                float c = UnityGet2DClipping(i.texcoord.zw, _ClipRect);
  				col.a = lerp(col.a, c * col.a, _UseClipRect);

                return col;
            }
            ENDCG
        }
    }
}
