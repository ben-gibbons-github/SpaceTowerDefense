
float4 Color;

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

float4 PixelShaderFunction(VertexShaderOutput input): COLOR0
{
   return Color;
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
