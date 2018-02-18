#version 430

in vec3 in_position;
in vec2 in_uv;

uniform int targetWidth;
uniform int targetHeight;
uniform bool vertical;

out vec2[11] pixels;

void main(void){
    gl_Position = vec4(in_position, 1);
	float pixelWidth;

	if(vertical){
		pixelWidth = 1f / targetHeight;
		for(int i = 0; i < 11; i++)
			pixels[i] = in_uv + vec2((i-5)*pixelWidth,0);
	}
	else{
		pixelWidth = 1f / targetWidth;
		for(int i = 0; i < 11; i++)
			pixels[i] = in_uv + vec2(0,(i-5)*pixelWidth);
	}


}