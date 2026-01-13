Shader "CpRemix/Equipment Screenshot"
{
	Properties
	{
	  _Diffuse("Diffuse", 2D) = "black" {}
	  [MaterialToggle] _UseUV2ForDecals("Use UV2 for Decals", float) = 0
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
	  _ScreenshotBGColor("Screenshot Background Color", Color) = (0.03,0.03,0.03,1)
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
			  CGPROGRAM

			  #pragma vertex vert
			  #pragma fragment frag

			  #include "UnityCG.cginc"

		  //float4x4 unity_ObjectToWorld;
		  //float4x4 unity_MatrixVP;
		  float _Decal1Scale;
		  float _Decal1UOffset;
		  float _Decal1VOffset;
		  float _Decal1RotationRads;
		  float _Decal2Scale;
		  float _Decal2UOffset;
		  float _Decal2VOffset;
		  float _Decal2RotationRads;
		  float _Decal3Scale;
		  float _Decal3UOffset;
		  float _Decal3VOffset;
		  float _Decal3RotationRads;
		  float _Decal4Scale;
		  float _Decal4UOffset;
		  float _Decal4VOffset;
		  float _Decal4RotationRads;
		  float _Decal5Scale;
		  float _Decal5UOffset;
		  float _Decal5VOffset;
		  float _Decal5RotationRads;
		  float _Decal6Scale;
		  float _Decal6UOffset;
		  float _Decal6VOffset;
		  float _Decal6RotationRads;
		  sampler2D _Diffuse;
		  sampler2D _Decal123OpacityTex;
		  sampler2D _Decal1Tex;
		  float3 _Decal1Color;
		  float _Decal1Repeat;
		  sampler2D _Decal2Tex;
		  float3 _Decal2Color;
		  float _Decal2Repeat;
		  sampler2D _Decal3Tex;
		  float3 _Decal3Color;
		  float _Decal3Repeat;
		  sampler2D _Decal4Tex;
		  float3 _Decal4Color;
		  float _Decal4Repeat;
		  sampler2D _Decal5Tex;
		  float3 _Decal5Color;
		  float _Decal5Repeat;
		  sampler2D _Decal6Tex;
		  float3 _Decal6Color;
		  float _Decal6Repeat;

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

		  struct FragOutput
		  {
		  float4 color : SV_Target;
		  };

		  v2f vert(
		  float4 _glesVertex : POSITION,
		  float4 _glesMultiTexCoord0 : TEXCOORD0,
		  out float4 gl_Position : SV_POSITION
		  )
		  {
			v2f o;
			float2 tmpvar_1;
			tmpvar_1 = _glesMultiTexCoord0.xy;
			float2 decal6RotatedUVs_2;
			float2 decal5RotatedUVs_3;
			float2 decal4RotatedUVs_4;
			float2 decal3RotatedUVs_5;
			float2 decal2RotatedUVs_6;
			float2 decal1RotatedUVs_7;
			float4 tmpvar_8;
			tmpvar_8.w = 1.0;
			tmpvar_8.xyz = _glesVertex.xyz;
			float2 tmpvar_9;
			tmpvar_9.x = (-0.5 + _Decal1UOffset);
			tmpvar_9.y = (-0.5 + _Decal1VOffset);
			float2 tmpvar_10;
			float2 point2D_11;
			point2D_11 = tmpvar_1;
			float2 pointLocalCenterToOrigin_12;
			pointLocalCenterToOrigin_12 = tmpvar_9;
			float angleRadians_13;
			angleRadians_13 = _Decal1RotationRads;
			float tmpvar_14;
			tmpvar_14 = sin(angleRadians_13);
			float tmpvar_15;
			tmpvar_15 = cos(angleRadians_13);
			float2x2 tmpvar_16;
			tmpvar_16[0].x = tmpvar_15;
			tmpvar_16[0].y = tmpvar_14;
			tmpvar_16[1].x = -(tmpvar_14);
			tmpvar_16[1].y = tmpvar_15;
			tmpvar_10 = (mul((point2D_11 + pointLocalCenterToOrigin_12), tmpvar_16) - pointLocalCenterToOrigin_12);
			decal1RotatedUVs_7 = tmpvar_10;
			float2 tmpvar_17;
			tmpvar_17.x = _Decal1UOffset;
			tmpvar_17.y = _Decal1VOffset;
			float2 tmpvar_18;
			tmpvar_18.x = (-0.5 + _Decal2UOffset);
			tmpvar_18.y = (-0.5 + _Decal2VOffset);
			float2 tmpvar_19;
			float2 point2D_20;
			point2D_20 = tmpvar_1;
			float2 pointLocalCenterToOrigin_21;
			pointLocalCenterToOrigin_21 = tmpvar_18;
			float angleRadians_22;
			angleRadians_22 = _Decal2RotationRads;
			float tmpvar_23;
			tmpvar_23 = sin(angleRadians_22);
			float tmpvar_24;
			tmpvar_24 = cos(angleRadians_22);
			float2x2 tmpvar_25;
			tmpvar_25[0].x = tmpvar_24;
			tmpvar_25[0].y = tmpvar_23;
			tmpvar_25[1].x = -(tmpvar_23);
			tmpvar_25[1].y = tmpvar_24;
			tmpvar_19 = (mul((point2D_20 + pointLocalCenterToOrigin_21), tmpvar_25) - pointLocalCenterToOrigin_21);
			decal2RotatedUVs_6 = tmpvar_19;
			float2 tmpvar_26;
			tmpvar_26.x = _Decal2UOffset;
			tmpvar_26.y = _Decal2VOffset;
			float2 tmpvar_27;
			tmpvar_27.x = (-0.5 + _Decal3UOffset);
			tmpvar_27.y = (-0.5 + _Decal3VOffset);
			float2 tmpvar_28;
			float2 point2D_29;
			point2D_29 = tmpvar_1;
			float2 pointLocalCenterToOrigin_30;
			pointLocalCenterToOrigin_30 = tmpvar_27;
			float angleRadians_31;
			angleRadians_31 = _Decal3RotationRads;
			float tmpvar_32;
			tmpvar_32 = sin(angleRadians_31);
			float tmpvar_33;
			tmpvar_33 = cos(angleRadians_31);
			float2x2 tmpvar_34;
			tmpvar_34[0].x = tmpvar_33;
			tmpvar_34[0].y = tmpvar_32;
			tmpvar_34[1].x = -(tmpvar_32);
			tmpvar_34[1].y = tmpvar_33;
			tmpvar_28 = (mul((point2D_29 + pointLocalCenterToOrigin_30), tmpvar_34) - pointLocalCenterToOrigin_30);
			decal3RotatedUVs_5 = tmpvar_28;
			float2 tmpvar_35;
			tmpvar_35.x = _Decal3UOffset;
			tmpvar_35.y = _Decal3VOffset;
			float2 tmpvar_36;
			tmpvar_36.x = (-0.5 + _Decal4UOffset);
			tmpvar_36.y = (-0.5 + _Decal4VOffset);
			float2 tmpvar_37;
			float2 point2D_38;
			point2D_38 = tmpvar_1;
			float2 pointLocalCenterToOrigin_39;
			pointLocalCenterToOrigin_39 = tmpvar_36;
			float angleRadians_40;
			angleRadians_40 = _Decal4RotationRads;
			float tmpvar_41;
			tmpvar_41 = sin(angleRadians_40);
			float tmpvar_42;
			tmpvar_42 = cos(angleRadians_40);
			float2x2 tmpvar_43;
			tmpvar_43[0].x = tmpvar_42;
			tmpvar_43[0].y = tmpvar_41;
			tmpvar_43[1].x = -(tmpvar_41);
			tmpvar_43[1].y = tmpvar_42;
			tmpvar_37 = (mul((point2D_38 + pointLocalCenterToOrigin_39), tmpvar_43) - pointLocalCenterToOrigin_39);
			decal4RotatedUVs_4 = tmpvar_37;
			float2 tmpvar_44;
			tmpvar_44.x = _Decal4UOffset;
			tmpvar_44.y = _Decal4VOffset;
			float2 tmpvar_45;
			tmpvar_45.x = (-0.5 + _Decal5UOffset);
			tmpvar_45.y = (-0.5 + _Decal5VOffset);
			float2 tmpvar_46;
			float2 point2D_47;
			point2D_47 = tmpvar_1;
			float2 pointLocalCenterToOrigin_48;
			pointLocalCenterToOrigin_48 = tmpvar_45;
			float angleRadians_49;
			angleRadians_49 = _Decal5RotationRads;
			float tmpvar_50;
			tmpvar_50 = sin(angleRadians_49);
			float tmpvar_51;
			tmpvar_51 = cos(angleRadians_49);
			float2x2 tmpvar_52;
			tmpvar_52[0].x = tmpvar_51;
			tmpvar_52[0].y = tmpvar_50;
			tmpvar_52[1].x = -(tmpvar_50);
			tmpvar_52[1].y = tmpvar_51;
			tmpvar_46 = (mul((point2D_47 + pointLocalCenterToOrigin_48), tmpvar_52) - pointLocalCenterToOrigin_48);
			decal5RotatedUVs_3 = tmpvar_46;
			float2 tmpvar_53;
			tmpvar_53.x = _Decal5UOffset;
			tmpvar_53.y = _Decal5VOffset;
			float2 tmpvar_54;
			tmpvar_54.x = (-0.5 + _Decal6UOffset);
			tmpvar_54.y = (-0.5 + _Decal6VOffset);
			float2 tmpvar_55;
			float2 point2D_56;
			point2D_56 = tmpvar_1;
			float2 pointLocalCenterToOrigin_57;
			pointLocalCenterToOrigin_57 = tmpvar_54;
			float angleRadians_58;
			angleRadians_58 = _Decal6RotationRads;
			float tmpvar_59;
			tmpvar_59 = sin(angleRadians_58);
			float tmpvar_60;
			tmpvar_60 = cos(angleRadians_58);
			float2x2 tmpvar_61;
			tmpvar_61[0].x = tmpvar_60;
			tmpvar_61[0].y = tmpvar_59;
			tmpvar_61[1].x = -(tmpvar_59);
			tmpvar_61[1].y = tmpvar_60;
			tmpvar_55 = (mul((point2D_56 + pointLocalCenterToOrigin_57), tmpvar_61) - pointLocalCenterToOrigin_57);
			decal6RotatedUVs_2 = tmpvar_55;
			float2 tmpvar_62;
			tmpvar_62.x = _Decal6UOffset;
			tmpvar_62.y = _Decal6VOffset;
			gl_Position = UnityObjectToClipPos(tmpvar_8);//mul(unity_MatrixVP, mul(unity_ObjectToWorld, tmpvar_8));(unity_MatrixVP * (unity_ObjectToWorld * tmpvar_8));
			o.xlv_TEXCOORD0 = tmpvar_1;
			o.xlv_TEXCOORD1 = (((
			  (decal1RotatedUVs_7 + tmpvar_17)
			 - float2(0.5, 0.5)) * _Decal1Scale) + float2(0.5, 0.5));
			o.xlv_TEXCOORD2 = (((
			  (decal2RotatedUVs_6 + tmpvar_26)
			 - float2(0.5, 0.5)) * _Decal2Scale) + float2(0.5, 0.5));
			o.xlv_TEXCOORD3 = (((
			  (decal3RotatedUVs_5 + tmpvar_35)
			 - float2(0.5, 0.5)) * _Decal3Scale) + float2(0.5, 0.5));
			o.xlv_TEXCOORD4 = (((
			  (decal4RotatedUVs_4 + tmpvar_44)
			 - float2(0.5, 0.5)) * _Decal4Scale) + float2(0.5, 0.5));
			o.xlv_TEXCOORD5 = (((
			  (decal5RotatedUVs_3 + tmpvar_53)
			 - float2(0.5, 0.5)) * _Decal5Scale) + float2(0.5, 0.5));
			o.xlv_TEXCOORD6 = (((
			  (decal6RotatedUVs_2 + tmpvar_62)
			 - float2(0.5, 0.5)) * _Decal6Scale) + float2(0.5, 0.5));
			 return o;
		  }


		  FragOutput frag(v2f i)
		  {
			FragOutput o;
			float3 decalOpacitySample_1;
			float3 diffuseSample_2;
			float3 tmpvar_3;
			tmpvar_3 = tex2D(_Diffuse, i.xlv_TEXCOORD0).xyz;
			diffuseSample_2 = tmpvar_3;
			float3 tmpvar_4;
			tmpvar_4 = tex2D(_Decal123OpacityTex, i.xlv_TEXCOORD0).xyz;
			decalOpacitySample_1 = tmpvar_4;
			float4 tmpvar_5;
			tmpvar_5 = tex2D(_Decal3Tex, i.xlv_TEXCOORD3);
			float2 tmpvar_6;
			tmpvar_6 = abs(((i.xlv_TEXCOORD3 - 0.5) * 2.0));
			float4 tmpvar_7;
			tmpvar_7 = (tmpvar_5 * float((
			  (1.0 + (255.0 * _Decal3Repeat))
			 >=
			  max(tmpvar_6.x, tmpvar_6.y)
			)));
			float tmpvar_8;
			tmpvar_8 = (tmpvar_7.w * decalOpacitySample_1.z);
			float4 tmpvar_9;
			tmpvar_9 = tex2D(_Decal2Tex, i.xlv_TEXCOORD2);
			float2 tmpvar_10;
			tmpvar_10 = abs(((i.xlv_TEXCOORD2 - 0.5) * 2.0));
			float4 tmpvar_11;
			tmpvar_11 = (tmpvar_9 * float((
			  (1.0 + (255.0 * _Decal2Repeat))
			 >=
			  max(tmpvar_10.x, tmpvar_10.y)
			)));
			float tmpvar_12;
			tmpvar_12 = (tmpvar_11.w * decalOpacitySample_1.y);
			float4 tmpvar_13;
			tmpvar_13 = tex2D(_Decal1Tex, i.xlv_TEXCOORD1);
			float2 tmpvar_14;
			tmpvar_14 = abs(((i.xlv_TEXCOORD1 - 0.5) * 2.0));
			float4 tmpvar_15;
			tmpvar_15 = (tmpvar_13 * float((
			  (1.0 + (255.0 * _Decal1Repeat))
			 >=
			  max(tmpvar_14.x, tmpvar_14.y)
			)));
			float tmpvar_16;
			tmpvar_16 = (tmpvar_15.w * decalOpacitySample_1.x);
			float4 tmpvar_17;
			tmpvar_17 = tex2D(_Decal6Tex, i.xlv_TEXCOORD6);
			float2 tmpvar_18;
			tmpvar_18 = abs(((i.xlv_TEXCOORD6 - 0.5) * 2.0));
			float4 tmpvar_19;
			tmpvar_19 = (tmpvar_17 * float((
			  (1.0 + (255.0 * _Decal6Repeat))
			 >=
			  max(tmpvar_18.x, tmpvar_18.y)
			)));
			float tmpvar_20;
			tmpvar_20 = (tmpvar_19.w * decalOpacitySample_1.z);
			float4 tmpvar_21;
			tmpvar_21 = tex2D(_Decal5Tex, i.xlv_TEXCOORD5);
			float2 tmpvar_22;
			tmpvar_22 = abs(((i.xlv_TEXCOORD5 - 0.5) * 2.0));
			float4 tmpvar_23;
			tmpvar_23 = (tmpvar_21 * float((
			  (1.0 + (255.0 * _Decal5Repeat))
			 >=
			  max(tmpvar_22.x, tmpvar_22.y)
			)));
			float tmpvar_24;
			tmpvar_24 = ((tmpvar_23.w * decalOpacitySample_1.y) * (1.0 - tmpvar_20));
			float4 tmpvar_25;
			tmpvar_25 = tex2D(_Decal4Tex, i.xlv_TEXCOORD4);
			float2 tmpvar_26;
			tmpvar_26 = abs(((i.xlv_TEXCOORD4 - 0.5) * 2.0));
			float4 tmpvar_27;
			tmpvar_27 = (tmpvar_25 * float((
			  (1.0 + (255.0 * _Decal4Repeat))
			 >=
			  max(tmpvar_26.x, tmpvar_26.y)
			)));
			float tmpvar_28;
			tmpvar_28 = (((tmpvar_27.w * decalOpacitySample_1.x) * (1.0 - tmpvar_24)) * (1.0 - tmpvar_20));
			float tmpvar_29;
			tmpvar_29 = min(1.0, ((tmpvar_20 + tmpvar_24) + tmpvar_28));
			float tmpvar_30;
			tmpvar_30 = min(1.0, (min(1.0,
			  ((tmpvar_8 + tmpvar_12) + tmpvar_16)
			) + tmpvar_29));
			float4 tmpvar_31;
			tmpvar_31.xyz = (((
			  (((tmpvar_27.xyz * _Decal4Color) * tmpvar_28) + ((tmpvar_23.xyz * _Decal5Color) * tmpvar_24))
			 +
			  ((tmpvar_19.xyz * _Decal6Color) * tmpvar_20)
			) * tmpvar_29) + ((
			  (((tmpvar_15.xyz * _Decal1Color) * tmpvar_16) + ((tmpvar_11.xyz * _Decal2Color) * tmpvar_12))
			 +
			  ((tmpvar_7.xyz * _Decal3Color) * tmpvar_8)
			) * (1.0 - tmpvar_29)));
			tmpvar_31.w = tmpvar_30;
			float4 tmpvar_32;
			tmpvar_32.w = 1.0;
			tmpvar_32.xyz = ((diffuseSample_2 * (1.0 - tmpvar_30)) + (tmpvar_31.xyz * tmpvar_30));
			o.color = tmpvar_32;
			return o;
		  }


		  ENDCG

	} // end phase
	Pass // ind: 2, name: 
	{
	  Tags
	  {
	  }
	  Blend SrcAlpha OneMinusSrcAlpha
		  CGPROGRAM

		  #pragma vertex vert
		  #pragma fragment frag

		  #include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			// $Globals ConstantBuffers for Fragment Shader
			float3 _BodyRedChannelColor;
			float3 _BodyGreenChannelColor;
			float3 _BodyBlueChannelColor;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _BodyColorsMaskTex;
			
			// Keywords: 
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp0 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp1 = tmp0.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp1 = unity_MatrixVP._m00_m10_m20_m30 * tmp0.xxxx + tmp1;
                tmp1 = unity_MatrixVP._m02_m12_m22_m32 * tmp0.zzzz + tmp1;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp0.wwww + tmp1;
                o.texcoord.xy = v.texcoord.xy;
                return o;
			}
			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                tmp0 = tex2D(_BodyColorsMaskTex, inp.texcoord.xy);
                tmp1.xyz = tmp0.yyy * _BodyGreenChannelColor;
                tmp1.xyz = tmp0.xxx * _BodyRedChannelColor + tmp1.xyz;
                tmp1.xyz = tmp0.zzz * _BodyBlueChannelColor + tmp1.xyz;
                tmp0.w = tmp0.x + tmp0.z;
                tmp0.w = tmp0.y + tmp0.w;
                tmp0.w = tmp0.w > 0.3;
                tmp1.w = tmp0.w ? 1.0 : 0.0;
                tmp0.w = tmp0.w ? 0.0 : 1.0;
                tmp2.xyz = tmp1.www * _BodyRedChannelColor;
                o.sv_target.xyz = tmp1.xyz * tmp0.www + tmp2.xyz;
                tmp0.x = max(tmp0.y, tmp0.x);
                o.sv_target.w = max(tmp0.z, tmp0.x);
                return o;
			}
			ENDCG
		}
		Pass {
			Tags { "LIGHTMODE" = "FORWARDBASE" }
			Blend Zero SrcColor, Zero SrcColor
			GpuProgramID 133977
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 position : SV_POSITION0;
				float2 texcoord : TEXCOORD0;
				float3 texcoord1 : TEXCOORD1;
				float3 color : COLOR0;
			};
			struct fout
			{
				float4 sv_target : SV_Target0;
			};
			// $Globals ConstantBuffers for Vertex Shader
			float4 _LightColor0;
			// $Globals ConstantBuffers for Fragment Shader
			float3 _EmissiveColorTint;
			// Custom ConstantBuffers for Vertex Shader
			// Custom ConstantBuffers for Fragment Shader
			// Texture params for Vertex Shader
			// Texture params for Fragment Shader
			sampler2D _DetailAndMatcapMaskAndEmissive;
			sampler2D _BodyColorsMaskTex;
			
			// Keywords: 
			v2f vert(appdata_full v)
			{
                v2f o;
                float4 tmp0;
                float4 tmp1;
                float4 tmp2;
                tmp0 = v.vertex.yyyy * unity_ObjectToWorld._m01_m11_m21_m31;
                tmp0 = unity_ObjectToWorld._m00_m10_m20_m30 * v.vertex.xxxx + tmp0;
                tmp0 = unity_ObjectToWorld._m02_m12_m22_m32 * v.vertex.zzzz + tmp0;
                tmp1 = tmp0 + unity_ObjectToWorld._m03_m13_m23_m33;
                tmp0.xyz = unity_ObjectToWorld._m03_m13_m23 * v.vertex.www + tmp0.xyz;
                tmp0.xyz = -tmp0.xyz * _WorldSpaceLightPos0.www + _WorldSpaceLightPos0.xyz;
                tmp2 = tmp1.yyyy * unity_MatrixVP._m01_m11_m21_m31;
                tmp2 = unity_MatrixVP._m00_m10_m20_m30 * tmp1.xxxx + tmp2;
                tmp2 = unity_MatrixVP._m02_m12_m22_m32 * tmp1.zzzz + tmp2;
                o.position = unity_MatrixVP._m03_m13_m23_m33 * tmp1.wwww + tmp2;
                o.texcoord.xy = v.texcoord.xy;
                tmp0.w = dot(tmp0.xyz, tmp0.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp0.xyz = tmp0.www * tmp0.xyz;
                tmp1.x = v.normal.x * unity_WorldToObject._m00;
                tmp1.y = v.normal.x * unity_WorldToObject._m01;
                tmp1.z = v.normal.x * unity_WorldToObject._m02;
                tmp2.x = v.normal.y * unity_WorldToObject._m10;
                tmp2.y = v.normal.y * unity_WorldToObject._m11;
                tmp2.z = v.normal.y * unity_WorldToObject._m12;
                tmp1.xyz = tmp1.xyz + tmp2.xyz;
                tmp2.x = v.normal.z * unity_WorldToObject._m20;
                tmp2.y = v.normal.z * unity_WorldToObject._m21;
                tmp2.z = v.normal.z * unity_WorldToObject._m22;
                tmp1.xyz = tmp1.xyz + tmp2.xyz;
                tmp0.w = dot(tmp1.xyz, tmp1.xyz);
                tmp0.w = rsqrt(tmp0.w);
                tmp1.xyz = tmp0.www * tmp1.xyz;
                tmp0.x = dot(tmp1.xyz, tmp0.xyz);
                tmp0.x = max(tmp0.x, 0.0);
                tmp0.xyz = tmp0.xxx * _LightColor0.xyz;
                tmp1.xyz = glstate_lightmodel_ambient.xyz * float3(0.9, 0.9, 0.9);
                o.texcoord1.xyz = tmp0.xyz * float3(0.65, 0.65, 0.65) + tmp1.xyz;
                o.color.xyz = v.color.xyz;
                return o;
			}
			// Keywords: 
			fout frag(v2f inp)
			{
                fout o;
                float4 tmp0;
                float4 tmp1;
                tmp0 = tex2D(_DetailAndMatcapMaskAndEmissive, inp.texcoord.xy);
                tmp0.xyw = tmp0.xxx * inp.texcoord1.xyz;
                tmp1.x = 1.0 - tmp0.z;
                tmp1.yzw = tmp0.zzz * _EmissiveColorTint;
                tmp0.xyz = tmp0.xyw * tmp1.xxx + tmp1.yzw;
                tmp1 = tex2D(_BodyColorsMaskTex, inp.texcoord.xy);
                tmp0.w = tmp1.x + tmp1.z;
                tmp0.w = tmp1.y + tmp0.w;
                tmp0.w = tmp0.w > 0.3;
                tmp1.x = tmp0.w ? 1.0 : 0.0;
                tmp0.w = tmp0.w ? 0.0 : 1.0;
                o.sv_target.xyz = tmp0.xyz * tmp0.www + tmp1.xxx;
                o.sv_target.w = 1.0;
                return o;
			}
			ENDCG
		}
	}
}