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
float EdgeSoftening = 4;

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

ForwardVertexShaderOutput ForwardVertexShaderFunction(ForwardVertexShaderInput input)
{
	ForwardVertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.UV = input.UV;

	float3 worldNormal = mul(input.Normal, Rotation);

	float3 ViewPos = normalize(input.Position - CameraPosition);

	/*
	output.Color = 
		(AmbientLightColor +
			//Light One
		(LightOneColor * (max(dot(worldNormal, LightOneDirection), 0) + 
		SpecularColor * pow(dot(reflect(LightOneDirection, worldNormal), ViewPos), SpecularExponent))) +
			//Light Two
		(LightTwoColor * (max(dot(worldNormal, LightTwoDirection), 0) + 
		SpecularColor * pow(dot(reflect(LightTwoDirection, worldNormal), ViewPos), SpecularExponent))) +
		float4(0, 0, 0, 1)) *
		min(1, abs(input.Position.x / 100)) * min(1, abs(input.Position.y / 100));
	*/

	float d = abs(dot(-ViewPos, worldNormal));
	output.Color = 	float4(1, 1, 1, 0) * (1 - d) * min(1, d * EdgeSoftening) + float4(0, 0, 0, 1);
	
	return output;
}

float4 ForwardPixelShaderFunction(ForwardVertexShaderOutput input) : COLOR0
{
	return DrawColor * input.Color * tex2D(textureSampler, input.UV);
}

technique ForwardTechnique
{
	pass Pass1
	{
		cullmode = none;
		VertexShader = compile vs_2_0 ForwardVertexShaderFunction();
		PixelShader = compile ps_2_0 ForwardPixelShaderFunction();
	}
}

//end