float2 TextureSize;
sampler GBuffer0 : register(s0);
sampler GBuffer1 : register(s1);
sampler GBuffer2 : register(s2);


struct VertexShaderInput
{
    float3 Position : POSITION0;
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

    output.Position = float4(input.Position,1);
	output.UV = input.UV - float2(1.0f / TextureSize.xy);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	return float4(tex2D(GBuffer0, input.UV).w,tex2D(GBuffer1, input.UV).w,0.5,1);
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
