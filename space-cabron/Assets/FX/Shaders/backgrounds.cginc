#ifndef BACKGROUNDS__H
#define BACKGROUNDS__H

struct v2f
{
	float2 uv : TEXCOORD0;
	float4 vertex : SV_POSITION;
};

float _Beat;
float _LastNoteDuration;

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


float beat_curve()
{
	fixed tt = step(_Time.y - (_Beat + _LastNoteDuration - 0.1), 0.0);
	fixed t = saturate(_Time.y - _Beat);
	return 1.0 - t;
}

fixed4 bg01(v2f i, float2 uv) {
	uv.y += _Time.y *0.5;
	uv = frac(uv * 10);

	float a = step(frac(uv.y), 0.5) * step(uv.x, 0.5);

	fixed4 clr = fixed4(sin(a + _Time.y), cos(a + _Time.y * 0.5) * 0.5 + sin(a*0.5*beat_curve()), 0.3f, 1.0f);
	//fixed4 clr = fixed4(a, a, a, 1.0);

	fixed3 hsv = rgb_to_hsv_no_clip(clr.rgb);
	hsv.r = frac(_Time.y + i.uv.y);
	//hsv.g -= 0.5;
	clr.rgb = hsv_to_rgb(hsv);
	clr.rgb += beat_curve()*0.25;
	return clr;
}

fixed4 bg02(v2f i, float2 uv) {

	uv.y += _Time.x * 10.0;
	uv = frac(abs(uv*5.0));
	float a = frac(beat_curve() + length((uv*uv)*5.0));

	fixed4 clr = fixed4(abs(sin(a)), cos(_Time.y), beat_curve(), 1.0);

	fixed3 hsv = rgb_to_hsv_no_clip(clr.rgb);
	hsv.r = frac(a + beat_curve() + _Time.y);
	hsv.g -= 0.5;
	clr.rgb = hsv_to_rgb(hsv);


	return clr;
}

fixed4 bg03(v2f i, float2 uv) {
	uv.y += _Time.y*.5;
	uv = frac(abs(uv*1.0));
	float a = frac(beat_curve() + length((uv*uv.x*uv.y)*10.0));
	a += sin(uv.x*20.0);


	fixed4 clr = fixed4(frac(a), cos(_Time.y), 0.0, 1.0);
	fixed3 hsv = rgb_to_hsv_no_clip(clr.rgb);
	hsv.r = frac(a+beat_curve()+_Time.y);
	hsv.g -= 0.3;
	clr.rgb = hsv_to_rgb(hsv);

	return clr;
}


#define PI_TWO			1.570796326794897
#define PI				3.141592653589793
#define TWO_PI			6.283185307179586
#define E               2.7182818284

float2 complex_exp(float2 z)
{
    // e^x * cos(y) + i*e^x*sin(y)
    float ex = pow(E, z.x);
    return float2(
        ex * cos(z.y),
        ex * sin(z.y)
    );
}

float2 tile(float2 uv, float t) {
    return frac(uv * t);
}

float2 remap_to0(float2 uv) {
    return uv*2.-1.;
}

float2 complex_map(float2 uv, float factor, float2 c)
{
    return complex_exp(remap_to0(uv)*factor) + complex_exp(c);
}

float2 to_polar(float2 uv) {
    return float2(
        length(uv),
        atan2(uv.y, uv.x)
    );
}

float grid(float2 uv, float t) {
    float2 ss = step(0.05, frac(uv*t));
    return 1.-(ss.x*ss.y);
}

float3 pattern_knots(float2 uv, float2 c, float f) {
    float2 p = complex_map(uv, f, c);
    float t = pow(abs(p.x), abs(p.y));

    float gridt = grid(p, f);
    float3 gridc = float3(gridt, gridt, gridt);
    return float3(p, t);
}

float3 pattern_lights(float2 uv, float2 c, float zoom) {
    float2 p = tile(uv, zoom);
    p = abs(remap_to0(p));
    p = complex_exp(p) + complex_exp(c);

    float gridt = grid(p, zoom);
    return float3(p, pow(abs(p.x), abs(p.y)));
}



fixed4 bg04(v2f i, float2 uv) {
   // float t = mod(u_time,10.0);
    float t = sin(frac(_Time.y/100.0))*100.0;
    uv.y += t*.5;
	uv = frac(uv);
    // uv.y = frac(uv.y);
    // uv.x *= max(u_resolution.x, u_resolution.y) / u_resolution.y;

    // uv = to_polar(to_polar(to_polar(uv)));

    float f = 1.0;
    float3 c = pattern_lights(uv, float2(cos(t), sin(t)), f);

    float3 clr = lerp(
        float3(sin(t*c.z), 0.6667, 0.302),
        float3(0.6078, cos(t*c.y), 0.3333),
        c.x*c.y
    );

    float3 cr = clr;
	float3 hsv = rgb_to_hsv_no_clip(cr);
	hsv.r = frac(hsv.r + _Time.y);
	hsv.g = 0.65;
	hsv.b = .15*beat_curve();
	// hsv.g -= 0.3;
	cr.rgb = hsv_to_rgb(hsv);
    return fixed4(cr, 1.0);
}


#endif