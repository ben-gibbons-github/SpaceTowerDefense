float4x4 World;
float4x4 View;
float4x4 Projection;

Texture ShadowReference; 
samplerCUBE ShadowSampler = sampler_state 
{ 
   texture = <ShadowReference> ; 
   magfilter = POINT; 
   minfilter = POINT; 
   mipfilter = POINT; 
   AddressU = WRAP; 
   AddressV = WRAP; 
}; 
struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 N : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	output.N = normalize(input.Position);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return texCUBE(ShadowSampler, input.N);
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.
		cullmode = none;
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
