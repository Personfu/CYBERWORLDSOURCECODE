Shader "CpRemix/BlobShadows/ShadowGeoShader"
{
	Properties
	{
	  _MainTex("Texture", 2D) = "white" {}
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

		// m_ProgramMask = 6
		CGPROGRAM
		//#pragma target 4.0

		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		  uniform float4x4 _blobShadowCamVp;
		  uniform sampler2D _MainTex;

		  struct appdata_t
		  {
			  float4 _glesVertex :POSITION;
			  float4 _glesMultiTexCoord0 : TEXCOORD0;
		  };


		  struct OUT_Data_Vert
		  {
			   float2 xlv_TEXCOORD0 :TEXCOORD0;
			   float xlv_TEXCOORD1 : TEXCOORD1;
			   float xlv_TEXCOORD2 : TEXCOORD2;
			   float4 gl_Position  :SV_POSITION;
		  };

		  struct v2f
		  {
			  float2 xlv_TEXCOORD0 :TEXCOORD0;
			  float xlv_TEXCOORD1 : TEXCOORD1;
			  float xlv_TEXCOORD2 : TEXCOORD2;
		  };

		  struct OUT_Data_Frag
		  {
			 float4 gl_FragData :SV_Target0;
		  };

		  OUT_Data_Vert vert(appdata_t v)
		  {
			OUT_Data_Vert o;
			float4 tmpvar_2;
			tmpvar_2.xzw = v._glesVertex.xzw;
			tmpvar_2.y = 0.0;
			float4 tmpvar_4 = mul(_blobShadowCamVp, tmpvar_2);
			float maxScreenSpace_3 = max(abs(tmpvar_4.x), abs(tmpvar_4.y));
			o.xlv_TEXCOORD0 = v._glesMultiTexCoord0.xy;
			o.gl_Position = tmpvar_4;
			o.xlv_TEXCOORD1 = max(0.0, ((maxScreenSpace_3 - 0.8) * 5.0));
			o.xlv_TEXCOORD2 = v._glesVertex.y;
			return o;
		  }


		  OUT_Data_Frag frag(v2f f)
		  {
			OUT_Data_Frag o;
			o.gl_FragData.zw = float2(1.0, 1.0);
			o.gl_FragData.x = tex2D(_MainTex, f.xlv_TEXCOORD0).x + f.xlv_TEXCOORD1;
			o.gl_FragData.y = f.xlv_TEXCOORD2;
			return o;
		  }
	  ENDCG
	} // end phase
	}
		FallBack Off
}
