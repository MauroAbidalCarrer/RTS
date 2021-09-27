Shader "fog" 
{
     Properties 
     {
            _MainTex ("map", 2D) = "white" {}
            _color ("color", Color) = (.25, .5, .5, 1)
            viewedAlpha ("viewedAlpha", Range(0.0, 1.0)) = 0.5
            unseenAlpha ("unseenAlpha", Range(0.0, 1.0)) = 0.5
            sideLength ("map side length", int) = 256
            nbSamples ("nbSamples", int) = 1
     }
 
     SubShader {
         Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
     
         ZWrite Off
         ZTest Always
         Blend SrcAlpha OneMinusSrcAlpha 
     
         Pass {  
             CGPROGRAM
                 #pragma vertex vert
                 #pragma fragment frag
             
                 #include "UnityCG.cginc"
 
                 struct appdata_t {
                     float4 vertex : POSITION;
                     float2 texcoord : TEXCOORD0;
                 };
 
                 struct v2f {
                     float4 vertex : SV_POSITION;
                     half2 texcoord : TEXCOORD0;
                 };
 
                 sampler2D _MainTex;
                 float4 _MainTex_ST;
             
                 v2f vert (appdata_t v)
                 {
                     v2f o;
                     o.vertex = UnityObjectToClipPos(v.vertex);
                     o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                     return o;
                 }
             
                 float4 _color;
                 float viewedAlpha;
                 float unseenAlpha;
                 int sideLength;
                 int nbSamples;
                 float4 getColor(float2 uv)
                 {
                     fixed4 col = tex2D(_MainTex, uv);
                     if (col.b == 1)
                        return float4(0, 0, 0, 0);
                     if (col.g == 1)
                        return float4(_color.rgb, viewedAlpha);
                     return float4(_color.rgb, unseenAlpha);
                 }
                 float4 sideBlur(float2 uv)
                 {
                     float pixSize = 1.0 / (float)sideLength;
                     float divider = 1.0 + 2.0 * (float)nbSamples;
                     float4 col = 0;
                     for (int i = -nbSamples; i <= nbSamples ; i++) 
                        col += getColor(uv + float2(pixSize * (float)i, 0)) / divider;
                     return col;
                 }
                 fixed4 frag (v2f i) : SV_Target
                 {
                     float2 uv = i.texcoord;
                     float pixSize = 1.0 / (float)sideLength;
                     float divider = 1.0 + 2.0 * (float)nbSamples;
                     float4 col = 0;
                     for (int i = -nbSamples; i <= nbSamples ; i++) 
                        col += sideBlur(uv + float2(0, (float)i * pixSize)) / divider;
                     return col;
                 }
             ENDCG
         }
     }
 } 