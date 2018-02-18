#version 430
const int MAXLIGHTS = 12;
in vec3 in_position;
in vec3 in_normal;
in vec2 in_uv;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 transformation_matrix;
uniform vec3 pointLight[MAXLIGHTS];
uniform vec3 dirLight[MAXLIGHTS];
uniform int pointLightNumber;
uniform int dirLightNumber;
uniform vec4 clipPlane;
uniform float far;

out vec3 normal;
out vec3 toLight[MAXLIGHTS];
out vec3 toCamera;
out vec2 uv;
out float visibility;
out vec4 clipSpace;

void main(void){


	uv = in_uv;

	vec4 worldPosition = transformation_matrix * vec4(in_position, 1);
	vec4 positionToCamera = view_matrix * worldPosition;
	vec3 cameraPosition = (inverse(view_matrix) * vec4(0,0,0,1)).xyz;
	toCamera = cameraPosition - worldPosition.xyz;

	float distance = length(positionToCamera.xyz);
	visibility = clamp(1.5/(1+exp( 20/far*(distance - far / 3))),0,1);

	gl_ClipDistance[0] = dot(worldPosition, clipPlane);

	normal = vec3((transformation_matrix * vec4(in_normal,0)).xyz);
	for(int i = 0; i < pointLightNumber + dirLightNumber && i < MAXLIGHTS; i++){
	
	if(i >= pointLightNumber){
			toLight[i] = -dirLight[i - pointLightNumber];
			}
		else{
			toLight[i] = pointLight[i] - worldPosition.xyz;
			}
	}

    clipSpace = projection_matrix * positionToCamera;
	gl_Position = clipSpace;
}