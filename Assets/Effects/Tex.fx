float4x4 World;
float4x4 View;
float4x4 Projection;

	texture Texture;
	sampler2D textureSampler = sampler_state {
		Texture = (Texture);
		MinFilter = Linear;
		MagFilter = Linear;
		MIPFILTER = LINEAR;
		ADDRESSU = WRAP;
		ADDRESSV = WRAP;
	};

	float4 DrawColor = float4(1,1,1,1);

//Forward Technique

	struct ForwardVertexShaderInput
	{
		float4 Position : POSITION0;
		float2 UV : TEXCOORD0;
	};

	struct ForwardVertexShaderOutput
	{
		float4 Position : POSITION0;
		float2 UV : TEXCOORD0;
	};

	ForwardVertexShaderOutput ForwardVertexShaderFunction(ForwardVertexShaderInput input)
	{
		ForwardVertexShaderOutput output;

		float4 worldPosition = mul(input.Position, World);
		float4 viewPosition = mul(worldPosition, View);
		output.Position = mul(viewPosition, Projection);

		output.UV = input.UV;

		return output;
	}

	ForwardVertexShaderOutput ForwardInstancedVertexShaderFunction(ForwardVertexShaderInput input,
		float4x4 instanceTransform : BLENDWEIGHT)
	{
		ForwardVertexShaderOutput output;

		float4 worldPosition = mul(input.Position, mul(World, transpose(instanceTransform)));
		float4 viewPosition = mul(worldPosition, View);
		output.Position = mul(viewPosition, Projection);

		output.UV = input.UV;

		return output;
	}

	float4 ForwardPixelShaderFunction(ForwardVertexShaderOutput input) : COLOR0
	{
		float4 TextureColor = tex2D(textureSampler, input.UV);

		return DrawColor * TextureColor;
	}

	technique ForwardTechnique
	{
		pass Pass1
		{
			VertexShader = compile vs_2_0 ForwardVertexShaderFunction();
			PixelShader = compile ps_2_0 ForwardPixelShaderFunction();
		}
	}

	technique ForwardInstancedTechnique
	{
		pass Pass1
		{
			VertexShader = compile vs_3_0 ForwardInstancedVertexShaderFunction();
			PixelShader = compile ps_3_0 ForwardPixelShaderFunction();
		}
	}

	//end