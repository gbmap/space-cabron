#ifndef FX_UTILS
#define FX_UTILS

#include "noise.cginc"

float3 rgb_to_hsv_no_clip(float3 RGB)
{
	float3 HSV;

	float minChannel, maxChannel;
	if (RGB.x > RGB.y) {
		maxChannel = RGB.x;
		minChannel = RGB.y;
	}
	else {
		maxChannel = RGB.y;
		minChannel = RGB.x;
	}

	if (RGB.z > maxChannel) maxChannel = RGB.z;
	if (RGB.z < minChannel) minChannel = RGB.z;

	HSV.xy = 0;
	HSV.z = maxChannel;
	float delta = maxChannel - minChannel;             //Delta RGB value
	if (delta != 0) {                    // If gray, leave H  S at zero
		HSV.y = delta / HSV.z;
		float3 delRGB;
		delRGB = (HSV.zzz - RGB + 3 * delta) / (6.0*delta);
		if (RGB.x == HSV.z) HSV.x = delRGB.z - delRGB.y;
		else if (RGB.y == HSV.z) HSV.x = (1.0 / 3.0) + delRGB.x - delRGB.z;
		else if (RGB.z == HSV.z) HSV.x = (2.0 / 3.0) + delRGB.y - delRGB.x;
	}
	return (HSV);
}

float3 hsv_to_rgb(float3 HSV)
{
	float3 RGB = HSV.z;

	float var_h = HSV.x * 6;
	float var_i = floor(var_h);   // Or ... var_i = floor( var_h )
	float var_1 = HSV.z * (1.0 - HSV.y);
	float var_2 = HSV.z * (1.0 - HSV.y * (var_h - var_i));
	float var_3 = HSV.z * (1.0 - HSV.y * (1 - (var_h - var_i)));
	if (var_i == 0) { RGB = float3(HSV.z, var_3, var_1); }
	else if (var_i == 1) { RGB = float3(var_2, HSV.z, var_1); }
	else if (var_i == 2) { RGB = float3(var_1, HSV.z, var_3); }
	else if (var_i == 3) { RGB = float3(var_1, var_2, HSV.z); }
	else if (var_i == 4) { RGB = float3(var_3, var_1, HSV.z); }
	else { RGB = float3(HSV.z, var_1, var_2); }

	return (RGB);
}


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

