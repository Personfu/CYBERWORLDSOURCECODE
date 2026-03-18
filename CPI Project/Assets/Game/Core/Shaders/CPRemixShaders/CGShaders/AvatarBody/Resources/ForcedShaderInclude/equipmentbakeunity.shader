Shader "CpRemix/Equipment Bake"
{
	Properties
	{
	  _Diffuse("Diffuse", 2D) = "black" {}
	  _Decal123OpacityTex("Decals 123 Opacity", 2D) = "black" {}
	  _Decal1Tex("Decal 1 Texture", 2D) = "white" {}
	  _Decal1Color("Decal 1 Color", Color) = (0.26,0.78,1,1)
	  _Decal1Scale("Decal 1 Scale", Range(0.1, 30)) = 1
	  _Decal1UOffset("Decal 1 uOffset", Range(-0.5, 0.5)) = 0
	  _Decal1VOffset("Decal 1 vOffset", Range(-0.5, 0.5)) = 0
	  _Decal1RotationRads("Decal 1 Rotation Rads", Range(-3.141, 3.141)) = 0
	  [MaterialToggle] _Decal1Repeat("Repeat Decal 1", float) = 0
	  _Decal2Tex("Decal 2 Texture", 2D) = "white" {}
	  _Decal2Color("Decal 2 Color", Color) = (0.06,0.55,1,1)
	  _Decal2Scale("Decal 2 Scale", Range(0.1, 30)) = 1
	  _Decal2UOffset("Decal 2 uOffset", Range(-0.5, 0.5)) = 0
	  _Decal2VOffset("Decal 2 vOffset", Range(-0.5, 0.5)) = 0
	  _Decal2RotationRads("Decal 2 Rotation Rads", Range(-3.141, 3.141)) = 0
	  [MaterialToggle] _Decal2Repeat("Repeat Decal 2", float) = 0
	  _Decal3Tex("Decal 3 Texture", 2D) = "white" {}
	  _Decal3Color("Decal 3 Color", Color) = (0.01,0.33,0.95,1)
	  _Decal3Scale("Decal 3 Scale", Range(0.1, 30)) = 1
	  _Decal3UOffset("Decal 3 uOffset", Range(-0.5, 0.5)) = 0
	  _Decal3VOffset("Decal 3 vOffset", Range(-0.5, 0.5)) = 0
	  _Decal3RotationRads("Decal 3 Rotation Rads", Range(-3.141, 3.141)) = 0
	  [MaterialToggle] _Decal3Repeat("Repeat Decal 3", float) = 0
	  _Decal4Tex("Decal 4 Texture", 2D) = "black" {}
	  _Decal4Color("Decal 4 Color", Color) = (1,1,1,1)
	  _Decal4Scale("Decal 4 Scale", Range(0.1, 30)) = 1
	  _Decal4UOffset("Decal 4 uOffset", Range(-0.5, 0.5)) = 0
	  _Decal4VOffset("Decal 4 vOffset", Range(-0.5, 0.5)) = 0
	  _Decal4RotationRads("Decal 4 Rotation Rads", Range(-3.141, 3.141)) = 0
	  [MaterialToggle] _Decal4Repeat("Repeat Decal 4", float) = 0
	  _Decal5Tex("Decal 5 Texture", 2D) = "black" {}
	  _Decal5Color("Decal 5 Color", Color) = (1,1,1,1)
	  _Decal5Scale("Decal 5 Scale", Range(0.1, 30)) = 1
	  _Decal5UOffset("Decal 5 uOffset", Range(-0.5, 0.5)) = 0
	  _Decal5VOffset("Decal 5 vOffset", Range(-0.5, 0.5)) = 0
	  _Decal5RotationRads("Decal 5 Rotation Rads", Range(-3.141, 3.141)) = 0
	  [MaterialToggle] _Decal5Repeat("Repeat Decal 5", float) = 0
	  _Decal6Tex("Decal 6 Texture", 2D) = "black" {}
	  _Decal6Color("Decal 6 Color", Color) = (1,1,1,1)
	  _Decal6Scale("Decal 6 Scale", Range(0.1, 30)) = 1
	  _Decal6UOffset("Decal 6 uOffset", Range(-0.5, 0.5)) = 0
	  _Decal6VOffset("Decal 6 vOffset", Range(-0.5, 0.5)) = 0
	  _Decal6RotationRads("Decal 6 Rotation Rads", Range(-3.141, 3.141)) = 0
	  [MaterialToggle] _Decal6Repeat("Repeat Decal 6", float) = 0
	  _BodyColorsMaskTex("Body Color Mask", 2D) = "black" {}
	  _BodyRedChannelColor("Body Red Channel Color", Color) = (1,0,0,1)
	  _BodyGreenChannelColor("Body Green Channel Color", Color) = (1,1,0,1)
	  _BodyBlueChannelColor("Body Blue Channel Color", Color) = (1,0,1,1)
	  _EmissiveColorTint("EmissiveColorTint", Color) = (1,1,1,1)
	  _DetailAndMatcapMaskAndEmissive("r=detail g=matcap b=emissive", 2D) = "black" {}
	  _AtlasOffsetU("AtlasOffset U", float) = 0
	  _AtlasOffsetV("AtlasOffset U", float) = 0
	  _AtlasOffsetScaleU("AtlasOffset U Scale", float) = 1
	  _AtlasOffsetScaleV("AtlasOffset V Scale", float) = 1
	}
		SubShader
	  {
		Tags
		{
		}
		Pass // ind: 1, name: 
		{
		  Tags
		  {
		  }
		  ZTest Always
		  ZWrite Off
		  Cull Off
		  Blend One One
		  // m_ProgramMask = 6
		  CGPROGRAM
		  //#pragma target 4.0

		  #pragma vertex vert
		  #pragma fragment frag

		  #include "UnityCG.cginc"

		  //uniform  float4x4 unity_ObjectToWorld;
		  //uniform  float4x4 unity_MatrixVP;
		  uniform  float _Decal1Scale;
		  uniform  float _Decal1UOffset;
		  uniform  float _Decal1VOffset;
		  uniform  float _Decal1RotationRads;
		  uniform  float _Decal2Scale;
		  uniform  float _Decal2UOffset;
		  uniform  float _Decal2VOffset;
		  uniform  float _Decal2RotationRads;
		  uniform  float _Decal3Scale;
		  uniform  float _Decal3UOffset;
		  uniform  float _Decal3VOffset;
		  uniform  float _Decal3RotationRads;
		  uniform  float _Decal4Scale;
		  uniform  float _Decal4UOffset;
		  uniform  float _Decal4VOffset;
		  uniform  float _Decal4RotationRads;
		  uniform  float _Decal5Scale;
		  uniform  float _Decal5UOffset;
		  uniform  float _Decal5VOffset;
		  uniform  float _Decal5RotationRads;
		  uniform  float _Decal6Scale;
		  uniform  float _Decal6UOffset;
		  uniform  float _Decal6VOffset;
		  uniform  float _Decal6RotationRads;
		  uniform  float _AtlasOffsetU;
		  uniform  float _AtlasOffsetV;
		  uniform  float _AtlasOffsetScaleU;
		  uniform  float _AtlasOffsetScaleV;
		  uniform sampler2D _Diffuse;
		  uniform sampler2D _Decal123OpacityTex;
		  uniform sampler2D _Decal1Tex;
		  uniform  float3 _Decal1Color;
		  uniform  float _Decal1Repeat;
		  uniform sampler2D _Decal2Tex;
		  uniform  float3 _Decal2Color;
		  uniform  float _Decal2Repeat;
		  uniform sampler2D _Decal3Tex;
		  uniform  float3 _Decal3Color;
		  uniform  float _Decal3Repeat;
		  uniform sampler2D _Decal4Tex;
		  uniform  float3 _Decal4Color;
		  uniform  float _Decal4Repeat;
		  uniform sampler2D _Decal5Tex;
		  uniform  float3 _Decal5Color;
		  uniform  float _Decal5Repeat;
		  uniform sampler2D _Decal6Tex;
		  uniform  float3 _Decal6Color;
		  uniform  float _Decal6Repeat;

		  struct appdata_t
		  {
			float4 _glesVertex :POSITION;
			float4 _glesMultiTexCoord0 :TEXCOORD0;
		  };

		  struct OUT_Data_Vert
		  {
			float2 xlv_TEXCOORD0 :TEXCOORD0;
			float2 xlv_TEXCOORD1 :TEXCOORD1;
			float2 xlv_TEXCOORD2 :TEXCOORD2;
			float2 xlv_TEXCOORD3 :TEXCOORD3;
			float2 xlv_TEXCOORD4 :TEXCOORD4;
			float2 xlv_TEXCOORD5 :TEXCOORD5;
			float2 xlv_TEXCOORD6 :TEXCOORD6;
			float4 gl_Position :SV_POSITION;
		  };

		  struct v2f
		  {
			float2 xlv_TEXCOORD0 :TEXCOORD0;
			float2 xlv_TEXCOORD1 :TEXCOORD1;
			float2 xlv_TEXCOORD2 :TEXCOORD2;
			float2 xlv_TEXCOORD3 :TEXCOORD3;
			float2 xlv_TEXCOORD4 :TEXCOORD4;
			float2 xlv_TEXCOORD5 :TEXCOORD5;
			float2 xlv_TEXCOORD6 :TEXCOORD6;
		  };

		  struct OUT_Data_Frag
		  {
			float4 gl_FragData :SV_Target0;
		  };

		  OUT_Data_Vert vert(appdata_t v)
		  {
			OUT_Data_Vert o;
			 float2 texUV_1;
			texUV_1.x = ((v._glesMultiTexCoord0.x - _AtlasOffsetU) / _AtlasOffsetScaleU);
			texUV_1.y = ((v._glesMultiTexCoord0.y - _AtlasOffsetV) / _AtlasOffsetScaleV);
			 float2 decal6RotatedUVs_2;
			 float2 decal5RotatedUVs_3;
			 float2 decal4RotatedUVs_4;
			 float2 decal3RotatedUVs_5;
			 float2 decal2RotatedUVs_6;
			 float2 decal1RotatedUVs_7;
			 float2 pointLocalCenterToOrigin_12;
			pointLocalCenterToOrigin_12.x = (-0.5 + _Decal1UOffset);
			pointLocalCenterToOrigin_12.y = (-0.5 + _Decal1VOffset);
			 float tmpvar_14;
			tmpvar_14 = sin(_Decal1RotationRads);
			 float tmpvar_15;
			tmpvar_15 = cos(_Decal1RotationRads);
			 float2x2 tmpvar_16;
			tmpvar_16[0].x = tmpvar_15;
			tmpvar_16[0].y = tmpvar_14;
			tmpvar_16[1].x = -(tmpvar_14);
			tmpvar_16[1].y = tmpvar_15;
			decal1RotatedUVs_7 = (mul((texUV_1 + pointLocalCenterToOrigin_12), tmpvar_16) - pointLocalCenterToOrigin_12);
			 float2 pointLocalCenterToOrigin_21;
			pointLocalCenterToOrigin_21.x = (-0.5 + _Decal2UOffset);
			pointLocalCenterToOrigin_21.y = (-0.5 + _Decal2VOffset);
			 float tmpvar_23;
			tmpvar_23 = sin(_Decal2RotationRads);
			 float tmpvar_24;
			tmpvar_24 = cos(_Decal2RotationRads);
			 float2x2 tmpvar_25;
			tmpvar_25[0].x = tmpvar_24;
			tmpvar_25[0].y = tmpvar_23;
			tmpvar_25[1].x = -(tmpvar_23);
			tmpvar_25[1].y = tmpvar_24;
			decal2RotatedUVs_6 = (mul((texUV_1 + pointLocalCenterToOrigin_21), tmpvar_25) - pointLocalCenterToOrigin_21);
			 float2 pointLocalCenterToOrigin_30;
			pointLocalCenterToOrigin_30.x = (-0.5 + _Decal3UOffset);
			pointLocalCenterToOrigin_30.y = (-0.5 + _Decal3VOffset);
			 float tmpvar_32;
			tmpvar_32 = sin(_Decal3RotationRads);
			 float tmpvar_33;
			tmpvar_33 = cos(_Decal3RotationRads);
			 float2x2 tmpvar_34;
			tmpvar_34[0].x = tmpvar_33;
			tmpvar_34[0].y = tmpvar_32;
			tmpvar_34[1].x = -(tmpvar_32);
			tmpvar_34[1].y = tmpvar_33;
			decal3RotatedUVs_5 = (mul((texUV_1 + pointLocalCenterToOrigin_30), tmpvar_34) - pointLocalCenterToOrigin_30);
			 float2 pointLocalCenterToOrigin_39;
			pointLocalCenterToOrigin_39.x = (-0.5 + _Decal4UOffset);
			pointLocalCenterToOrigin_39.y = (-0.5 + _Decal4VOffset);
			 float tmpvar_41;
			tmpvar_41 = sin(_Decal4RotationRads);
			 float tmpvar_42;
			tmpvar_42 = cos(_Decal4RotationRads);
			 float2x2 tmpvar_43;
			tmpvar_43[0].x = tmpvar_42;
			tmpvar_43[0].y = tmpvar_41;
			tmpvar_43[1].x = -(tmpvar_41);
			tmpvar_43[1].y = tmpvar_42;
			decal4RotatedUVs_4 = (mul((texUV_1 + pointLocalCenterToOrigin_39), tmpvar_43) - pointLocalCenterToOrigin_39);
			 float2 pointLocalCenterToOrigin_48;
			pointLocalCenterToOrigin_48.x = (-0.5 + _Decal5UOffset);
			pointLocalCenterToOrigin_48.y = (-0.5 + _Decal5VOffset);
			 float tmpvar_50;
			tmpvar_50 = sin(_Decal5RotationRads);
			 float tmpvar_51;
			tmpvar_51 = cos(_Decal5RotationRads);
			 float2x2 tmpvar_52;
			tmpvar_52[0].x = tmpvar_51;
			tmpvar_52[0].y = tmpvar_50;
			tmpvar_52[1].x = -(tmpvar_50);
			tmpvar_52[1].y = tmpvar_51;
			decal5RotatedUVs_3 = (mul((texUV_1 + pointLocalCenterToOrigin_48), tmpvar_52) - pointLocalCenterToOrigin_48);
			 float2 pointLocalCenterToOrigin_57;
			pointLocalCenterToOrigin_57.x = (-0.5 + _Decal6UOffset);
			pointLocalCenterToOrigin_57.y = (-0.5 + _Decal6VOffset);
			 float tmpvar_59;
			tmpvar_59 = sin(_Decal6RotationRads);
			 float tmpvar_60;
			tmpvar_60 = cos(_Decal6RotationRads);
			 float2x2 tmpvar_61;
			tmpvar_61[0].x = tmpvar_60;
			tmpvar_61[0].y = tmpvar_59;
			tmpvar_61[1].x = -(tmpvar_59);
			tmpvar_61[1].y = tmpvar_60;
			decal6RotatedUVs_2 = (mul((texUV_1 + pointLocalCenterToOrigin_57), tmpvar_61) - pointLocalCenterToOrigin_57);
			o.gl_Position = mul(unity_MatrixVP,  mul(unity_ObjectToWorld, float4(v._glesVertex.xyz, 1.0)));
			o.xlv_TEXCOORD0 = texUV_1;
			o.xlv_TEXCOORD1 = ((( (decal1RotatedUVs_7 + float2(_Decal1UOffset, _Decal1VOffset)) - float2(0.5, 0.5)) * _Decal1Scale) + float2(0.5, 0.5));
			o.xlv_TEXCOORD2 = ((( (decal2RotatedUVs_6 + float2(_Decal2UOffset, _Decal2VOffset)) - float2(0.5, 0.5)) * _Decal2Scale) + float2(0.5, 0.5));
			o.xlv_TEXCOORD3 = ((( (decal3RotatedUVs_5 + float2(_Decal3UOffset, _Decal3VOffset)) - float2(0.5, 0.5)) * _Decal3Scale) + float2(0.5, 0.5));
			o.xlv_TEXCOORD4 = ((( (decal4RotatedUVs_4 + float2(_Decal4UOffset, _Decal4VOffset)) - float2(0.5, 0.5)) * _Decal4Scale) + float2(0.5, 0.5));
			o.xlv_TEXCOORD5 = ((( (decal5RotatedUVs_3 + float2(_Decal5UOffset, _Decal5VOffset)) - float2(0.5, 0.5)) * _Decal5Scale) + float2(0.5, 0.5));
			o.xlv_TEXCOORD6 = ((( (decal6RotatedUVs_2 + float2(_Decal6UOffset, _Decal6VOffset)) - float2(0.5, 0.5)) * _Decal6Scale) + float2(0.5, 0.5));
			 return o;
		  }

		  OUT_Data_Frag frag(v2f f)
		  {
			OUT_Data_Frag o;
			 float2 uv_2;
			uv_2 = f.xlv_TEXCOORD0;
			 float2 tmpvar_3;
			tmpvar_3 = abs(((uv_2 - 0.5) * 2.0));
			 float tmpvar_4;
			tmpvar_4 = float((1.0 >= max(tmpvar_3.x, tmpvar_3.y)));
			 float3 decalOpacitySample_5;
			 float3 diffuseSample_6;
			diffuseSample_6 = tex2D(_Diffuse, f.xlv_TEXCOORD0).xyz;
			decalOpacitySample_5 = tex2D(_Decal123OpacityTex, f.xlv_TEXCOORD0).xyz;
			 float2 tmpvar_10;
			tmpvar_10 = abs(((f.xlv_TEXCOORD3 - 0.5) * 2.0));
			 float4 tmpvar_11;
			tmpvar_11 = (tex2D(_Decal3Tex, f.xlv_TEXCOORD3) * float(( (1.0 + (255.0 * _Decal3Repeat)) >= max(tmpvar_10.x, tmpvar_10.y))));
			 float tmpvar_12;
			tmpvar_12 = (tmpvar_11.w * decalOpacitySample_5.z);
			 float2 tmpvar_14;
			tmpvar_14 = abs(((f.xlv_TEXCOORD2 - 0.5) * 2.0));
			 float4 tmpvar_15;
			tmpvar_15 = (tex2D(_Decal2Tex, f.xlv_TEXCOORD2) * float(( (1.0 + (255.0 * _Decal2Repeat)) >= max(tmpvar_14.x, tmpvar_14.y))));
			 float tmpvar_16;
			tmpvar_16 = (tmpvar_15.w * decalOpacitySample_5.y);
			 float2 tmpvar_18;
			tmpvar_18 = abs(((f.xlv_TEXCOORD1 - 0.5) * 2.0));
			 float4 tmpvar_19;
			tmpvar_19 = (tex2D(_Decal1Tex, f.xlv_TEXCOORD1) * float(( (1.0 + (255.0 * _Decal1Repeat)) >= max(tmpvar_18.x, tmpvar_18.y))));
			 float tmpvar_20;
			tmpvar_20 = (tmpvar_19.w * decalOpacitySample_5.x);
			 float2 tmpvar_22;
			tmpvar_22 = abs(((f.xlv_TEXCOORD6 - 0.5) * 2.0));
			 float4 tmpvar_23;
			tmpvar_23 = (tex2D(_Decal6Tex, f.xlv_TEXCOORD6) * float(( (1.0 + (255.0 * _Decal6Repeat)) >= max(tmpvar_22.x, tmpvar_22.y))));
			 float tmpvar_24;
			tmpvar_24 = (tmpvar_23.w * decalOpacitySample_5.z);
			 float2 tmpvar_26;
			tmpvar_26 = abs(((f.xlv_TEXCOORD5 - 0.5) * 2.0));
			 float4 tmpvar_27;
			tmpvar_27 = (tex2D(_Decal5Tex, f.xlv_TEXCOORD5) * float(( (1.0 + (255.0 * _Decal5Repeat)) >= max(tmpvar_26.x, tmpvar_26.y))));
			 float tmpvar_28;
			tmpvar_28 = ((tmpvar_27.w * decalOpacitySample_5.y) * (1.0 - tmpvar_24));
			 float2 tmpvar_30;
			tmpvar_30 = abs(((f.xlv_TEXCOORD4 - 0.5) * 2.0));
			 float4 tmpvar_31;
			tmpvar_31 = (tex2D(_Decal4Tex, f.xlv_TEXCOORD4) * float(( (1.0 + (255.0 * _Decal4Repeat)) >= max(tmpvar_30.x, tmpvar_30.y))));
			 float tmpvar_32;
			tmpvar_32 = (((tmpvar_31.w * decalOpacitySample_5.x) * (1.0 - tmpvar_28)) * (1.0 - tmpvar_24));
			 float tmpvar_33;
			tmpvar_33 = min(1.0, ((tmpvar_24 + tmpvar_28) + tmpvar_32));
			 float tmpvar_34;
			tmpvar_34 = min(1.0, (min(1.0, ((tmpvar_12 + tmpvar_16) + tmpvar_20)) + tmpvar_33));
			 float4 tmpvar_35;
			tmpvar_35.xyz = ((( (((tmpvar_31.xyz * _Decal4Color) * tmpvar_32) + ((tmpvar_27.xyz * _Decal5Color) * tmpvar_28)) + ((tmpvar_23.xyz * _Decal6Color) * tmpvar_24)) * tmpvar_33) + (( (((tmpvar_19.xyz * _Decal1Color) * tmpvar_20) + ((tmpvar_15.xyz * _Decal2Color) * tmpvar_16)) + ((tmpvar_11.xyz * _Decal3Color) * tmpvar_12)) * (1.0 - tmpvar_33)));
			tmpvar_35.w = tmpvar_34;
			o.gl_FragData = (float4(((diffuseSample_6 * (1.0 - tmpvar_34)) + (tmpvar_35.xyz * tmpvar_34)), 1.0) * tmpvar_4);
			return o;
		  }

	  ENDCG
	} // end phase
	Pass // ind: 2, name: 
	{
	  Tags
	  {
	  }
	  ZTest Always
	  ZWrite Off
	  Cull Off
	  Blend SrcAlpha OneMinusSrcAlpha
		  CGPROGRAM

		  #pragma vertex vert
		  #pragma fragment frag

		  #include "UnityCG.cginc"

		  // float4x4 unity_ObjectToWorld;
		   //float4x4 unity_MatrixVP;
		   float _AtlasOffsetU;
		   float _AtlasOffsetV;
		   float _AtlasOffsetScaleU;
		   float _AtlasOffsetScaleV;
		   sampler2D _BodyColorsMaskTex;
		   float3 _BodyRedChannelColor;
		   float3 _BodyGreenChannelColor;
		   float3 _BodyBlueChannelColor;

		   struct v2f
		   {
		   float2 xlv_TEXCOORD0 : TEXCOORD0;
		   };

		   struct FragOutput
		   {
		   float4 gl_FragData : SV_Target;
		   };

		   v2f vert(
		   float4 _glesVertex : POSITION,
		   float4 _glesMultiTexCoord0 : TEXCOORD0,
		   out float4 gl_Position : SV_POSITION
		   )
		   {
			 v2f o;
			 float2 texUV_1;
			 texUV_1.x = ((_glesMultiTexCoord0.x - _AtlasOffsetU) / _AtlasOffsetScaleU);
			 texUV_1.y = ((_glesMultiTexCoord0.y - _AtlasOffsetV) / _AtlasOffsetScaleV);
			 gl_Position = UnityObjectToClipPos(float4(_glesVertex.xyz, 1.0));//mul(unity_MatrixVP, mul(unity_ObjectToWorld * tmpvar_2));
			 o.xlv_TEXCOORD0 = texUV_1;
			 return o;
		   }

		   FragOutput frag(v2f i)
		   {
			 FragOutput o;
			 float2 uv_2;
			 uv_2 = i.xlv_TEXCOORD0;
			 float2 tmpvar_3;
			 tmpvar_3 = abs(((uv_2 - 0.5) * 2.0));
			 float tmpvar_4;
			 tmpvar_4 = float((1.0 >= max(tmpvar_3.x, tmpvar_3.y)));
			 float4 bodyColors_6;
			 float2 texUV_7;
			 texUV_7 = i.xlv_TEXCOORD0;
			 float3 tmpvar_9;
			 tmpvar_9 = tex2D(_BodyColorsMaskTex, texUV_7).xyz;
			 bodyColors_6.xyz = (((tmpvar_9.x * _BodyRedChannelColor) + (tmpvar_9.y * _BodyGreenChannelColor)) + (tmpvar_9.z * _BodyBlueChannelColor));
			 bodyColors_6.w = max(max(tmpvar_9.x, tmpvar_9.y), tmpvar_9.z);
			 o.gl_FragData = (bodyColors_6 * tmpvar_4);
			 return o;
		   }


		   ENDCG

	 } // end phase
	 Pass // ind: 3, name: 
	 {
	   Tags
	   {
	   }
	   ZTest Always
	   ZWrite Off
	   Cull Off
	   Blend Zero SrcColor
		   CGPROGRAM

		   #pragma vertex vert
		   #pragma fragment frag

		   #include "UnityCG.cginc"
		   #include "AutoLight.cginc"
		   #include "Lighting.cginc"

			   //float4 _WorldSpaceLightPos0;
			   //float4x4 unity_ObjectToWorld;
			   //float4x4 unity_WorldToObject;
			   //float4 glstate_lightmodel_ambient;
			   //float4x4 unity_MatrixVP;
			   //float4 _LightColor0;
			   float _AtlasOffsetU;
			   float _AtlasOffsetV;
			   float _AtlasOffsetScaleU;
			   float _AtlasOffsetScaleV;
			   float3 _EmissiveColorTint;
			   sampler2D _DetailAndMatcapMaskAndEmissive;

			   struct v2f
			   {
			   float2 xlv_TEXCOORD0 : TEXCOORD0;
			   float3 xlv_TEXCOORD1 : TEXCOORD1;
			   float3 xlv_COLOR0 : COLOR;
			   };

			   struct FragOutput
			   {
			   float4 gl_FragData : SV_Target;
			   };

			   v2f vert(
			   float4 _glesVertex : POSITION,
			   float4 _glesColor : COLOR,
			   float3 _glesNormal : NORMAL,
			   float4 _glesMultiTexCoord0 : TEXCOORD0,
			   out float4 gl_Position : SV_POSITION
			   )
			   {
				 v2f o;
				 float3 worldSpaceLightDirNormalized_3;
				 float3 worldSpaceNormalNormalized_4;
				 float3 tmpvar_5;
				 float4 v_8;
				 v_8.x = unity_WorldToObject[0].x;
				 v_8.y = unity_WorldToObject[1].x;
				 v_8.z = unity_WorldToObject[2].x;
				 v_8.w = unity_WorldToObject[3].x;
				 float4 v_9;
				 v_9.x = unity_WorldToObject[0].y;
				 v_9.y = unity_WorldToObject[1].y;
				 v_9.z = unity_WorldToObject[2].y;
				 v_9.w = unity_WorldToObject[3].y;
				 float4 v_10;
				 v_10.x = unity_WorldToObject[0].z;
				 v_10.y = unity_WorldToObject[1].z;
				 v_10.z = unity_WorldToObject[2].z;
				 v_10.w = unity_WorldToObject[3].z;
				 worldSpaceNormalNormalized_4 = normalize((( (v_8.xyz * _glesNormal.x) + (v_9.xyz * _glesNormal.y)) + (v_10.xyz * _glesNormal.z)));
				 worldSpaceLightDirNormalized_3 = normalize((_WorldSpaceLightPos0.xyz - ( mul(unity_ObjectToWorld, _glesVertex) .xyz * _WorldSpaceLightPos0.w)));
				 tmpvar_5 = ((_LightColor0.xyz * max(0.0, dot(worldSpaceNormalNormalized_4, worldSpaceLightDirNormalized_3))) * 0.65);
				 tmpvar_5 = (tmpvar_5 + ((glstate_lightmodel_ambient * 2.0).xyz * 0.45));
				 o.xlv_TEXCOORD0.x = ((_glesMultiTexCoord0.x - _AtlasOffsetU) / _AtlasOffsetScaleU);
				 o.xlv_TEXCOORD0.y = ((_glesMultiTexCoord0.y - _AtlasOffsetV) / _AtlasOffsetScaleV);
				 gl_Position = UnityObjectToClipPos(float4(_glesVertex.xyz, 1.0)); //mul(unity_MatrixVP, mul(unity_ObjectToWorld * tmpvar_7));
				 o.xlv_TEXCOORD1 = tmpvar_5;
				 o.xlv_COLOR0 = _glesColor.xyz;
				 return o;
			   }

			   FragOutput frag(v2f i)
			   {
				 FragOutput o;
				 float3 detailOrEmissive_2;
				 float combinedMatcapmaskAndEmissive_3;
				 float3 detail_MatcapMask_Emissive_4;
				 detail_MatcapMask_Emissive_4 = tex2D(_DetailAndMatcapMaskAndEmissive, i.xlv_TEXCOORD0).xyz;
				 float matcap_9;
				 matcap_9 = detail_MatcapMask_Emissive_4.y;
				 float emissive_10;
				 emissive_10 = detail_MatcapMask_Emissive_4.z;
				 float tmpvar_11;
				 tmpvar_11 = float((emissive_10 > 127.0));
				 combinedMatcapmaskAndEmissive_3 = (((0.5 - (matcap_9 * 0.5)) * (1.0 - tmpvar_11)) + (( (0.5 * emissive_10) + 0.5) * tmpvar_11));
				 detailOrEmissive_2 = ((detail_MatcapMask_Emissive_4.x * (1.0 - detail_MatcapMask_Emissive_4.z)) + ((_EmissiveColorTint * detail_MatcapMask_Emissive_4.z) * 2.0));
				 float2 uv_13;
				 uv_13 = i.xlv_TEXCOORD0;
				 float2 tmpvar_14;
				 tmpvar_14 = abs(((uv_13 - 0.5) * 2.0));
				 float tmpvar_15;
				 tmpvar_15 = float((1.0 >= max(tmpvar_14.x, tmpvar_14.y)));
				 o.gl_FragData = ((float4(detailOrEmissive_2, combinedMatcapmaskAndEmissive_3) * tmpvar_15) + float((1.0 - tmpvar_15)));
				 return o;
			   }


			   ENDCG

		 } // end phase
	  }
		  FallBack Off
}
