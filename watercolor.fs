// Author @lesleyvanhoek (lesleyvanhoek.nl) - 2018
// Title: Watercolor

#ifdef GL_ES
precision highp float;
#endif

#define inv(x) 1.0 - x
#define flip(v) (v.y,v.x)
#define PI 3.14159265359
#define TWO_PI 6.28318530718

#define COL0 vec3(0.423, 0.337, 0.482)
#define COL1 vec3(0.752, 0.423, 0.517)
#define COL2 vec3(0.964, 0.447, 0.501)
#define COL3 vec3(0.972, 0.694, 0.584)

#define PLATINUM vec3(0.929, 0.901, 0.890)

uniform vec2 u_resolution;
uniform vec2 u_mouse;
uniform float u_time;

float rand (in vec2 st) 
{
    return fract(sin(dot(st.xy, vec2(12.9898,78.233))) * 43758.5453123);
}

// Based on Morgan McGuire @morgan3d
// https://www.shadertoy.com/view/4dS3Wd
float noise (in vec2 st) 
{
    vec2 i = floor(st);
    vec2 f = fract(st);

    // Four corners in 2D of a tile
    float a = rand(i);
    float b = rand(i + vec2(1.0, 0.0));
    float c = rand(i + vec2(0.0, 1.0));
    float d = rand(i + vec2(1.0, 1.0));

    vec2 u = f * f * (3.0 - 2.0 * f);

    return mix(a, b, u.x) +
            (c - a)* u.y * (1.0 - u.x) +
            (d - b) * u.x * u.y;
}

#define OCTAVES 6
float fbm (in vec2 st) 
{
    // Initial values
    float value = 0.0;
    float amplitude = .5;
    float frequency = 0.;

    // Loop of octaves
    for (int i = 0; i < OCTAVES; i++) {
        value += amplitude * noise(st);
        st *= 2.;
        amplitude *= .5;
    }
    return value;
}

float sfunc(float x)
{
    return max(max(0.8*step(x,0.33)*inv(clamp(pow(abs(max(x-0.235, 0.0)*10.0), 4.0), 0.0, 1.0)),
               0.25 * inv(clamp(pow(abs(min(x-0.820, 0.0)*2.0), 16.0), 0.0, 1.0))),
               0.45 * inv(clamp(pow(abs(min(x-0.864, 0.0)*2.0), 40.0), 0.0, 1.0)));
}

void main() 
{
    vec2 st = gl_FragCoord.xy/u_resolution.xy;
    st.x *= u_resolution.x/u_resolution.y;
    
    float t = 0.75*u_time;
    float dist = distance(st, vec2(0.5, 0.5));
    float pct;

    st = st * 24.0 + 20.0*vec2(0.730,0.660) + 15.0 * vec2(sin(t), cos(t));
    pct = fbm(st*0.09);
    pct += dist*1.5;
    pct = clamp(pct, 0.0, 1.0);
    pct = sfunc(pct*0.45);
    pct += 0.085*noise(st*16.0);
    
    vec3 col = mix(COL2, PLATINUM, pct);
    gl_FragColor = vec4(col, 1.0);
}
