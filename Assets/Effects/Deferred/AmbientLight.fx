
float4 LightColor = float4(0.5,0.5,0.5,0);

struct VertexShaderInput
{
float3 Position : POSITION0;
};

struct VertexShaderOutput
{
float4 Position : POSITION0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = float4(input.Position,1);
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input): COLOR0
{
   return LightColor;
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
