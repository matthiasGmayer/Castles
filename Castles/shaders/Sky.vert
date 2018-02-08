#version 430

in vec3 in_position;

uniform mat4 projection_matrix;
uniform mat4 rotate_view_matrix;

out vec3 tex;

void main(void){
    gl_Position = projection_matrix * rotate_view_matrix * vec4(in_position, 1);
	tex = in_position;
}