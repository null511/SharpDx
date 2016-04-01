// Solid

float4x4 matWVP;
float4 color;
float3 sunDir;


struct VS_IN {
	float4 pos	: POSITION0;
	float2 tex	: TEXCOORD;
	float3 nor	: NORMAL;
};

struct PS_IN {
	float4 pos	: SV_POSITION;
	float3 nor	: NORMAL;
	float2 tex	: TEXCOORD;
};


PS_IN VS(VS_IN input) {
	PS_IN output = (PS_IN)0;

	output.pos = mul(input.pos, matWVP);
	output.tex = input.tex;
	output.nor = input.nor;

	return output;
}

float4 PS(PS_IN input) : SV_Target {
	float3 n = normalize(input.nor);
	float lit = saturate(dot(n, sunDir));

	float4 output = 0;
	output.rgb = (0.12 + 0.8*lit) * color.rgb;
	output.a = color.a;
	return output;
}