#version 430
const int MAXLIGHTS = 12;
in vec3 in_position;
in vec3 in_normal;
in vec2 in_uv;
in vec3 in_tangent;

uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 transformation_matrix;
uniform vec3 pointLight[MAXLIGHTS];
uniform vec3 dirLight[MAXLIGHTS];
uniform int pointLightNumber;
uniform int dirLightNumber;
uniform vec4 clipPlane;
uniform float far;
uniform bool grassNormalMapping;
uniform bool stoneNormalMapping;

out vec3 normal;
out vec3 toLight[MAXLIGHTS];
out vec3 toCamera;
out vec2 uv;
out float visibility;
out vec4 clipSpace;
out float steepness;

void main(void){

	uv = in_uv;
	vec4 worldPosition = transformation_matrix * vec4(in_position, 1);
	mat4 modelViewMatrix = view_matrix * transformation_matrix;
	vec4 positionToCamera = modelViewMatrix * vec4(in_position, 1);

	float distance = length(positionToCamera.xyz);
	visibility = clamp(1.5/(1+exp( 20/far*(distance - far / 3))),0,1);

	gl_ClipDistance[0] = dot(worldPosition, clipPlane);

	vec4 worldNormal = (transformation_matrix * vec4(in_normal,0));
	steepness = worldNormal.y;
	normal = (view_matrix * worldNormal).xyz;
	
	vec3 norm = normalize(normal);
	vec3 tang =  normalize((modelViewMatrix * vec4(in_tangent, 0)).xyz);
	vec3 bitang =  normalize(cross(norm, tang));

	mat3 toTangent = mat3(
		tang.x, bitang.x, norm.x,
		tang.y, bitang.y, norm.y,
		tang.z, bitang.z, norm.z
	);

	bool normalMapping = grassNormalMapping || stoneNormalMapping;
	for(int i = 0; i < pointLightNumber + dirLightNumber && i < MAXLIGHTS; i++){
	if(i >= pointLightNumber)
			toLight[i] = (vec4((-dirLight[i - pointLightNumber]),1) * inverse(view_matrix)).xyz;
		else
			toLight[i] = (view_matrix * vec4(pointLight[i],1)).xyz - positionToCamera.xyz;
			if(normalMapping)
				toLight[i] = toTangent * toLight[i];
	}
	toCamera = -positionToCamera.xyz;
	if(normalMapping)
		toCamera = toTangent * toCamera;

    clipSpace = projection_matrix * positionToCamera;
	gl_Position = clipSpace;
}