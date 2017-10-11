#ifdef GL_ES
precision mediump float;
#endif

#define inv(x) 1.0 - x
#define PI     3.14159265359
#define TWO_PI 6.28318530718

uniform vec2 u_resolution;
uniform float u_time;

vec2 tile(in vec2 st, in float zoom)
{
    st *= zoom;
    return fract(st);
}

float circle(vec2 st, vec2 pos, float radius)
{
    vec2 tp = pos - st; 
    return smoothstep(radius + radius*0.015, radius - radius*0.015, dot(tp, tp) * 4.0);
}

float shape(vec2 st, float sides, float mult)
{    
    // remap to [-1.0, 1.0]
    vec2 tp = st*2.0-1.0;
    sides = floor(abs(sides));
    tp = (sides == 3.0) ? tp*1.5+vec2(0.0, 0.25) : tp;
    
    // angle and radius from pixel
    float a = atan(tp.x, tp.y)+PI;
    float r = TWO_PI/sides;
    
    // shaping function
    float d = cos(floor(0.5+a/r)*r-a)*length(tp)/mult;
    return inv(smoothstep(0.5, 0.525, d));
}

void main()
{
    vec2 st = gl_FragCoord.xy/u_resolution;   
    float numtiles = floor(mod(u_time, 12.0)+1.0);
    
    st.y = inv(st.y);
    float row = floor(st.y*numtiles);
    float column = floor(st.x*numtiles);
    st.y = inv(st.y);
    
    vec2 grid = tile(st, numtiles);
    
    float even = mod(row, 2.0);
    vec3 col = mix(vec3(0.250, 0.525, 0.415), vec3(0.796,0.460,0.297), even);
    vec3 bg = vec3(0.286, 0.258, 0.239);
    vec3 pct = vec3(shape(grid, max(row, column)+3.0, 1.0));
    
    col = mix(bg, col, pct);
    gl_FragColor = vec4(col, 1.0);
}