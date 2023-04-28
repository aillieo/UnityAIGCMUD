Shader "AillieoUtils/ImageBlur" {
    Properties {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _BlurSize("Blur Size", Range(0.0, 1.0)) = 0.5
        _BlurStep("Blur Step", Range(0, 10)) = 5

        // for UI.Mask
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
    }

    SubShader {
        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        // for UI.Mask
        Stencil {
             Ref[_Stencil]
             Comp[_StencilComp]
             Pass[_StencilOp]
             ReadMask[_StencilReadMask]
             WriteMask[_StencilWriteMask]
        }

        ColorMask[_ColorMask]

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float4 color    : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            float _BlurSize;
            float _BlurStep;
            float4 _MainTex_TexelSize;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
#ifdef UNITY_HALF_TEXEL_OFFSET
                o.vertex.xy -= (_ScreenParams.zw - 1.0);
#endif
                o.color = v.color * _Color;

                return o;
            }

            float4 frag(v2f i) : SV_Target {
                float4 color = tex2D(_MainTex, i.uv) * i.color;
                float4 blur = float4(0.0, 0.0, 0.0, 0.0);
                float weight = 0.0;

                int step = (int)_BlurStep;
                for (int x = -step; x <= step; x++) {
                    for (int y = -step; y <= step; y++) {
                        float2 offset = float2(x, y) * _BlurSize / _MainTex_TexelSize.z;
                        float4 c = tex2D(_MainTex, i.uv + offset);
                        blur += c;
                        weight ++;
                    }
                }

                float4 finalColor = lerp(color, blur / weight, _BlurSize) * i.color;

                return finalColor;
            }
            ENDCG
        }
    }
}
