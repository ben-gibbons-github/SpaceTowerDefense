float4x4 World;
float4x4 View;
float4x4 Projection;
float DistortionScale = 10000;
float Time;

struct VertexShaderInput
{
   float4 Position : POSITION;
   float3 Normal : NORMAL;
};

struct VertexShaderOutput
{
   float4 Position : POSITION;
   float4 Displacement : TEXCOORD;
};

struct InstancedVertexShaderInput
{
	float4x4 World : BLENDWEIGHT;
	float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input,
	InstancedVertexShaderInput input2)
{
   VertexShaderOutput output;

   float4 worldPosition = mul(input.Position, mul(World, transpose(input2.World)));
   float4 viewPosition = mul(worldPosition, View);
   output.Position = mul(viewPosition, Projection);

   float3 normalWV = mul(input.Normal, mul(World, View));
   normalWV.y = -normalWV.y;
   
   float amount = dot(normalWV, float3(0,0,1)) * DistortionScale;
   output.Displacement = float4(float2(0.5, 0.5) + amount * normalWV.xy, 0.5, 1);

   return output;   
}

float4 PixelShaderFunction(float4 displacement : TEXCOORD) : COLOR
{  
   return displacement;
}

technique PullIn
{
    pass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
