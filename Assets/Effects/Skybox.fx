float4x4 World;
float4x4 View;
float4x4 Projection;

TextureCube SkyBoxTexture; 
float3 CameraPosition;

float ColorIntensity = 1;
float Contrast = 1;
float4 AmbientColor = float4(1,1,1,1);
float4 MultColor = float4(1,1,1,1);

samplerCUBE SkyBoxSampler = sampler_state 
{ 
   texture = <SkyBoxTexture>; 
   magfilter = LINEAR; 
   minfilter = LINEAR; 
   mipfilter = LINEAR; 
   AddressU = Mirror; 
   AddressV = Mirror; 
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float3 TextureCoordinate : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

	  float4 VertexPosition = mul(input.Position, World);
    output.TextureCoordinate = VertexPosition - CameraPosition;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 col = texCUBE(SkyBoxSampler, normalize(input.TextureCoordinate));
	float BW = col.x+col.y+col.z /3;
	col = col* ColorIntensity + BW * (1- ColorIntensity);
	col *= ((BW - 0.5) * Contrast) + 0.5;
	return col*MultColor + AmbientColor;
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
