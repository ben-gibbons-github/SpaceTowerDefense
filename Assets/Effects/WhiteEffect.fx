float4x4 World;
float4x4 View;
float4x4 Projection;

// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    return output;
}

VertexShaderOutput InstancedVertexShaderFunction(VertexShaderInput input,
	float4x4 instanceTransform : BLENDWEIGHT)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, mul(World, transpose(instanceTransform)));
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return float4(1,1,1,1);
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}

technique ForwardInstancedTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 InstancedVertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}

