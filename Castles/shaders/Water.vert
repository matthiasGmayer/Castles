#version 430
const int MAXLIGHTS = 12;
in vec3 in_position;
uniform mat4 transformation_matrix;
uniform mat4 view_matrix;
uniform mat4 projection_matrix;
uniform vec2 size;
uniform float waveSize;
uniform vec3 pointLight[MAXLIGHTS];
uniform vec3 dirLight[MAXLIGHTS];
uniform int pointLightNumber;
uniform int dirLightNumber;
out vec4 clipSpace;
out vec2 tex;
out vec3 toCamera;
out vec3 toLight[MAXLIGHTS];

void main(void){
    vec4 worldPosition = transformation_matrix * vec4(in_position, 1);

	tex = (in_position.xz/2+vec2(0.5)) * size / 100f / waveSize;

	for(int i = 0; i < pointLightNumber + dirLightNumber && i < MAXLIGHTS; i++){
	
	if(i >= pointLightNumber)
			toLight[i] = -dirLight[i];
		else
			toLight[i] = pointLight[i] - worldPosition.xyz;
	}

	toCamera = (inverse(view_matrix) * vec4(0,0,0,1) - worldPosition).xyz;

	clipSpace = projection_matrix * view_matrix * worldPosition;
	gl_Position = clipSpace;
}