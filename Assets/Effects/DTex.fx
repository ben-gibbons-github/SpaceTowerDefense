//Fields:
	float4x4 World;
	float4x4 View;
	float4x4 Projection;
	float4x4 WorldViewIT;

	
	texture Texture;
	sampler2D textureSampler = sampler_state {
		Texture = (Texture);
		MinFilter = Linear;
		MagFilter = Linear;
		MIPFILTER = LINEAR;
		ADDRESSU = WRAP;
		ADDRESSV = WRAP;
	};

	float SpecularPower = 1.5;
	float SpecularExponent = 0.25;
	float4 DrawColor = float4(1,1,1,1);

	float3 LightDistance;
	float3 LightPosition;

	float2 UVOffset = float2(0, 0);
	float2 UVMult = float2(0, 0);


	sampler LightTexture : register(s0);
//end



//DeferredTechnique

	struct GBufPixelShaderOutput
	{
		float4 Normal: COLOR0;
		float4 Depth: COLOR1;
	};


	half3 encode(half3 n)
	{
		n = normalize(n);
		n.xyz = 0.5f * (n.xyz + 1.0f);
		return n;
	}

	half3 decode(half4 enc)
	{
		return (2.0f * enc.xyz- 1.0f);
	}

	struct GBufVertexShaderInput
	{
		float4 Position : POSITION0;
		float3 Normal : NORMAL0;
	};

	struct GBufVertexShaderOutput
	{
		float4 Position : POSITION0;
		float3 Depth : TEXCOORD0;
		float3 Normal : TEXCOORD1;
	};

	GBufVertexShaderOutput GBufVertexShaderFunction(GBufVertexShaderInput input)
	{
		GBufVertexShaderOutput output;

		float4 worldPosition = mul(input.Position, World);
		float4 viewPosition = mul(worldPosition, View);
		output.Position = mul(viewPosition, Projection);

		output.Depth.x = output.Position.z;
		output.Depth.y = output.Position.w;
		output.Depth.z = viewPosition.z;

		output.Normal = normalize(mul(input.Normal, (float3x3)WorldViewIT));

		return output;
	}

	GBufPixelShaderOutput GBufPixelShaderFunction(GBufVertexShaderOutput input)
	{
		GBufPixelShaderOutput output;
	
		output.Normal.xyz = encode(normalize(input.Normal));
		output.Normal.w = SpecularExponent;

		output.Depth = input.Depth.x / input.Depth.y;
		output.Depth.g = input.Depth.z;

		return output;
	}

	technique DeferredTechnique
	{
		pass Pass1
		{
			Cullmode= ccw;
			VertexShader = compile vs_3_0 GBufVertexShaderFunction();
			PixelShader = compile ps_3_0 GBufPixelShaderFunction();
		}
	}

//end


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
		float4 ScreenPosition : TEXCOORD1;
	};

	ForwardVertexShaderOutput ForwardVertexShaderFunction(ForwardVertexShaderInput input)
	{
		ForwardVertexShaderOutput output;

		float4 worldPosition = mul(input.Position, World);
		float4 viewPosition = mul(worldPosition, View);
		output.Position = mul(viewPosition, Projection);
		output.ScreenPosition = output.Position;

		output.UV = input.UV;

		return output;
	}

	float4 ForwardPixelShaderFunction(ForwardVertexShaderOutput input) : COLOR0
	{
	    float2 ScreenUV = 0.5f * (float2(input.ScreenPosition.x, -input.ScreenPosition.y) / input.ScreenPosition.w + 1) * UVMult + UVOffset;

		float4 TextureColor = tex2D(textureSampler, input.UV);
		float4 LightColor = tex2D(LightTexture, ScreenUV) * float4(6,6,6,1);

		return saturate(float4(LightColor.xyz,1) * (TextureColor * DrawColor + LightColor.w * SpecularPower));
		//return LightColor;
	}

	technique ForwardTechnique
	{
		pass Pass1
		{
			Cullmode= ccw;
			VertexShader = compile vs_3_0 ForwardVertexShaderFunction();
			PixelShader = compile ps_3_0 ForwardPixelShaderFunction();
		}
	}

	//end




	//Shadow Technique

	struct ShadowVertexShaderInput
	{
		float4 Position : POSITION0;
	};

	struct ShadowVertexShaderOutput
	{
		float4 Position : POSITION0;
		float Distance : TEXCOORD0;
	};

	ShadowVertexShaderOutput ShadowVertexShaderFunction(ShadowVertexShaderInput input)
	{
		ShadowVertexShaderOutput output;

		float4 worldPosition = mul(input.Position, World);
		float4 viewPosition = mul(worldPosition, View);
		output.Position = mul(viewPosition, Projection);
		output.Distance = distance(worldPosition,LightPosition) / LightDistance;

		return output;
	}

	float4 ShadowPixelShaderFunction(ShadowVertexShaderOutput input) : COLOR0
	{
		return float4(input.Distance,0,0,0);
	}

	technique ShadowTechnique
	{
		pass Pass1
		{
			Cullmode= ccw;
			VertexShader = compile vs_3_0 ShadowVertexShaderFunction();
			PixelShader = compile ps_3_0 ShadowPixelShaderFunction();
		}
	}

	//end