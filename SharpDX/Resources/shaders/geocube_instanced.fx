// Solid

float4x4 matVP;
float3 sunDir;


struct VS_IN {
	float4 pos	: POSITION0;
	float3 wPos : POSITION1;
	float2 tex	: TEXCOORD;
	float3 nor	: NORMAL;
	float4 col	: COLOR;
};

struct PS_IN {
	float4 pos	: SV_POSITION;
	float3 nor	: NORMAL;
	float2 tex	: TEXCOORD;
	float4 col	: COLOR;
};


PS_IN VS(VS_IN input) {
	PS_IN output = (PS_IN)0;

	float4 pos = input.pos;
	pos.xyz += input.wPos;
	output.pos = mul(pos, matVP);
	output.tex = input.tex;
	output.nor = input.nor;
	output.col = input.col;

	return output;
}

float4 PS(PS_IN input) : SV_Target {
	float3 n = normalize(input.nor);
	float lit = saturate(dot(n, sunDir));

	float4 output = 0;
	output.rgb = (0.12 + 0.8*lit) * input.col.rgb;
	output.a = input.col.a;
	return output;
}