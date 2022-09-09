#ifndef BACKGROUNDS__H
#define BACKGROUNDS__H

//	<https://www.shadertoy.com/view/4dS3Wd>
//	By Morgan McGuire @morgan3d, http://graphicscodex.com
//
float hash(float n) { return frac(sin(n) * 1e4); }
float hash(float2 p) { return frac(1e4 * sin(17.0 * p.x + p.y * 0.1) * (0.1 + abs(sin(p.y * 13.0 + p.x)))); }

float noise(float x) {
	float i = floor(x);
	float f = frac(x);
	float u = f * f * (3.0 - 2.0 * f);
	return lerp(hash(i), hash(i + 1.0), u);
}

float noise(float2 x) {
	float2 i = floor(x);
	float2 f = frac(x);

	// Four corners in 2D of a tile
	float a = hash(i);
	float b = hash(i + float2(1.0, 0.0));
	float c = hash(i + float2(0.0, 1.0));
	float d = hash(i + float2(1.0, 1.0));

	// Simple 2D lerp using smoothstep envelope between the values.
	// return float3(lerp(lerp(a, b, smoothstep(0.0, 1.0, f.x)),
	//			lerp(c, d, smoothstep(0.0, 1.0, f.x)),
	//			smoothstep(0.0, 1.0, f.y)));

	// Same code, with the clamps in smoothstep and common subexpressions
	// optimized away.
	float2 u = f * f * (3.0 - 2.0 * f);
	return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}

struct v2f
{
	float2 uv : TEXCOORD0;
	float4 vertex : SV_POSITION;
	float4 worldPos : TEXCOORD1;
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

fixed4 post_process_bg(fixed4 clr)
{
	fixed3 hsv = rgb_to_hsv_no_clip(clr);
	hsv.g = 0.65;
	hsv.b = .15*beat_curve();
	clr.rgb = saturate(hsv_to_rgb(hsv))*.0625;
	return clr;
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
	clr.rgb = saturate(hsv_to_rgb(hsv))*.25;
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
	clr.rgb = saturate(hsv_to_rgb(hsv))*0.25;


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
	clr.rgb = saturate(hsv_to_rgb(hsv))*.2;

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
	// hsv.g = 0.65;
	// hsv.b = .15*beat_curve();
	// hsv.g -= 0.3;
	cr.rgb = saturate(hsv_to_rgb(hsv));
    return fixed4(cr*.15, 1.0);
}

// src : https://thebookofshaders.com/13/
#define OCTAVES 8
float fbm (float2 st) {
    // Initial values
    float value = 0.0;
    float amplitude = .5;
    float frequency = 0.;

	float t = .5;

    //
    // Loop of octaves
    for (int i = 0; i < OCTAVES; i++) {
        value += amplitude * noise(st+st*25.0*i*0.00125+_Time.y*cos(i*3.14159268*0.33)*0.25*i);
		float a = 0.5;
		st = mul(float2x2(
			cos(a), -sin(a),
			sin(a), cos(a)
		),st*1.5+float2(0.0, -10.0));
        amplitude *= .5;
		t += amplitude;
    }
	// return value/OCTAVES;
    return value/t;
}

float draw_stars(float2 uv, float2 vel) {
	uv *= 10.;
	uv += vel*.1;
	float2 id = floor(uv);

	float xoffset = noise(id);
	float yoffset = noise(id*2.)-.5;

	uv = frac(uv)*2.-1.;
	float stars = saturate(1.-(length(uv - float2(xoffset,yoffset))+.7));
	float stars2 = saturate(1.-(length(uv - float2(xoffset,yoffset))+.5));
	stars2 *= noise(uv*5.+vel);
	stars = stars*stars * noise((uv*5.)+vel)*5.;
	return stars2*.15+stars;
}

fixed4 bg05(v2f i, float2 uv) {
	float t = _Time.y*4.;
	float x = fbm((uv-fixed2(0., -t*0.125))*5.)*0.750 + beat_curve()*0.05;
	x = smoothstep(0., 1., x*x*x);
	float x2 = fbm((uv-fixed2(10., -t*0.25))*5.)*0.750+beat_curve()*0.05;
	x2 = smoothstep(0., 1., x2*x2*x2);

	float s1 = draw_stars(uv*3., fixed2(0.,t*1.5)); 
	float s2 = draw_stars((uv+fixed2(0.257, 0.356))*2., fixed2(0.,t*1.25));
	fixed4 stars = fixed4(0.63, 0.63, 0.1, 1.0)*(s1+s2);
	stars.a = 1.;

	fixed4 bgcolor = fixed4(0.76*0.25, 0.23*0.25, 0.74*0.25, 1.0)*0.15;
	fixed4 green = fixed4(noise(uv*20.), noise(uv*10.), noise(uv*30.), 1.0);
	fixed4 purple = fixed4(noise(uv*100.), noise(uv*50.), noise(uv*60.), 1.0);

	fixed4 nebula = green*x+purple*x2*noise(1000.*uv);
	fixed3 nebula_hsv = rgb_to_hsv_no_clip(nebula.rgb);
	nebula_hsv.g += 0.8;
	nebula.rgb = hsv_to_rgb(nebula_hsv);

	fixed4 clr = bgcolor + nebula + stars ; 
	fixed3 hsv = rgb_to_hsv_no_clip(clr.rgb);
	hsv.r += t*.1 + fbm(uv*20.)*6.;
	// hsv.r += t*.5;
	// hsv.g += .5;
	clr.rgb = clr.rgb + hsv_to_rgb(hsv);
	return clr;
	return bgcolor + (max(green*saturate(x), purple*saturate(x2)))*0.5;
	return fixed4(x,x,x,1.);
}


float hash1( float n )
{
	return frac(sin(n)*43758.5453);
}

float2 hash2( float2  p )
{
	p = float2( dot(p,float2(127.1,311.7)), dot(p,float2(269.5,183.3)) );
	return frac(sin(p)*43758.5453);
}

float4 voronoi( in float2 x, float w, float frame_count )
{
    float2 n = floor( x );
    float2 f = frac( x );

	float4 m = float4( 8.0, 0.0, 0.0, 0.0 );
    for( int j=-1; j<=2; j++ )
    for( int i=-1; i<=2; i++ )
    {
        float2 g = float2( float(i),float(j) );
        float2 o = hash2( n + g );
		
		// animate
        o = 0.5 + 0.5*sin( 0.01 * frame_count + 6.2831*o );

        // distance to cell		
		float d = length(g - f + o);
		
        // do the smooth min for colors and distances		
		float3 col = 0.5 + 0.5*sin( hash1(dot(n+g,float2(7.0,113.0)))*2.5 + 3.5 + float3(2.0,3.0,0.0));
		float h = smoothstep( 0.0, 1.0, 0.5 + 0.5*(m.x-d)/w );
		
	    m.x   = lerp( m.x,     d, h ) - h*(1.0-(h))*w/(1.0+3.0*w); // distance
		m.yzw = lerp( m.yzw, col, h ) - h*(1.0-h)*w/(1.0+3.0*w); // cioloe
    }
	
	return m;
}

fixed4 bg06(v2f i, float2 uv) {
	uv*=2.;
	float uvNoiseScale = 5.;
	float noiseScale = 0.5;
	float2 seaUv = ((uv+float2(0., _Time.y*0.5)) * 10.)
				 + float2(noise(uv*uvNoiseScale), noise((uv+float2(5.,-5.))*uvNoiseScale))
				 * noiseScale;

	float mainWave = 0.25*cos(abs(
		(uv.y + (frac(_Time.y*0.125)-0.5)*20.) - (-0.5*uv.x)
	));
	// seaUv.xy += mainWave*1.;
	float4 v = voronoi(seaUv, 0.00, _Time.y*100.);

	fixed4 clr = fixed4(113./255., 266./255., 246./255., 1.)*0.35;
	fixed3 hsv = rgb_to_hsv_no_clip(clr.rgb);
	// hsv.r += fbm(seaUv*10.)*0.125;
	hsv.b -= 0.1;
	// hsv.r = frac(hsv.g + v.x*0.5);
	clr.rgb = hsv_to_rgb(hsv);

	float darkerSea = noise((uv*0.25+float2(0.,_Time.y*0.25))*uvNoiseScale);
	fixed4 clrDarker = clr*0.7;
	clr = lerp(clr, clrDarker, darkerSea);
	
	float darkSea = smoothstep(0.0, 0.5, noise((uv+float2(0.,_Time.y*0.5))*uvNoiseScale*2.));
	// darkSea *= ;
	fixed4 clrDark = clr*0.8;
	clr = lerp(clr, clrDark, darkSea);

	float c = noise(seaUv*0.25);
	float x = (noise(uv+fixed2(0.,ddx(uv.x))))-c;
	float y = (noise(uv+fixed2(0.,ddy(uv.y))))-c;
	float lighterSea = x*y;
	fixed4 clrLight = clr * 2.1;
	clr = lerp(clr, clrLight, lighterSea);

	float a = lighterSea;

	float ripples = smoothstep(0.045, 0.5, v.x*v.x*v.x);
	ripples = saturate(ripples-noise(seaUv));

	float2 ruv = seaUv*.75;
	ripples = length(frac(ruv)*2.-1);
	float nruv = fbm(ruv);
	ripples = step(.5, nruv) - step(.6, nruv);
	ripples = smoothstep(0.45, 0.5, nruv) - smoothstep(0.5, 0.525, nruv);
	fixed4 clrRipples = fixed4(245./255.,208./255.,162./255.,1.)*0.4;
	clr = lerp(clr, clrRipples, ripples);

	hsv = rgb_to_hsv_no_clip(clr.rgb);
	hsv.g -= 0.1;
	hsv.b -= 0.05;
	clr.rgb = hsv_to_rgb(hsv);

	return clr;
}

// uv.y += _Time.y*.5;
	// float2 buv = uv*5.;
	// float2 id = floor(buv);
	// buv.y += (id.x % 2.)*1.5;
	// id = floor(buv);
	// buv = frac(buv)*2.-1.;

	// float angle = noise(id*2.)*100.;
	// float2x2 rot = float2x2(
	// 	cos(angle), -sin(angle),
	// 	sin(angle), cos(angle)
	// );
	// buv = mul(rot, buv);

	// float upper = .7;
	// float lower = .1;

	// float ml1 = lerp(-upper, -lower, noise(id.x));
	// float ml2 = lerp(-upper, -lower, noise(id.y));
	// float mr1 = lerp(lower, upper, noise(id.y));
	// float mr2 = lerp(lower, upper, noise(id.x));

	// float2 sqr = step(float2(ml1,ml2), buv) * step(buv, float2(mr1,mr2));
	// float a = sqr.x * sqr.y;
	// a *= step(.5, noise(id));
	// // a = noise(id);
	// // a += sqr.x * sqr.y;


	// return fixed4(a,0.,0.,1.0);

// https://iquilezles.org/articles/distfunctions/
float sdBox( float3 p, float3 b )
{
  float3 q = abs(p) - b;
  return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0)-.001;
}

// Calcs the gradient of the distance to a box. Works at
// any point in space, including the interior of the box.
float boxGradient( in float3 p, in float3 rad ) 
{
    float3 d = abs(p)-rad;
    float3 s = sign(p);
    float g = max(d.z, max(d.x, d.y));
    return s*((g>0.0) ? normalize(max(d,0.0)) : 
                        step(d.yzx,d.xyz)*step(d.zxy,d.xyz));
}

#define mod(x,y) (x-y*floor(x/y))
float f(float3 p ) {
	// Repeat domain for multiple boxes
	float e = 1.0;
	float3 c = float3(e, e, e);

	// Id each box and rotate it randomly
	// float2 op = round(p.xz+.5*c.xz)-.5*c.xz;
	float2 op = round(p.xz);
	p.xz = mod(p.xz+0.5*c.xz,c.xz)-0.5*c.xz;

	float angle = (noise(op.xy))*180.;
	float3x3 rotY = float3x3(
		cos(angle), 0., sin(angle),
		0., 1., 0.,
		-sin(angle), 0., cos(angle)
	);
	p = mul(rotY, p);

	float sdf = max(0., p.y+0.1);


	float n = (noise(op.xy-.5/c.xz)+.5/c.xz);
	float bsz = .125;
	float boxWidth= bsz + noise(op)*.25;
	float boxDepth = bsz + noise(op)*.25;
	float boxHeight = bsz*1. + n*1.0;
	float boxY = boxHeight + 0.1;

	float3 boxSz = float3(boxWidth,boxHeight,boxDepth);
	// boxSz *= step(.5, noise(op.xz));

	sdf = min(sdf, sdBox(p-float3(0.,boxY,0.), boxSz));
	return sdf;
}

float m(float3 p, float3 d) {
	float sdf = 0.;
	for (int i = 0; i < 50; i++) {
		float dd = f(p);
		p += d*dd;
		sdf += dd;
		if (dd <= 0.01 || dd >= 1000.)
			break;
	}
	return sdf;
}

float shadow(float3 p, float3 n, float3 lpos)
{
    float3 dir = normalize(lpos-p);
	float3 pp = p+n*.1;
    float d = m(pp, dir);
	if (d < length(lpos-p))
		return 0.0;
	return 1.;
    return clamp(d/length(lpos-p), 0., 1.);
}

float3 calcNormal(float3 p ) // for function f(p)
{
   float s = f(p).x;
    float2 a = float2(0.01, 0.);
    float3 n = float3(
        s - f(p-a.xyy).x,
        s - f(p-a.yxy).x,
        s - f(p-a.yyx).x
    );

    return normalize(n);
}


fixed4 bg07(v2f i, float2 uv) {
	float3 p = float3(0.,8.0,-3.);
	p += float3(0.,0.,1.)*_Time.y*0.5;
	float3 target = p+float3(0.,-1.0,1.35);
	float3 fwd = normalize(target - p);
	float3 up = float3(0., 1., 0.);
	float3 right = cross(up, fwd);
	up = cross(fwd, right);
	float3 dir = normalize(fwd+up*uv.y+right*uv.x);
	float sdf = 999.;
	float obj = 0.;
	float3 pp = p;

	sdf = m(p, dir);
	pp = p+dir*sdf;

	fixed4 clr = fixed4(0., 0., 0., 1.);
	clr = fixed4(1.,1.,1.,1.);

	if (pp.y <= 0.1)
		clr = fixed4(0.3, 0.45, 0.3,1.);
	else {
		float2 id = round(pp.xz);
		clr = lerp(
			fixed4(0.2, 0.15, 0.5,1.), 
			fixed4(0.05, 0.15, 0.4,1.), 
			noise(id.xy)
		);
		// clr = fixed4(noise(id), noise(id*2.), noise(id*3.), 1.);
	}

	float3 normal = calcNormal(pp);

	float3 lpos = p + float3(10., 10., -5.)*3.;
	// clr.rgb = normal;
	clr.rgb *= max(0.5,dot(normal, normalize(lpos-pp)));

	float ss = shadow(pp, normal, lpos);
	clr.rgb *= clamp(ss, .25, 1.);
	return clr;
}


#endif