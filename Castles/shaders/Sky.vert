#version 430

in vec3 in_position;

uniform mat4 projection_matrix;
uniform mat4 rotate_view_matrix;
uniform vec4 clipPlane;
out vec3 tex;

void main(void){
	vec4 worldPosition = vec4(in_position, 1);
	gl_ClipDistance[0] = dot(worldPosition, clipPlane);
    gl_Position = projection_matrix * rotate_view_matrix * worldPosition;
	tex = in_position;
}