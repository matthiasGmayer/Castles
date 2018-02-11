#version 430

in vec2 uv;

out vec4 vFragColor;

uniform sampler2D tex;

void main(void){
	vFragColor = texture(tex, uv);
}