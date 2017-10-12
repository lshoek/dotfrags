// Author @lesleyvanhoek (lesleyvanhoek.nl) - 2017
// Title: Modern Tartan

#ifdef GL_ES
precision mediump float;
#endif

#define inv(x) 1.0 - x
#define flip(v) (v.y,v.x)
#define PI     3.14159265359
#define TWO_PI 6.28318530718

#define CARMINE_PINK vec3(0.9, 0.3, 0.3)
#define PLATINUM vec3(0.929, 0.901, 0.890)
#define EERIE_BLACK vec3(0.066, 0.082, 0.109)
#define CAMBRIDGE_BLUE vec3(0.643, 0.764, 0.698)

uniform vec2 u_resolution;
uniform float u_time;

vec2 tile(vec2 st, float zoom)
{
    st *= zoom;
    return fract(st);
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

void main()
{
    vec2 st = gl_FragCoord.xy/u_resolution;
    float time = u_time/2.5;
    float stripeoffset = u_time/25.0;
    float thickness = -0.030;
    float tiles = 1.25;
    
    float row = floor(st.y*tiles);
    float column = floor(st.x*tiles);
    
    vec2 translate = vec2(time*3.0, sin(time*1.5));
    st += translate*0.1;
    
    vec2 grid = tile(st, tiles);    
    vec2 offset = vec2(0.2, 0.0);
    float even = mod(column, 2.0);
    
    // ver black
    float pct0, pct1;
    for (int i = 1; i <= 3; i++)
        pct0 += rect(grid+vec2(0.5, 0.0)+vec2(-0.25,0.0)*float(i), vec2(0.01, 1.0));
    pct0 -= stripe(rotate(grid, PI * 0.25).x, thickness, thickness, 0.0 + stripeoffset);   
    // hor black
    for (int i = 1; i <= 3; i++)
        pct1 += rect(grid+vec2(0.0, 0.5)+vec2(0.0, -0.25)*float(i), vec2(1.0, 0.01));
    pct1 -= stripe(rotate(grid, PI * 0.25).x, thickness, thickness, thickness + stripeoffset);
    
    float res0 = max(pct0, pct1);
    res0 = clamp(res0, 0.0, 1.0);
    
    // ver white
    float pct2 = rect(grid, vec2(2.0, 0.16));
    pct2 -= stripe(rotate(grid, PI * 0.25).x, thickness, thickness, thickness + stripeoffset);
    // hor white
    float pct3 = rect(grid, vec2(0.16, 2.0));
    pct3 -= stripe(rotate(grid, PI * 0.25).x, thickness, thickness, 0.0 + stripeoffset);
    
    float res1 = max(pct2, pct3);
    res1 -= res0;
    res1 = clamp(res1, 0.0, 1.0);
    
    // ver red
    float pct4 = rect(grid + vec2(0.0, 0.5), vec2(2.0, 0.001));
    pct4 -= stripe(rotate(grid, PI * 0.25).x, thickness, thickness, thickness + stripeoffset);
    // hor red
    float pct5 = rect(grid + vec2(0.5, 0.0), vec2(0.001, 2.0));
    pct5 -= stripe(rotate(grid, PI * 0.25).x, thickness, thickness, 0.0 + stripeoffset);
    
    float res2 = max(pct4, pct5);
    
    vec3 col = mix(CAMBRIDGE_BLUE, EERIE_BLACK, res0);
    col = mix(col, PLATINUM, res1);
    col = mix(col, CARMINE_PINK, res2);
    
    gl_FragColor = vec4(col, 1.0);
}