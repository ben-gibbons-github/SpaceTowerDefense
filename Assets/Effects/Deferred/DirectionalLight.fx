
float4x4 InverseViewProjection;
float4x4 InverseView;
float3 CameraPosition;

float AmbientLight =0.5;
float SpecularPower = 0.5;
float3 Direction = float3(0.5,-0.75,0.5);
float4 LightColor = float4(0.5,0.5,0.5,1);

float2 TextureSize;
sampler GBuffer1 : register(s0);
sampler GBuffer2 : register(s1);

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
	output.UV = input.UV + float2(1.0f / TextureSize.xy);
    return output;
}


float4 manualSample(sampler Sampler, float2 UV, float2 textureSize)
{
	float2 texelpos = textureSize * UV;
	float2 lerps = frac(texelpos);
	float texelSize = 1.0 / textureSize;
	float4 sourcevals[4];
	sourcevals[0] = tex2D(Sampler, UV);
	sourcevals[1] = tex2D(Sampler, UV + float2(texelSize, 0));
	sourcevals[2] = tex2D(Sampler, UV + float2(0, texelSize));
	sourcevals[3] = tex2D(Sampler, UV + float2(texelSize, texelSize));
	float4 interpolated = lerp(lerp(sourcevals[0], sourcevals[1], lerps.x), lerp(sourcevals[2], sourcevals[3], lerps.x ), lerps.y);
	return interpolated;
}

float4 Phong(float3 Position, float3 N, float SpecularExponent)
{
	float3 R = normalize(reflect(Direction, N));
	float3 E = normalize(CameraPosition - Position.xyz);
	float NL = dot(N, -Direction);
	NL = (AmbientLight + NL * (1 - AmbientLight));
	float Specular = pow(saturate(dot(R, E)), SpecularExponent);
	float3 Diffuse = LightColor.xyz * NL;

	return float4(Diffuse.rgb, Specular * SpecularPower);
}

float3 decode(float3 enc)
{
	return (2.0f * enc.xyz- 1.0f);
}


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	half4 encodedNormal = tex2D(GBuffer1, input.UV);
	half3 Normal = mul(decode(encodedNormal.xyz), InverseView);

	float Depth = manualSample(GBuffer2, input.UV, TextureSize).x;
	float4 Position = 1.0f;

	Position.x = input.UV.x * 2.0f - 1.0f;
	Position.y = -(input.UV.y * 2.0f - 1.0f);
	Position.z = Depth;
	Position = mul(Position, InverseViewProjection);
	Position /= Position.w;

	return Phong(Position.xyz, Normal,  encodedNormal.w * 100);
}

technique Technique1
{
    pass Pass1
	{
		cullmode =none;
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
