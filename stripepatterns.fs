// Author @lesleyvanhoek (lesleyvanhoek.nl) - 2018
// Title: Stripe Patterns

#ifdef GL_ES
precision highp float;
#endif

#define inv(x) 1.0 - x
#define flip(v) (v.y,v.x)
#define PI     3.14159265359
#define TWO_PI 6.28318530718

#define MARGIN 0.000
#define OFFSET 0.0375

#define COLOR_0 vec3(1, 0.803, 0.737)
#define COLOR_1 vec3(0.223, 0.356, 0.313)

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

float rand (vec2 st) 
{
    return fract(sin(dot(st.xy, vec2(20.9898, 78.233))) * 43758.513);
}

void main()
{
    vec2 st = gl_FragCoord.xy/u_resolution;
    float time = u_time/2.5;
    float stripeoffset = u_time/8.0;
    float thickness = 0.075;
    const float tiles = 4.0;
    
    // tile the scene
    float row = floor(st.y*tiles);  
    vec2 grid = tile(st, tiles);  
    float rate = 0.25*row;
    
    float pct0, pct1;
    
    // draw four rects
    for (int i = 0; i <= int(tiles); i++)
        pct0 += rect(grid + vec2(-float(i), 0.0), vec2(1.0, 0.7));
    
    float frame = max(step(st.y, MARGIN + OFFSET) + step(inv(st.y), MARGIN + OFFSET), step(st.x, OFFSET) + step(inv(st.x), OFFSET));

    // cut out some shape
    
    pct0 -= stripe(rotate(grid, cos(dot(st.xy, vec2(row/2.5) + 1.5))).x, 0.025+rate*thickness, thickness, stripeoffset);
    pct0 -= frame;
    
    // mix result
    float res0 = clamp(pct0, 0.0, 1.0);
    vec3 col = mix(COLOR_0, COLOR_1, res0);
    gl_FragColor = vec4(col, 1.0);
}
