#ifdef GL_ES
precision mediump float;
#endif

#define inv(x) 1.0 - x
#define PI     3.14159265359
#define TWO_PI 6.28318530718

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
    float f = cos(a * 3.0) + 1.9;
    return smoothstep(f+f*0.01, f-f*0.01, r);
}

void main()
{
	vec2 st = gl_FragCoord.xy/u_resolution;
    vec2 mouse = u_mouse / u_resolution;
	vec2 center = vec2(0.5);
    vec2 mousedir = mouse - center;
 	float angle = -atan(mousedir.y, mousedir.x);
    
    float offset = 0.5;
    vec2 rot0 = center + vec2(sin(angle + offset), 
                                 cos(angle + offset)) * 0.3;
    vec2 rot1 = center + vec2(sin(angle+TWO_PI*(1.0/3.0) + offset), 
                                 cos(angle +TWO_PI*(1.0/3.0) + offset)) * 0.3;
    vec2 rot2 = center + vec2(sin(angle+TWO_PI*(2.0/3.0) + offset), 
                                 cos(angle +TWO_PI*(2.0/3.0) + offset)) * 0.3;
    
    float holes = circle(st, rot0, 0.025);
    holes += circle(st, rot1, 0.025);
    holes += circle(st, rot2, 0.025);
    holes -= circle(st, rot0, 0.015);
    holes -= circle(st, rot1, 0.015);
    holes -= circle(st, rot2, 0.015);
    holes += circle(st, rot0, 0.005);
    holes += circle(st, rot1, 0.005);
    holes += circle(st, rot2, 0.005);
    
    float pct = shape(st, center, 0.15, angle);
    pct -= circle(st, center, 0.03);
    pct += circle(st, center, 0.020);
    
    pct -= holes;
    
    vec3 color = pct * vec3(1.000,0.492,0.328);  
	gl_FragColor = vec4(color, 1.0);
}
