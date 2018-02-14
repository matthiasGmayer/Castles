#version 430

in vec2 pixels[11];

out vec4 vFragColor;

uniform sampler2D tex;

void main(void){


	vec2 finalPixel[11];

	for(int i = 0; i < 11; i++)
		finalPixel[i] = clamp(pixels[i],0,1);
	

	vFragColor = vec4(0);
	vFragColor += texture(tex, finalPixel[0]) * 0.0093;
    vFragColor += texture(tex, finalPixel[1]) * 0.028002;
    vFragColor += texture(tex, finalPixel[2]) * 0.065984;
    vFragColor += texture(tex, finalPixel[3]) * 0.121703;
    vFragColor += texture(tex, finalPixel[4]) * 0.175713;
    vFragColor += texture(tex, finalPixel[5]) * 0.198596;
    vFragColor += texture(tex, finalPixel[6]) * 0.175713;
    vFragColor += texture(tex, finalPixel[7]) * 0.121703;
    vFragColor += texture(tex, finalPixel[8]) * 0.065984;
    vFragColor += texture(tex, finalPixel[9]) * 0.028002;
    vFragColor += texture(tex, finalPixel[10]) * 0.0093;
}