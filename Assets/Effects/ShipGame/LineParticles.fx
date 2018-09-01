float4x4 View;
float4x4 Projection;

float Time;
float Duration = 1;

struct VertexShaderInput
{
    float3 Position : POSITION0;
    float Time : TEXCOORD0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

// Custom vertex shader animates particles entirely on the GPU.
VertexShaderOutput ParticleVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    // Compute the age of the particle.
    float age = Time - input.Time;
    
    // Normalize the age into the range zero to one.
    float normalizedAge = saturate(age / Duration);

    // Compute the particle position, size, color, and rotation.
    output.Position = mul(mul(float4(input.Position, 1), View), Projection);

    output.Color = (1 - normalizedAge) * min(1, normalizedAge * 10) * input.Color;
    
    return output;
}


// Pixel shader for drawing particles.
float4 ParticlePixelShader(VertexShaderOutput input) : COLOR0
{
    return input.Color;
}


// Effect technique for drawing particles.
technique Particles
{
    pass P0
    {
        VertexShader = compile vs_2_0 ParticleVertexShader();
        PixelShader = compile ps_2_0 ParticlePixelShader();
    }
}
