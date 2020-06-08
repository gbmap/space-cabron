#ifndef FX_UTILS
#define FX_UTILS

#include "noise.cginc"

float2 spawnfxuv(fixed2 uv, float t)
{
	float s = pow(t, 0.25);
	float ofx = (PerlinNoise2D(30.0*uv + float2(_Time.x*10.0, _Time.x*100.0)))*2.0;
	ofx += (PerlinNoise2D(3.0*uv + float2(_Time.x*10.0, _Time.x*50.0)))*10.0;
	float ofy = 0.0;
	ofy = (PerlinNoise2D(3.0*uv + float2(_Time.x*10.0, _Time.x*50.0))) * 10.0;
	float2 offset = float2(ofx, ofy);
	return offset * s * 0.05;
}

fixed3 spawnfx(fixed2 uv)
{
	return PerlinNoise2D((uv + _Time)*300.0)*10.0;
}

#endif

