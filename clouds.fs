// Author @lesleyvanhoek (lesleyvanhoek.nl) - 2018
// Title: Clouds

#ifdef GL_ES
precision highp float;
#endif

// toolbox
#define inv(x) 1.0 - x
#define PI     3.14159265359
#define TWO_PI 6.28318530718
#define TIME u_time

// margins
#define MARGIN 0.150
#define OFFSET 0.02

// palette: bright tints
#define COL000 vec3(0.721, 0.615, 0.737)
#define COL001 vec3(0.619, 0.278, 0.415)
#define COL002 vec3(0.670, 0.321, 0.345)
#define COL003 vec3(0.694, 0.368, 0.305)
#define COL004 vec3(0.768, 0.654, 0.356)
#define COL005 vec3(0.639, 0.443, 0.454)
#define COL006 vec3(0.619, 0.329, 0.396)
#define COL007 vec3(0.721, 0.447, 0.313)

// palette: dark tints
#define COL000D vec3(0.330,0.253,0.475)
#define COL001D vec3(0.392, 0.180, 0.270)
#define COL002D vec3(0.466, 0.192, 0.223)
#define COL003D vec3(0.537, 0.211, 0.086)
#define COL004D vec3(0.584, 0.372, 0.035)
#define COL005D vec3(0.498, 0.266, 0.274)
#define COL006D vec3(0.4, 0.207, 0.180)
#define COL007D vec3(0.6, 0.266, 0.145)

// palette: etc
#define GAINSBORO vec3(0.900,0.854,0.838)
#define OLD_BURGHUNDY vec3(0.147,0.119,0.175)
#define GHOST_WHITE vec3(0.976, 0.972, 0.972)
#define RAISIN_BLACK vec3(0.145, 0.149, 0.152)
#define PARROT_PINK vec3(0.952, 0.854, 0.847)

// params
#define DISTORTION_RATE 24.0
#define POS vec2(0.5, 0.26)
#define PUSH vec2(0.0, 0.009)
#define PY vec2(0.0, 0.1)
#define FD 0.055
#define EDG 0.01

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

float ellipse(vec2 st, vec2 pos, vec2 size, float angle, float time, float distort, float rate, float grad)
{
    vec2 tp = pos - st;
    
    float a = atan(tp.y, tp.x) + angle;
    float pct = size.x*size.y/(pow(pow(size.x*cos(a), 2.0) + pow(size.y*sin(a), 2.0), 0.65));
    
    vec2 stx = vec2(st.x, st.y) * rate;
    stx += vec2(sin(time), cos(time));
    
    // wavy distortion
    pct += distort * noise(stx) * 0.25;
    
    //noisy distortion
    pct += distort * fbm(12.0*st) * 0.25;
    
    return smoothstep(pct+pct*grad, pct-pct*grad, length(tp) * 4.0);
}

float plotfill(vec2 st, float pct, float rate, float time, float grad)
{
    pct = inv(smoothstep(pct+0.01, pct-0.1, st.y));

    // distortion
    vec2 stx = vec2(st.x, st.y) * rate;
    stx += vec2(sin(time), cos(time));
    
    // wavy distortion
    pct +=  noise(stx) * 0.175;
    
    //noisy distortion
    pct += fract(pct) * fbm(300.0*st) * 0.25;
    
    return smoothstep(pct+pct*grad, pct-pct*grad, 0.275);
}

void main()
{
    vec2 st = gl_FragCoord.xy/u_resolution;
    
    // corrections
    float d0 = 0.0 + 0.1 * sin(TIME);
    float d1 = -0.2 + 0.02 * cos(TIME);
    float a = 0.0;
    float rt = 18.0;
    float t = TIME;

    // clouds
    float x = ellipse(st, vec2(0,-.02)+POS-2.5*PUSH+PY, 1.0*abs(vec2(0.2+d0,0.5+d1)), a, t, 1.0, rt, EDG+FD);
    
    float pct0d = ellipse(st, vec2(0,.04)+POS+PUSH+PY, 2.05*abs(vec2(0.70+d0,1.9+d1)), a, t, 1.0, rt-1.0, EDG)-x;
    float pct0 = ellipse(st, vec2(0,.04)+POS+PY, 2.0*abs(vec2(0.70+d0,1.9+d1)), a, t, 1.0, rt-1.0, EDG+FD)-x;
    
    float pct1d = ellipse(st, vec2(0,.025)+POS+PUSH+PY, 2.05*abs(vec2(0.5+d0,1.5+d1)), 0.02, t, 1.0, rt-0.5, EDG)-x;
    float pct1 = ellipse(st, vec2(0,.025)+POS+PY, 2.03*abs(vec2(0.5+d0,1.5+d1)), 0.04, t, 1.0, rt-0.5, EDG+FD)-x;
    
    float pct2d = ellipse(st, vec2(0,.01)+POS+PUSH+PY, 1.95*abs(vec2(0.42+d0,1.26+d1)), -0.02, t, 1.0, rt-1.0, EDG)-x;
    float pct2 = ellipse(st, vec2(0,.01)+POS+PY, 1.93*abs(vec2(0.42+d0,1.26+d1)), -0.04, t, 1.0, rt-1.0, EDG+FD)-x;
    
    float pct3d = ellipse(st, POS+PUSH+PY, 1.77*abs(vec2(0.39+d0,1.1+d1)), -0.01, t, 1.0, rt-1.0, EDG)-x;
    float pct3 = ellipse(st, POS+PY, 1.75*abs(vec2(0.39+d0,1.1+d1)), 0.01, t, 1.0, rt-1.0, EDG+FD)-x; 
    
    float pct4d = ellipse(st, vec2(0,-.01)+POS+PUSH+PY, 1.57*abs(vec2(0.37+d0,1.025+d1)), 0.01, t, 1.0, rt-0.5, EDG)-x;
    float pct4 = ellipse(st, vec2(0,-.01)+POS+PY, 1.55*abs(vec2(0.37+d0,1.025+d1)), -0.02, t, 1.0, rt-0.5, EDG+FD)-x;
    
    float pct5d = ellipse(st, vec2(0,-.01)+POS+PUSH+PY, 1.35*abs(vec2(0.33+d0,0.95+d1)), 0.02, t, 1.0, rt, EDG)-x;
    float pct5 = ellipse(st, vec2(0,-.01)+POS+PY, 1.33*abs(vec2(0.33+d0,0.95+d1)), -0.01, t, 1.0, rt, EDG+FD)-x;
    
    float pct6d = ellipse(st, vec2(0,-.02)+POS+PUSH+PY, 1.15*abs(vec2(0.29+d0,0.85+d1)), -0.04, t, 1.0, rt-0.325, EDG)-x;
    float pct6 = ellipse(st, vec2(0,-.02)+POS+PY, 1.15*abs(vec2(0.29+d0,0.85+d1)), a, t, 1.0, rt-0.25, EDG+FD)-x;
    
    float pct7d = ellipse(st, vec2(0,-.02)+POS+PUSH+PY, 1.05*abs(vec2(0.22+d0,0.7+d1)), a, t, 1.0, rt, EDG)-x;
    float pct7 = ellipse(st, vec2(0,-.02)+POS+PY, 1.0*abs(vec2(0.22+d0,0.7+d1)), a, t, 1.0, rt+0.25, 0.05+EDG+FD)-x;
    
    // ground
    float eq = clamp(max(st.y,0.35+0.1)+max(0.0175*pow(cos(PI*(7.0*clamp(st.x,0.3,0.6)-1.5)),0.05),0.001),0.0,1.0);
    float gnd = plotfill(st + vec2(0.430, 0.025), eq, 59.0, 2.75, 0.175);
    float gndx = inv(ellipse(st, vec2(0.50,0.650), 6.0*abs(vec2(0.22,0.7)), a, t, 1.0, rt*1.5, 0.085));
    
    // splatters for artistic effect
    vec2 stn = st + noise(st*10.0*vec2(-0.380,0.600));
    float splatters0 = inv(smoothstep(0.0,0.05, noise(100.0*vec2(-0.230,0.060)+stn*150.0)));
    float splatters1 = inv(smoothstep(0.0,0.075, noise(10.0*vec2(-0.600,0.190)+stn*120.0)));
    
    // framing
    float letterbox = step(st.y, MARGIN) + step(inv(st.y), MARGIN);
    float frame = max(step(st.y, MARGIN + OFFSET) + step(inv(st.y), MARGIN + OFFSET), 
        step(st.x, OFFSET) + step(inv(st.x), OFFSET));
    float tex = 3.0*smoothstep(0.0, 4.0, fbm(600.0*st));
    
    // mixing clouds
    vec3 col = mix(GAINSBORO, OLD_BURGHUNDY, 0.3*tex);
    col = mix(col, COL000D, pct0d);
    col = mix(col, COL000, pct0 + pct0*tex);
    col = mix(col, COL001D, pct1d);
    col = mix(col, COL001, pct1 + pct1*tex);
    col = mix(col, COL002D, pct2d);
    col = mix(col, COL002, pct2 + pct2*tex);
    col = mix(col, COL007D, pct3d);
    col = mix(col, COL007, pct3 + pct3*tex);
    col = mix(col, COL004D, pct4d); 
    col = mix(col, COL004, pct4 + pct4*tex); 
    col = mix(col, COL005D, pct5d);
    col = mix(col, COL005, pct5 + pct5*tex);
    col = mix(col, COL006D, pct6d);
    col = mix(col, COL006, pct6 + pct6*tex);
    col = mix(col, COL003D, pct7d);
    col = mix(col, COL003, pct7 + pct7*tex);
    
    // mixing ground
    col = mix(col, PARROT_PINK, gndx*0.6);
    col = mix(col, OLD_BURGHUNDY + 0.08*fbm(600.0*st), inv(gnd));
    col = mix(col, COL005, splatters0);
    col = mix(col, COL001, clamp(0.0, 1.0, inv(gnd))*splatters1);
    
    // coloring frame
    col = mix(col, GHOST_WHITE, frame);
    col = mix(col, RAISIN_BLACK, letterbox);
    
    gl_FragColor = vec4(col, 1.0);
}
