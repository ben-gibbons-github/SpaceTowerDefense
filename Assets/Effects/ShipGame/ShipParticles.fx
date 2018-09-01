float4x4 View;
float4x4 Projection;
float2 TextureSize;

float Time;
float Duration = 1;
float RotateSpeed;

float StartSize = 1;
float EndSize = 1;

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
    float3 Velocity : NORMAL0;
    float4 Color : COLOR0;
    float2 TimeSize : TEXCOORD0;
};


// Vertex shader output structure specifies the position and color of the particle.
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinate : COLOR1;
};


// Vertex shader helper for computing the position of a particle.
float4 ComputeParticlePosition(float3 position, float3 velocity,
                               float age, float normalizedAge)
{
    float startVelocity = length(velocity);

    // Work out how fast the particle should be moving at the end of its life,
    // by applying a constant scaling factor to its starting velocity.
    float endVelocity = startVelocity;
    
    float velocityIntegral = startVelocity * normalizedAge +
                             (endVelocity - startVelocity) * normalizedAge *
                                                             normalizedAge / 2; 
    position += normalize(velocity) * velocityIntegral * Duration;
    
    // Apply the camera view and projection transforms.
    return mul(mul(float4(position, 1), View), Projection);
}


// Vertex shader helper for computing the rotation of a particle.
float2x2 ComputeParticleRotation(float age, float Size)
{   
    float rotation = RotateSpeed * (age + Size * 10);

    // Compute a 2x2 rotation matrix.
    float c = cos(rotation);
    float s = sin(rotation);
    
    return float2x2(c, -s, s, c);
}


// Custom vertex shader animates particles entirely on the GPU.
VertexShaderOutput ParticleVertexShader(VertexShaderInput input)
{
    VertexShaderOutput output;
    
    // Compute the age of the particle.
    float age = Time - input.TimeSize.x;
    
    // Normalize the age into the range zero to one.
    float normalizedAge = saturate(age / Duration);

    // Compute the particle position, size, color, and rotation.
    output.Position = ComputeParticlePosition(input.Position, input.Velocity,
                                              age, normalizedAge);

    float2x2 rotation = ComputeParticleRotation(age, input.TimeSize.y);

    output.Position.xy += mul(input.Corner, rotation) * TextureSize * input.TimeSize.y * lerp(StartSize, EndSize, normalizedAge);
    
    output.Color = input.Color * (1 - normalizedAge) * min(1, normalizedAge * 10);
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
