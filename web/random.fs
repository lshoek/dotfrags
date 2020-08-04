// Author @lesleyvanhoek (lesleyvanhoek.nl) - 2017
// Title: Random Noise

#ifdef GL_ES
precision highp float;
#endif

#define inv(x) 1.0 - x
#define flip(v) (v.y,v.x)
#define PI     3.14159265359
#define TWO_PI 6.28318530718

#define CARMINE_PINK vec3(0.952, 0.145, 0.031)
#define PLATINUM vec3(0.929, 0.901, 0.890)

uniform vec2 u_resolution;
uniform float u_time;

vec2 tile(vec2 st, float zoom)
{
    st *= zoom;
    return vec2(st.x, fract(st.y));
}

vec2 rotate(vec2 coord, float angle) 
{
    return mat2(cos(angle),-sin(angle), sin(angle),cos(angle)) * coord;
}

float rect(vec2 st, vec2 dim)
{
    dim = 0.25-dim*0.25;
    vec2 uv = step(dim, st*(inv(st)));
    return uv.x*uv.y;
}

float stripe(float p, float w0, float w1, float offset) 
{
    float w = w0 + w1;
    return step(w0, mod(p + offset, w));
}

float random (vec2 st) 
{
    return fract(sin(dot(st.xy, vec2(20.9898, 78.233))) * 43758.513);
}

void main()
{
    vec2 st = gl_FragCoord.xy/u_resolution;
    float time = u_time/2.5;
    float stripeoffset = u_time/8.0;
    float thickness = 0.10;
    const float tiles = 4.0;
    
    // tile the scene
    float row = floor(st.y*tiles);  
    vec2 grid = tile(st, tiles);  
    float even = mod(row, 2.0);
    
    float pct0, pct1;
    
    // draw four rects
    for (int i = 0; i <= int(tiles); i++)
        pct0 += rect(grid + vec2(-float(i), 0.0), vec2(0.5, 0.5));

    // cut out some shape
    pct0 -= clamp(even, 0.0, 1.0) * 
        stripe(rotate(grid, random(grid)).x, thickness, thickness, stripeoffset);
    pct0 -= clamp(inv(even), 0.0, 1.0) * 
        stripe(rotate(grid, cos(dot(st.xy, vec2(1.75)))).x, thickness, thickness, stripeoffset);
    
    // mix result
    float res0 = clamp(pct0, 0.0, 1.0);
    vec3 col = mix(PLATINUM, CARMINE_PINK, res0);
    gl_FragColor = vec4(col, 1.0);
}
