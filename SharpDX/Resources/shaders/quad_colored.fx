// Quad-Colored

float2 position;
float2 size;
float4 color;


struct VS_IN {
	float4 pos	: POSITION;
};

struct PS_IN {
	float4 pos	: SV_POSITION;
};


PS_IN VS(VS_IN input) {
	PS_IN output = (PS_IN)0;

	output.pos.xy = (position + input.pos.xy * size) * 2 - 1;
	output.pos.zw = input.pos.zw;
	output.pos.y = -output.pos.y;

	return output;
}

float4 PS() : SV_Target {
	return color;
}