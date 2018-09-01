
sampler Texture : register(s0);

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 texCoord: TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 texCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = input.Position;
	output.texCoord = input.texCoord;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input): COLOR0
{
   return tex2D(Texture,input.texCoord);
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
