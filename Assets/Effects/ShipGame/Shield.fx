float4x4 World;
float4x4 View;
float4x4 Projection;
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

ForwardVertexShaderOutput ForwardInstancedVertexShaderFunction(ForwardVertexShaderInput input,
	InstancedVertexShaderInput input2)
{
	ForwardVertexShaderOutput output;

	float4 worldPosition = mul(input.Position, mul(World, transpose(input2.World)));
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

	output.UV = input.UV;

	float3 ViewPos = normalize(worldPosition - CameraPosition);

	output.Color = input2.Color * (1 - abs(dot(ViewPos, input.Normal))) * 0.8 + max(0, 1 - worldPosition.y / 100) / 3;
	
	return output;
}

float4 ForwardPixelShaderFunction(ForwardVertexShaderOutput input) : COLOR0
{
	float4 Texture = tex2D(textureSampler, input.UV); 
	return saturate((1 + Texture.x) * input.Color / 2);
}

technique ForwardTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 ForwardInstancedVertexShaderFunction();
		PixelShader = compile ps_2_0 ForwardPixelShaderFunction();
	}
}

technique ForwardInstancedTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 ForwardInstancedVertexShaderFunction();
		PixelShader = compile ps_3_0 ForwardPixelShaderFunction();
	}
}

//end