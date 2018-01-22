// Author @lesleyvanhoek (lesleyvanhoek.nl) - 2017
// Title: Rainbow Wheel

#ifdef GL_ES
precision highp float;
#endif

#define inv(x) 1.0 - x
#define PI     3.14159265359
#define TWO_PI 6.28318530718

uniform vec2 u_resolution;
uniform float u_time;

// Iñigo Quiles https://www.shadertoy.com/view/MsS3Wc
vec3 hsb2rgb(in vec3 c)
{
    vec3 rgb = clamp(abs(mod(c.x * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
    rgb = rgb * rgb * (3.0 - 2.0 * rgb);
    return c.z * mix(vec3(1.0), rgb, c.y);
}

float plot(vec2 st, float pct)
{
    return smoothstep(pct - 0.025, pct, st.y) - smoothstep(pct, pct + 0.025, st.y);
}

void main()
{
    vec2 st = gl_FragCoord.xy/u_resolution;
    vec3 color = vec3(0.0);
    
    // Use polar coordinates instead of cartesian
    vec2 toCenter = vec2(0.5) - st;
    float angle = atan(toCenter.y, toCenter.x) + u_time;
    float radius = length(toCenter) * 2.0;
    
    // Map the angle (-PI to PI) to the Hue (from 0 to 1), and the Saturation to the radius
    float angle_normalized = angle / TWO_PI + 0.5;
    
    // Shaping functions
    float ypolar = pow(cos(PI*(angle_normalized*2.0-1.0)/2.0), 2.5); 
    float ycarthesian = pow(cos(PI*(st.x*2.0-1.0)/2.0), 2.5); 
    float pct0 = plot(st, ypolar);
    float pct1 = plot(st, ycarthesian);
    
    // Mix colors and plots
    color = hsb2rgb(vec3(ypolar+(u_time*0.25), radius, 1.0));
    //color = mix(color, vec3(1.0), pct0);
    //color = mix(color, vec3(1.0), pct1);
    
    gl_FragColor = vec4(color, 1.0);
}
