float4x4 World;
float4x4 View;
float4x4 Projection;

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

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 UV: TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 UV: TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.UV = input.UV;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.

    return tex2D(textureSampler, input.UV) * DrawColor;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.
		cullmode = none;
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
