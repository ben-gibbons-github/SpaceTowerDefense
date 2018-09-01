
sampler Texture : register(s0);

float Displacment = 0;
float ViewWidth = 1280;

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0, float4 color : COLOR0): COLOR0
{
	if (Displacment == 0)
		return tex2D(Texture, texCoord) * color;
	else
	{
		float4 rColor = tex2D(Texture, float2(texCoord.x + Displacment / ViewWidth, texCoord.y));
		float4 gColor = tex2D(Texture, float2(texCoord.x - Displacment / ViewWidth, texCoord.y));
		float4 bColor = tex2D(Texture, texCoord);
		return float4(rColor.r, gColor.g, bColor.b, (rColor.a + gColor.a + bColor.a) / 3) * color;
	}
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
