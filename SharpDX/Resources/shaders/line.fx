// Line

float4x4 matWVP;
float4 color;


struct VS_IN {
	float4 pos	: POSITION;
};

struct PS_IN {
	float4 pos	: SV_POSITION;
};


PS_IN VS(VS_IN input) {
	PS_IN output = (PS_IN)0;

	output.pos = mul(input.pos, matWVP);

	return output;
}

float4 PS() : SV_Target {
	return color;
}