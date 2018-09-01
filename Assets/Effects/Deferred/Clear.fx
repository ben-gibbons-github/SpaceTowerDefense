
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
    output.Position = input.Position;
    return output;
}

struct PixelShaderOutput
{
	float4 Normal: COLOR0;
	float4 Depth: COLOR1;
};

PixelShaderOutput PixelShaderFunction(VertexShaderOutput input)
{
    PixelShaderOutput output;

	output.Normal.rgb = 0.5f;
	output.Normal.a = 1;

	output.Depth = 1.0f;

	return output;
}

technique Technique1
{
    pass Pass1
    {
		cullmode = none;
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
