float4x4 View;
float4x4 Projection;
float2 TextureSize;
float Time;

float Duration = 1;
float DurationRandomnes = 1;
float3 Gravity;
float EndVelocity;
float4 MinColor = float4(0.8,0.8,0.8,1);
float4 MaxColor = float4(1,1,1,1);
float4 MultColor = float4(1,1,1,1);

float2 RotateSpeed;
float2 StartSize = float2(100,100);
float2 EndSize = float2(100,100);

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
    float4 Random : COLOR0;
    float Time : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinate : COLOR1;
};


float4 ComputeParticlePosition(float3 position, float3 velocity,
                               float age, float normalizedAge)
{
    float startVelocity = length(velocity);
    float endVelocity = startVelocity * EndVelocity;
    
    float velocityIntegral = startVelocity * normalizedAge +
                             (endVelocity - startVelocity) * normalizedAge *
                                                             normalizedAge / 2;
    position += normalize(velocity) * velocityIntegral * Duration;
    position += Gravity * age * normalizedAge;
    
    return float4(position, 1);
}


// Vertex shader helper for computing the size of a particle.
float ComputeParticleSize(float randomValue, float normalizedAge)
{
    // Apply a random factor to make each particle a slightly different size.
    float startSize = lerp(StartSize.x, StartSize.y, randomValue);
    float endSize = lerp(EndSize.x, EndSize.y, randomValue);
    
    // Compute the actual size based on the age of the particle.
    float size = lerp(startSize, endSize, normalizedAge);
    
    // Project the size into screen coordinates.
    return size;
}


// Vertex shader helper for computing the color of a particle.
float4 ComputeParticleColor(float4 projectedPosition,
                            float randomValue, float normalizedAge)
{
    // Apply a random factor to make each particle a slightly different color.
    float4 color = lerp(MinColor, MaxColor, randomValue);
    
    // Fade the alpha based on the age of the particle. This curve is hard coded
    // to make the particle fade in fairly quickly, then fade out more slowly:
    // plot x*(1-x)*(1-x) for x=0:1 in a graphing program if you want to see what
    // this looks like. The 6.7 scaling factor normalizes the curve so the alpha
    // will reach all the way up to fully solid.
    
    color.a *= normalizedAge * (1-normalizedAge) * (1-normalizedAge) * 6.7;
   
    return color;
}


// Vertex shader helper for computing the rotation of a particle.
float2x2 ComputeParticleRotation(float randomValue, float age)
{    
    // Apply a random factor to make each particle rotate at a different speed.
    float rotateSpeed = lerp(RotateSpeed.x, RotateSpeed.y, randomValue);
    
    float rotation = rotateSpeed * age;

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
    float age = Time - input.Time;
    
    // Apply a random factor to make different particles age at different rates.
    age *= 1 + input.Random.x * DurationRandomnes;
    
    // Normalize the age into the range zero to one.
    float normalizedAge = saturate(age / Duration);

    // Compute the particle position, size, color, and rotation.
    output.Position = ComputeParticlePosition(input.Position, input.Velocity,
                                              age, normalizedAge);

    float size = ComputeParticleSize(input.Random.y, normalizedAge);
    float2x2 rotation = ComputeParticleRotation(input.Random.w, age);

    output.Position.xy += mul(input.Corner, rotation) * size * TextureSize;
    
    output.Color = ComputeParticleColor(output.Position, input.Random.z, normalizedAge);
    output.TextureCoordinate = (input.Corner + 1) / 2;
    
    return output;
}


// Pixel shader for drawing particles.
float4 ParticlePixelShader(VertexShaderOutput input) : COLOR0
{
    return tex2D(Sampler, input.TextureCoordinate) * input.Color;
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
