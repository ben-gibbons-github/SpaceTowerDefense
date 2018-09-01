float4x4 World;
float4x4 View;
float4x4 InverseView;
float4x4 Projection;
float4x4 InverseViewProjection;
float3 CameraPosition;

float2 TextureSize;
float3 LightDistance;
float3 LightPosition;


float4 Color = float4(0.5,0.5,0.5,1);
float SpecularPower = 0.5;
float AmbientLight = 0.5;

sampler GBuffer1 : register(s0);
sampler GBuffer2 : register(s1);

float2 UVOffset = float2(0, 0);
float2 UVMult = float2(0, 0);

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 ScreenPosition: TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
	output.ScreenPosition = mul(viewPosition, Projection);

    output.Position = output.ScreenPosition;

    return output;
}

float4 manualSample(sampler Sampler, float2 UV, float2 textureSize)
{
	float2 texelSize = 1.0f / textureSize;
	float2 texelpos = textureSize * UV;
	float2 lerps = frac(texelpos);

	float4 sourcevals[4];
	sourcevals[0] = tex2D(Sampler, UV);
	sourcevals[1] = tex2D(Sampler, UV + float2(texelSize.x, 0));
	sourcevals[2] = tex2D(Sampler, UV + float2(0, texelSize.y));
	sourcevals[3] = tex2D(Sampler, UV + float2(texelSize.x, texelSize.y));

	float4 interpolated = lerp(lerp(sourcevals[0], sourcevals[1], lerps.x), lerp(sourcevals[2], sourcevals[3], lerps.x ), lerps.y);
	return interpolated;
}


float4 Phong(float3 L, float Attenuation, float3 Position, float3 N, 
		float SpecularExponent)
{
	float3 R = normalize(reflect(-L, N));
	float3 E = normalize(CameraPosition - Position.xyz);
	float NL = dot(N, L);
	NL = (AmbientLight + NL * (1 - AmbientLight));
	float Specular = SpecularPower * pow(saturate(dot(R, E)), SpecularExponent);
	float3 Diffuse = Color.xyz * NL;

	return Attenuation * float4(Diffuse.rgb, Specular);
}

//Decoding of GBuffer Normals
float3 decode(float3 enc)
{
	return (2.0f * enc.xyz- 1.0f);
}

//Decode Color Vector to Float Value for shadowMap
float RGBADecode(float4 value)
{
	const float4 bits = float4(1.0 / (256.0 * 256.0 * 256.0),
	1.0 / (256.0 * 256.0),
	1.0 / 256.0,
	1);
	return dot(value.xyzw , bits);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	input.ScreenPosition.xy /= input.ScreenPosition.w;
	float2 UV = 0.5f * ((float2(input.ScreenPosition.x, -input.ScreenPosition.y) + 1) * UVMult + float2(1.0f / TextureSize.xy)) + UVOffset;

	float Depth = manualSample(GBuffer2, UV, TextureSize).x;

	float4 Position = 1.0f;
	Position.xy = input.ScreenPosition.xy;
	Position.z = Depth;
	Position = mul(Position, InverseViewProjection);
	Position /= Position.w;

	float3 L = (LightPosition.xyz - Position.xyz);
	float Attenuation = saturate(1.0f  - max(.01f, length(L)) / (LightDistance));
	

	if(Attenuation <= 0)
	{
		clip(-1);
		return float4(0,0,0,0);
	}
	else
	{
		half4 encodedNormal = tex2D(GBuffer1, UV);
		return Phong(normalize(L), Attenuation, Position.xyz, mul(decode(encodedNormal.xyz), InverseView), encodedNormal.w  * 100);
	}
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
