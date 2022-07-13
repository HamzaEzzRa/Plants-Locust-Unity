struct Attributes
{
	float4 positionOS	: POSITION; // OS -> Object Space
	float3 normalOS		: NORMAL;
	float4 tangentOS	: TANGENT;
	float2 uv			: TEXCOORD0;
	float2 uvLM			: TEXCOORD1; // Light Map UVs
	float4 color		: COLOR; // Vertex color data (think of vertex painting in blender)

	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
	float3 normalOS						: NORMAL;
	float2 uv							: TEXCOORD0;
	float2 uvLM							: TEXCOORD1;
	float4 positionWSAndFogFactor		: TEXCOORD2; // w -> vertex fog factor
	half3 normalWS						: TEXCOORD3; // WS -> World Space
	half3 tangentWS						: TEXCOORD4;
	float4 positionOS					: TEXCOORD5;

	float4 color						: COLOR;
	#if _NORMALMAP
	half3 bitangentWS					: TEXCOORD5;
	#endif

	#ifdef _MAIN_LIGHT_SHADOWS
	float4 shadowCoord					: TEXCOORD6;
	#endif
	float4 positionCS					: SV_POSITION;
};

// Utilities
float3x3 RotX(float theta)
{
	return float3x3
	(
		1, 0, 0,
		0, cos(theta), -sin(theta),
		0, sin(theta), cos(theta)
	);
}

float3x3 RotY(float theta)
{
	return float3x3
	(
		cos(theta), 0, sin(theta),
		0, 1, 0,
		-sin(theta), 0, cos(theta)
	);
}

float3x3 RotZ(float theta)
{
	return float3x3
	(
		cos(theta), -sin(theta), 0,
		sin(theta), cos(theta), 0,
		0, 0, 1
	);
}

float rand(float3 co)
{
	return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 53.539))) * 43758.5453);
}

float4 CustomWorldToShadowCoord(float3 positionWS)
{
	#ifdef _MAIN_LIGHT_SHADOWS_CASCADE
	half cascadeIndex = ComputeCascadeIndex(positionWS);
	#else
	half cascadeIndex = 0;
	#endif

	return mul(_MainLightWorldToShadow[cascadeIndex], float4(positionWS, 1));
}

// Properties
sampler2D _DistortionMap;
sampler2D _TrampleMap;
float4 _WindFrequency;
float _WindStrength;
float _Height;
float _Base;
float4 _Tint;
float _LightFactor;
float _TranslucentFactor;
float _AlphaClip;
float4 _BlendColor;
float _ShadowFactor;
float _MinHeight;
float _MaxHeight;
float _ReceiveShadows;
half _TrampleMultiplier;

// Vertex Pass
Varyings LitPassVertex(Attributes input)
{
	Varyings output;

	output.color = input.color;

	VertexPositionInputs vPositionInput = GetVertexPositionInputs(input.positionOS.xyz);
	VertexNormalInputs vNormalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
	float fogFactor = ComputeFogFactor(vPositionInput.positionCS.z);

	output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
	output.uvLM = input.uvLM.xy * unity_LightmapST.xy + unity_LightmapST.zw;

	output.positionOS = input.positionOS;
	output.positionCS = vPositionInput.positionCS;
	output.positionWSAndFogFactor = float4(vPositionInput.positionWS, fogFactor);

	output.normalOS = input.normalOS;
	output.normalWS = vNormalInput.normalWS;
	output.tangentWS = vNormalInput.tangentWS;

	#ifdef _NORMALMAP
		output.bitangentWS = vNormalInput.bitangentWS;
	#endif

	#ifdef _MAIN_LIGHT_SHADOWS
		output.shadowCoord = GetShadowCoord(vPositionInput);
	#endif

	return output;
}

//[maxvertexcount(10)]
//void LitPassGeom(triangle Varyings input[3], inout TriangleStream<Varyings> outStream)
//{
//	if (input[0].color.g < 0.1 && input[0].color.r > 0.9)
//		return;
//
//	float2 uv = (input[0].positionOS.xy * _Time.xy * _WindFrequency.xy);
//	float4 windSample = tex2Dlod(_DistortionMap, float4(uv, 0, 0)) * _WindStrength;
//
//	float3 rotatedNormalZ = mul(input[0].normalWS, RotZ(windSample.x));
//	float3 rotatedNormal = mul(rotatedNormalZ, RotX(windSample.y));
//
//	float randomHeightFactor = rand(input[0].positionWSAndFogFactor.xyz * 2.27323f);
//
//	float3 basePos = (input[0].positionWSAndFogFactor.xyz + input[1].positionWSAndFogFactor.xyz + input[2].positionWSAndFogFactor.xyz) * 0.333333;
//	
//	float3 rotatedTangent = normalize(mul(input[0].tangentWS, RotY(rand(input[0].positionWSAndFogFactor.xyz) * 90)));
//	float3 correctedNormal = mul(rotatedTangent, RotY(PI * 0.5));
//
//	Varyings o1 = input[0];
//	o1.positionCS = TransformWorldToHClip(basePos - rotatedTangent * _Base);
//	o1.uv = TRANSFORM_TEX(float2(0, 0), _BaseMap);
//	o1.normalWS = correctedNormal;
//
//	Varyings o2 = input[0];
//	o2.positionCS = TransformWorldToHClip(basePos + rotatedTangent * _Base);
//	o2.uv = TRANSFORM_TEX(float2(1, 0), _BaseMap);
//	o2.normalWS = correctedNormal;
//
//	Varyings o3 = input[0];
//	o3.positionCS = TransformWorldToHClip(basePos + rotatedNormal * clamp(_Height * randomHeightFactor, _MinHeight, _MaxHeight) + rotatedTangent * _Base);
//	o3.uv = TRANSFORM_TEX(float2(1, 1), _BaseMap);
//	o3.normalWS = correctedNormal;
//
//	Varyings o4 = input[0];
//	o4.positionCS = TransformWorldToHClip(basePos + rotatedNormal * _Height - rotatedTangent * _Base);
//	o4.uv = TRANSFORM_TEX(float2(0, 1), _BaseMap);
//	o4.normalWS = correctedNormal;
//
//	outStream.Append(o4);
//	outStream.Append(o3);
//	outStream.Append(o1);
//	outStream.RestartStrip();
//
//	outStream.Append(o3);
//	outStream.Append(o2);
//	outStream.Append(o1);
//	outStream.RestartStrip();
//}

[maxvertexcount(4)]
void LitPassGeom(point Varyings input[1], inout TriangleStream<Varyings> outStream)
{
	if (input[0].color.g < 0.1 && input[0].color.r > 0.9)
		return;

	float3 basePos = input[0].positionWSAndFogFactor.xyz;

	float4 trample = tex2Dlod(_TrampleMap, float4(1 - (basePos.x * 0.01 + 0.5), 1 - (basePos.z * 0.01 + 0.5), 0, 0));
	trample = UnPackFloat4(trample);
	trample.y *= _Height;
	//trample = normalize(trample);

	float2 uv = (input[0].positionOS.xy * _Time.xy * _WindFrequency.xy);
	float4 windSample = tex2Dlod(_DistortionMap, float4(uv, 0, 0)) * _WindStrength;

	float3 rotatedNormalZ = mul(input[0].normalWS, RotZ(windSample.x));
	float3 rotatedNormal = mul(rotatedNormalZ, RotX(windSample.y));

	float randomHeightFactor = rand(input[0].positionWSAndFogFactor.xyz * 2.27323f);

	float3 rotatedTangent = normalize(mul(input[0].tangentWS, RotY(rand(basePos) * 90)));
	float3 correctedNormal = mul(rotatedTangent, RotY(PI * 0.5));

	Varyings o1 = input[0];
	o1.positionCS = TransformWorldToHClip(basePos - rotatedTangent * _Base);
	o1.uv = TRANSFORM_TEX(float2(0, 0), _BaseMap);
	o1.normalWS = correctedNormal;
	outStream.Append(o1);

	Varyings o2 = input[0];
	o2.positionCS = TransformWorldToHClip(basePos + rotatedTangent * _Base);
	o2.uv = TRANSFORM_TEX(float2(1, 0), _BaseMap);
	o2.normalWS = correctedNormal;
	outStream.Append(o2);

	Varyings o3 = input[0];
	o3.positionCS = TransformWorldToHClip(basePos + rotatedNormal * clamp(_Height * randomHeightFactor, _MinHeight, _MaxHeight) + rotatedTangent * _Base + trample * _TrampleMultiplier);
	o3.uv = TRANSFORM_TEX(float2(1, 1), _BaseMap);
	o3.normalWS = correctedNormal;
	outStream.Append(o3);

	Varyings o4 = input[0];
	o4.positionCS = TransformWorldToHClip(basePos + rotatedNormal * clamp(_Height * randomHeightFactor, _MinHeight, _MaxHeight) - rotatedTangent * _Base + trample * _TrampleMultiplier);
	o4.uv = TRANSFORM_TEX(float2(0, 1), _BaseMap);
	o4.normalWS = correctedNormal;
	outStream.Append(o4);

	outStream.RestartStrip();
}

half4 LitPassFragment(Varyings input, bool vf : SV_IsFrontFace) : SV_Target
{
	SurfaceData surfaceData;
	InitializeStandardLitSurfaceData(input.uvLM, surfaceData);

	BRDFData brdfData;
	InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);

	//float3 positionWS = input.positionWSAndFogFactor.xyz;
	float3 positionWS = input.positionOS.xyz;
	float fogFactor = input.positionWSAndFogFactor.w;

	half3 normalWS = normalize(input.normalWS);
	if (vf == true)
		normalWS = -normalWS;

	half3 bakedGI = SampleSH(normalWS);

	float4 shadowCoord = CustomWorldToShadowCoord(positionWS);
	Light mainLight = GetMainLight(shadowCoord);
	
	if (_ReceiveShadows == 1)
	{
		#if SHADOWS_SCREEN
		float4 clipPos = TransformWorldToHClip(input.positionWS);
		float4 shadowCoord = ComputeScreenPos(clipPos);
		#else
		float4 shadowCoord = CustomWorldToShadowCoord(positionWS);
		#endif
		mainLight = GetMainLight(shadowCoord);
	}
	else
	{
		#ifdef _MAIN_LIGHT_SHADOWS
		mainLight = GetMainLight(input.shadowCoord);
		#else
		mainLight = GetMainLight();
		#endif
	}

	float3 normalLight = LightingLambert(mainLight.color, mainLight.direction, normalWS) * _LightFactor;
	float3 invertNormalLight = LightingLambert(mainLight.color, mainLight.direction, -normalWS) * _TranslucentFactor;

	half3 color = _Tint.rgb + normalLight + invertNormalLight;
	color = lerp(color, _BlendColor.xyz, 1 - input.uv.y);
	color = lerp(_BlendColor.xyz, color, clamp(mainLight.shadowAttenuation + _ShadowFactor, 0, 1));
	color = MixFog(color, fogFactor);
	color *= mainLight.color * _LightFactor;

	float a = _BaseMap.Sample(sampler_BaseMap, input.uv).a;
	clip(a - _AlphaClip);

	return half4(color, a);
}
