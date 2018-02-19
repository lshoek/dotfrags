// Author @lesleyvanhoek (lesleyvanhoek.nl) - 2018
// Title: Cloud

#ifdef GL_ES
precision highp float;
#endif

// toolbox
#define inv(x) 1.0 - x
#define PI     3.14159265359
#define TWO_PI 6.28318530718

// margins
#define MARGIN 0.150
#define OFFSET 0.02

// palette
#define COL000 vec3(0.964, 0.447, 0.501)
#define COL001 vec3(0.729, 0.949, 0.733)
#define COL002 vec3(0.854, 0.203, 0.301)
#define COL003 vec3(0.737, 0.768, 0.858)
#define COL004 vec3(0.737, 0.768, 0.858)
#define COL005 vec3(0.176, 0.192, 0.258)
#define COL006 vec3(0.470, 0.501, 0.709)
#define COL007 vec3(0.752, 0.662, 0.690)

#define BACK vec3(0.917, 0.870, 0.854)
#define BRIGHT vec3(0.976, 0.972, 0.972)
#define DARK vec3(0.066, 0.082, 0.109)
#define BLACK vec3(0.0)

// params
#define DISTORTION_RATE 24.0

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

float ellipse(vec2 st, vec2 pos, vec2 size, float angle, float distort, float rate, float grad)
{
    vec2 tp = pos - st;
    
    float a = atan(tp.y, tp.x) + angle;
    float pct = size.x*size.y/(pow(pow(size.x*cos(a), 2.0) + pow(size.y*sin(a), 2.0), 0.65));
    
    // distortion
    vec2 stx = vec2(st.x, st.y) * rate;
    stx += vec2(sin(u_time), cos(u_time));
    
    pct += distort * noise(stx) * 0.25;
    return smoothstep(pct+pct*grad, pct-pct*grad, length(tp) * 4.0);
}

float modshape(vec2 st, vec2 pos, float radius, float angle, float distort, float rate)
{
    vec2 tp = pos - st;
    float r = (length(tp) * 2.0) / (radius * 2.0);
    float a = atan(tp.y, tp.x) + angle;
    float pct = 0.5 * sin(a*2.0) + 1.0;
    
    // distortion
    vec2 stx = vec2(st.x, st.y) * rate;
    stx += vec2(sin(u_time), cos(u_time));
    
    pct += distort * noise(stx) * 0.25;
    return smoothstep(pct+pct*0.01, pct-pct*0.01, r);
}

float plotfill(vec2 st, float pct, float distort, float rate, float time, float grad)
{
    pct = inv(smoothstep(pct+0.01, pct-0.1, st.y));

    // distortion
    vec2 stx = vec2(st.x, st.y) * rate;
    stx += vec2(sin(time), cos(time));
    
    pct += distort * noise(stx) * 0.25;
    return smoothstep(pct+pct*grad, pct-pct*grad, 0.275);
}

void main()
{
    vec2 st = gl_FragCoord.xy/u_resolution;
    float a = 0.0;
    float rt = 18.0;
    
    // corrections
    float py = 0.0975;
    float d0 = 0.0 + 0.1 * sin(u_time);
    float d1 = -0.25 + 0.025 * cos(u_time);
    
    // clouds
    float x = ellipse(st, vec2(0.50,0.28+py), 1.058*abs(vec2(0.22+d0,0.56+d1)), a, 1.0, rt, 0.01);
    float pct1 = ellipse(st, vec2(0.500,0.28+py), 1.75*abs(vec2(0.5+d0,1.5+d1)), a, 1.0, rt, 0.01);
    float pct0 = ellipse(st, vec2(0.500,0.28+py), 2.0*abs(vec2(0.70+d0,1.9+d1)), a, 1.0, rt, 0.01)-pct1;
    float pct2 = ellipse(st, vec2(0.500,0.31+py), 1.85*abs(vec2(0.27+d0,1.15+d1)), a, 1.0, rt, 0.01)-x;
    float pct3 = ellipse(st, vec2(0.500,0.225+py), 1.75*abs(vec2(0.48+d0,1.2+d1)), a, 1.0, rt, 0.01)-x;
    float pct4 = ellipse(st, vec2(0.500,0.239+py), 1.682*abs(vec2(0.41+d0,1.025+d1)), a, 1.0, rt, 0.01)-x; 
    float pct5 = ellipse(st, vec2(0.50,0.255+py), 1.75*abs(vec2(0.33+d0, 0.85+d1)), a, 1.0, rt, 0.01)-x;
    float pct6 = ellipse(st, vec2(0.50,0.285+py), 1.15*abs(vec2(0.29+d0,0.85+d1)), a, 1.0, rt, 0.01)-x;
    float pct7 = ellipse(st, vec2(0.50,0.2875+py), 1.15*abs(vec2(0.240+d0,0.7+d1)), a, 1.0, rt, 0.01)-x;

    float eq = clamp(max(st.y,0.35+py)+max(0.045*pow(cos(PI*(7.0*clamp(st.x,0.3,0.6)-1.5)),0.5),0.001),0.0,1.0);
    float gnd0 = plotfill(st + vec2(0.430, 0.001), eq, 0.8, 84.0, 1.568, 0.05);
    float gnd1 = plotfill(st + vec2(0.430, 0.010), eq, 0.8, 84.0, 1.568, 0.15);
    vec2 stn = st + noise(st*10.0*vec2(-0.380,0.600));
    float splatters = inv(smoothstep(0.0,0.05, noise(100.0*vec2(-0.230,0.060)+stn*150.0)));
    
    float letterbox = step(st.y, MARGIN) + step(inv(st.y), MARGIN);
    float frame = max(step(st.y, MARGIN + OFFSET) + step(inv(st.y), MARGIN + OFFSET), 
        step(st.x, OFFSET) + + step(inv(st.x), OFFSET));
    
    // coloring clouds
    vec3 col = mix(BACK, DARK, smoothstep(0.0, 4.0, fbm(600.0*st)));
    col = mix(col, COL000, pct0);
    col = mix(col, COL002, pct2);
    col = mix(col, COL001, pct3);
    col = mix(col, BACK, pct4); 
    col = mix(col, COL007, pct5);
    col = mix(col, COL006, pct6);
    col = mix(col, COL005, pct7);
    
    // coloring ground
    col = mix(col, COL000, inv(gnd0));
    col = mix(col, DARK + 0.08*fbm(600.0*st), inv(gnd1));
    col = mix(col, COL000, splatters);
    
    // coloring frame
    col = mix(col, BRIGHT, frame);
    col = mix(col, BLACK, letterbox);
    
    gl_FragColor = vec4(col, 1.0);
}
