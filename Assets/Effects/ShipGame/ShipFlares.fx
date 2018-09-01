float4x4 View;
float4x4 Projection;
float2 TextureSize;

texture Texture;

sampler Sampler = sampler_state
{
    Texture = (Texture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    MipFilter = Point;
    
    AddressU = Clamp;
    AddressV = Clamp;
};


struct VertexShaderInput
{
    float2 Corner : POSITION0;
    float3 Position : POSITION1;
    float4 Color : COLOR0;
    float Size : TEXCOORD0;
};


// Vertex shader output structure specifies the position and color of the particle.
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinate : COLOR1;
};


// Custom vertex shader animates particles entirely on the GPU.
VertexShaderOutput ParticleVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;

    // Compute the particle position, size, color, and rotation.
    output.Position = mul(mul(float4(input.Position, 1), View), Projection);

    output.Position.xy += input.Corner * TextureSize * input.Size;
    
    output.Color = input.Color;
    output.TextureCoordinate = (input.Corner + 1) / 2;
    
    return output;
}


// Pixel shader for drawing particles.
float4 ParticlePixelShader(VertexShaderOutput input) : COLOR0
{
    return tex2D(Sampler, input.TextureCoordinate) * input.Color * 3;
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
