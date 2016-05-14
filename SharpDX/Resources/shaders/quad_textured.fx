// Quad-Colored

Texture2D Texture : register(t0);
SamplerState Sampler : register(s0);

float2 position;
float2 size;


struct VS_IN {
	float4 pos	: POSITION;
	float2 tex	: TEXCOORD0;
};

struct PS_IN {
	float4 pos	: SV_POSITION;
	float2 tex	: TEXCOORD0;
};


PS_IN VS(VS_IN input) {
	PS_IN output = (PS_IN)0;

	output.pos.xy = (position + input.pos.xy * size) * 2 - 1;
	output.pos.zw = input.pos.zw;
	output.pos.y = -output.pos.y;

	output.tex = input.tex;

	return output;
}

float4 PS(PS_IN input) : SV_Target {
	return Texture.Sample(Sampler, input.tex);
}