float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 Rotation;
float3 CameraPosition;

texture Texture;
sampler2D textureSampler = sampler_state {
Texture = (Texture);
MinFilter = Linear;
MagFilter = Linear;
MIPFILTER = LINEAR;
ADDRESSU = WRAP;
ADDRESSV = WRAP;
};

float4 DrawColor = float4(1,1,1,1);
float4 SpecularColor = float4(1,1,1,1);
float SpecularExponent = 0.3;

float4 AmbientLightColor = float4(1,1,1,1);
float4 LightOneColor;
float4 LightTwoColor;
float3 LightOneDirection;
float3 LightTwoDirection;

//Forward Technique

struct ForwardVertexShaderInput
{
	float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
	float3 Normal : NORMAL0;
};

struct ForwardVertexShaderOutput
{
	float4 Position : POSITION0;
	float2 UV : TEXCOORD0;
	float4 Color : TEXCOORD1;
};

struct InstancedVertexShaderInput
{
	float4x4 World : BLENDWEIGHT;
	float4 Color : COLOR0;
};

ForwardVertexShaderOutput ForwardVertexShaderFunction(ForwardVertexShaderInput input)
{
	ForwardVertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.UV = input.UV;

	float3 worldNormal = mul(input.Normal, Rotation);

	float3 ViewPos = normalize(worldPosition - CameraPosition);

	output.Color = 
		AmbientLightColor +
			//Light One
		(LightOneColor * (max(dot(worldNormal, LightOneDirection), 0) + 
		SpecularColor * pow(dot(reflect(LightOneDirection, worldNormal), ViewPos), SpecularExponent))) +
			//Light Two
		(LightTwoColor * (max(dot(worldNormal, LightTwoDirection), 0) + 
		SpecularColor * pow(dot(reflect(LightTwoDirection, worldNormal), ViewPos), SpecularExponent))) +
		float4(0, 0, 0, 1);
	
	return output;
}

ForwardVertexShaderOutput ForwardInstancedVertexShaderFunction(ForwardVertexShaderInput input,
	InstancedVertexShaderInput input2)
{
	ForwardVertexShaderOutput output;

	float4 worldPosition = mul(input.Position, mul(World, transpose(input2.World)));
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.UV = input.UV;

	float3 ViewPos = normalize(worldPosition - CameraPosition);

	float3 worldNormal = input.Normal;

	output.Color = 
		(AmbientLightColor +
			//Light One
		(LightOneColor * (max(dot(worldNormal, LightOneDirection), 0) + 
		SpecularColor * pow(dot(reflect(LightOneDirection, worldNormal), ViewPos), SpecularExponent))) +
			//Light Two
		(LightTwoColor * (max(dot(worldNormal, LightTwoDirection), 0) + 
		SpecularColor * pow(dot(reflect(LightTwoDirection, worldNormal), ViewPos), SpecularExponent))) +
		float4(0, 0, 0, 1)) * input2.Color;
	
	return output;
}

float4 ForwardPixelShaderFunction(ForwardVertexShaderOutput input) : COLOR0
{
	if (input.Color.a < 0.1)
		clip(-1);

	return DrawColor * tex2D(textureSampler, input.UV) * input.Color;
}

technique ForwardTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 ForwardVertexShaderFunction();
		PixelShader = compile ps_2_0 ForwardPixelShaderFunction();
	}
}

technique ForwardInstancedTechnique
{
	pass Pass1
	{
		cullmode = none;
		VertexShader = compile vs_3_0 ForwardInstancedVertexShaderFunction();
		PixelShader = compile ps_3_0 ForwardPixelShaderFunction();
	}
}

//end