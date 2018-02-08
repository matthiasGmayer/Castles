#version 430
in vec3 tex;

out vec4 vFragColor;

uniform samplerCube cubeMap;

void main(void){
	vFragColor = texture(cubeMap, tex);
	vFragColor = vec4(1);
}