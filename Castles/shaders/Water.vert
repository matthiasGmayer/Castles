#version 430

in vec3 in_position;
uniform mat4 transformation_matrix;
uniform mat4 view_matrix;
uniform mat4 projection_matrix;

void main(void){
    vec4 worldPosition = transformation_matrix * vec4(in_position, 1);


	gl_Position = projection_matrix * view_matrix * worldPosition;
}