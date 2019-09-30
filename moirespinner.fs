// Author @lesleyvanhoek (lesleyvanhoek.nl) - 2017
// Title: Moiré Spinner

#ifdef GL_ES
precision highp float;
#endif

#define inv(x) 1.0 - x
#define PI     3.14159265359
#define TWO_PI 6.28318530718
#define PEAKS 512.0

uniform vec2 u_resolution;
uniform vec2 u_mouse;
uniform float u_time;

float circle(vec2 st, vec2 pos, float radius)
{
    vec2 tp = pos - st; 
    return smoothstep(radius + radius*0.075, radius - radius*0.075, dot(tp, tp) * 4.0);
}

float shape(vec2 st, vec2 pos, float radius, float angle)
{
    vec2 tp = pos - st;
    float r = (length(tp) * 2.0) / (radius * 2.0);
    float a = atan(tp.y, tp.x) + angle;
    
    float peaks = angle/TWO_PI*PEAKS;
    float f = cos(a * floor(peaks)) + 2.0;
    
    return smoothstep(f+f*0.01, f-f*0.01, r);
}

void main()
{
	vec2 st = gl_FragCoord.xy/u_resolution;
    vec2 mouse = u_mouse / u_resolution;
	vec2 center = vec2(0.5);
    
    vec2 mousedir = normalize(mouse - center);
 	float angle = -atan(mousedir.y, mousedir.x);
    
    float pct = shape(st, center, 0.15, angle);
    vec3 color = pct * vec3(1.000,0.492,0.328);  
	gl_FragColor = vec4(color, 1.0);
}
