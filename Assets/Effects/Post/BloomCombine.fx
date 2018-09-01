// Pixel shader combines the bloom image with the original
// scene, using tweakable intensity levels and saturation.
// This is the final step in applying a bloom postprocess.

sampler BloomSampler : register(s0);
sampler BaseSampler : register(s1);
sampler DistortionMap : register(s2);

const float ZeroOffset = 0.5f / 255.0f;

float BloomIntensity;
float BaseIntensity;

float BloomSaturation;
float BaseSaturation;

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

// Helper for modifying the saturation of a color.
float4 AdjustSaturation(float4 color, float saturation)
{
    // The constants 0.3, 0.59, and 0.11 are chosen because the
    // human eye is more sensitive to green light, and less to blue.
    float grey = dot(color, float3(0.3, 0.59, 0.11));

    return lerp(grey, color, saturation);
}


float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    // Look up the bloom and original base image colors.
    float2 displacement = tex2D(DistortionMap, texCoord).rg - float2(0.5f, 0.5f);
	
    float4 bloom;
    float4 base;

    if ((displacement.x == 0) && (displacement.y == 0))
    {
		bloom = tex2D(BloomSampler, texCoord);
		base = tex2D(BaseSampler, texCoord);
    }
	else
	{
		bloom = tex2D(BloomSampler, texCoord + displacement);
		base = tex2D(BaseSampler, texCoord + displacement);
    }

    // Adjust color saturation and intensity.
    bloom = AdjustSaturation(bloom, BloomSaturation) * BloomIntensity;
    base = AdjustSaturation(base, BaseSaturation) * BaseIntensity;
    
    // Darken down the base image in areas where there is a lot of bloom,
    // to prevent things looking excessively burned-out.
    base *= (1 - saturate(bloom));
    
    // Combine the two images.
    return base + bloom;
}


technique BloomCombine
{
    pass Pass1
    {
		cullmode = none;
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
